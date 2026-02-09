using BarryFileMan.Rename.Enums;
using BarryFileMan.Rename.Interfaces;
using BarryFileMan.Rename.Models;
using System.Text.RegularExpressions;

namespace BarryFileMan.Rename.Providers
{
    public abstract class BaseRenameMatchProvider<TMatchOptions> : IRenameMatchProvider<TMatchOptions> where TMatchOptions : class
    {
        private static readonly string _renameTagPattern = "(?:<(?<=<)(?<tag>.*?)(?=>)>)";

        public RenameProviderTypes ProviderType { get; }

        public BaseRenameMatchProvider(RenameProviderTypes providerType)
        {
            ProviderType = providerType;
        }

        public abstract IEnumerable<IRenameMatch>? Match(TMatchOptions? options);

        public abstract Task<IEnumerable<IRenameMatch>?> MatchAsync(TMatchOptions? options);

        public RenameResult Rename(IEnumerable<IRenameMatch> matches, string renamePattern, string? defaultTagFallbackValue = null)
        {
            var output = renamePattern;
            var tags = new List<RenameTag>();
            var errors = new List<string>();
            if (matches == null || !matches.Any())
            {
                errors.Add("No matches found!");
            }

            var regex = new System.Text.RegularExpressions.Regex(_renameTagPattern);
            var renamePatternMatches = regex.Matches(renamePattern);
            if (renamePatternMatches.Any())
            {
                foreach (Match renameMatch in renamePatternMatches.Cast<Match>())
                {
                    var renameTag = new RenameTag(renameMatch.Value, renameMatch.Groups["tag"].Value, renameMatch.Index, renameMatch.Length, defaultTagFallbackValue);

                    if (string.IsNullOrEmpty(renameTag.Error) && renameTag.Functions.All((function) => string.IsNullOrEmpty(function.Error)))
                    {
                        IRenameMatch? match = null;
                        if (renameTag.MatchIndex == -1)
                            match = matches?.LastOrDefault();
                        else
                            match = matches?.ElementAtOrDefault(renameTag.MatchIndex);

                        if (match != null)
                        {
                            IRenameMatchGroupValue? groupValue = null;
                            if (match.Groups.ContainsKey(renameTag.TagName))
                            {
                                if (renameTag.GroupIndex == -1)
                                    groupValue = match.Groups[renameTag.TagName].LastOrDefault();
                                else
                                    groupValue = match.Groups[renameTag.TagName].ElementAtOrDefault(renameTag.GroupIndex);
                            }

                            if (groupValue != null)
                            {
                                try
                                {
                                    var renamedTagStr = BaseRenameMatchProvider<TMatchOptions>.CalculateRenamedTag(groupValue, renameTag);
                                    output = output.Replace(renameTag.Tag, renamedTagStr);
                                }
                                catch (Exception ex)
                                {
                                    errors.Add(ex.Message);
                                }
                            }
                            else
                            {
                                if (renameTag.FallbackValue != null)
                                {
                                    output = output.Replace(renameTag.Tag, renameTag.FallbackValue);
                                }
                                else
                                {
                                    errors.Add($"No match found for { renameTag.Tag }");
                                }
                            }
                        }
                        else
                        {
                            if (renameTag.FallbackValue != null)
                            {
                                output = output.Replace(renameTag.Tag, renameTag.FallbackValue);
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(renameTag.Error))
                        {
                            errors.Add(renameTag.Error);
                        }

                        foreach (var function in renameTag.Functions)
                        {
                            if (!string.IsNullOrEmpty(function.Error))
                            {
                                errors.Add(function.Error);
                            }
                        }
                    }

                    tags.Add(renameTag);
                }
            }

            return new RenameResult(output, tags, errors.Count > 0 ? errors : null);
        }

        private static string CalculateRenamedTag(IRenameMatchGroupValue groupValue, RenameTag renameTag)
        {
            var renamedValue = groupValue.Value;
            foreach(var function in renameTag.Functions)
            {
                renamedValue = function.Function(renamedValue);
            }
            return renamedValue;
        }
    }

    public class RenameTag
    {
        private static readonly string _renameInnerTagPattern = "\\G\\s*(?<tagName>(?:[a-zA-Z]|\\d)+)(?:{\\s*(?<matchIndex>-?\\d+)\\s*})?(?:\\[\\s*(?<groupIndex>-?\\d+)\\s*\\])?(?<function>.(?<functionName>(?:[a-zA-Z]|\\d)+)(?:\\((?<functionParams>.*?)\\)))*?(?:\\s*\\?\\?\\s*\'(?<fallbackValue>.*)\')?\\s*$";

        /// <summary>
        /// Entire tag value including angle brackets, tag name, and functions ex.. <tagName.functions()>
        /// </summary>
        public string Tag { get; }
        /// <summary>
        /// Inner tag value excluding angle brackets
        /// </summary>
        public string InnerTag { get; }
        public int Index { get; }
        public int Length { get; }
        public string TagName { get; private set; } = string.Empty;
        public int MatchIndex { get; private set; } = 0;
        public int GroupIndex { get; private set; } = 0;
        public IList<RenameTagFunction> Functions { get; } = new List<RenameTagFunction>();
        public string? FallbackValue { get; set; }
        public string? Error { get; private set; }

        public RenameTag(string tag, string innerTag, int index, int length, string? fallbackValue)
        {
            Tag = tag;
            InnerTag = innerTag;
            Index = index;
            Length = length;
            FallbackValue = fallbackValue;

            ParseInnerTag(innerTag);
        }

        private void ParseInnerTag(string value)
        {
            var regex = new System.Text.RegularExpressions.Regex(_renameInnerTagPattern);
            var match = regex.Match(value);
            if(match?.Success == true)
            {
                Group? group = match.Groups["tagName"];
                if (group != null && group.Captures.Any())
                {
                    TagName = group.Value;
                }

                group = match.Groups["matchIndex"];
                if (group != null && group.Captures.Any() && int.TryParse(group.Value, out int matchIndex))
                {
                    MatchIndex = matchIndex;
                }

                group = match.Groups["groupIndex"];
                if (group != null && group.Captures.Any() && int.TryParse(group.Value, out int groupIndex))
                {
                    GroupIndex = groupIndex;
                }

                var functions = match.Groups["function"]?.Captures;
                var functionNames = match.Groups["functionName"]?.Captures;
                var functionParams = match.Groups["functionParams"]?.Captures;
                if (functions != null && functionNames != null && functionParams != null 
                    && functions.Count == functionNames.Count 
                    && functionNames.Count == functionParams.Count)
                {
                    for(int i = 0; i < functionNames.Count; i++)
                    {
                        Functions.Add(new RenameTagFunction(functionNames[i].Value, functionNames[i].Index, functionNames[i].Length,
                            functionParams[i].Value, functionParams[i].Index, functionParams[i].Length,
                            functions[i].Index, functions[i].Length));
                    }
                }

                group = match.Groups["fallbackValue"];
                if (group != null && group.Captures.Any())
                {
                    FallbackValue = group.Value;
                }
            }
            else
            {
                Error = $"<{InnerTag}> tag is invalid!";
            }
        }
    }

    public class RenameTagFunction
    {
        private static readonly string _renameFunctionAppendPrependParamPattern = "\\G\\s*'(?<value>.*)'\\s*$";
        private static readonly string _renameFunctionPadParamPattern = "\\G\\s*(?<type>left|l|right|r)\\s*,\\s*\'(?<char>.)\'\\s*,\\s*(?<length>\\d+)\\s*$";
        private static readonly string _renameFunctionReplaceParamPattern = "\\G\\s*\'(?<input>.*)\'\\s*,\\s*\'(?<replace>.*)\'\\s*$";
        private static readonly string _renameFunctionTrimParamPattern = "\\G\\s*(?<type>left|l|right|r|both|b)?\\s*$";
        private static readonly string _renameFunctionNoParamPattern = "\\G\\s*$";
        
        public string Name { get; }
        public int NameIndex { get; }
        public int NameLength { get; }
        public string Params { get; }
        public int ParamsIndex { get; }
        public int ParamsLength { get; }
        public int Index { get; }
        public int Length { get; }
        public Func<string, string> Function { get; private set; } = (str) => str;
        public string? Error { get; private set; }


        public RenameTagFunction(string name, int nameIndex, int nameLength, 
            string @params, int paramsIndex, int paramsLength,
            int index, int length)
        {
            Name = name;
            NameIndex = nameIndex;
            NameLength = nameLength;
            Params = @params;
            ParamsIndex = paramsIndex;
            ParamsLength = paramsLength;
            Index = index;
            Length = length;

            SetFunction(name, @params);
        }

        private void SetFunction(string name, string @params)
        {
            switch(name.ToLower())
            {
                case "append":
                case "ap":
                    SetAppendPrependFunction(@params, true);
                    break;
                case "pad":
                case "pd":
                    SetPadFunction(@params);
                    break;
                case "prepend":
                case "pp":
                    SetAppendPrependFunction(@params, false);
                    break;
                case "replace":
                case "rp":
                    SetReplaceFunction(@params);
                    break;
                case "separate":
                case "sp":
                    SetSeparateFunction(@params);
                    break;
                case "trim":
                case "tm":
                    SetTrimFunction(@params);
                    break;
                default:
                    Error = $"{name} is not a valid function!";
                    break;
            }
        }

        private void SetAppendPrependFunction(string @params, bool append = true)
        {
            var regex = new System.Text.RegularExpressions.Regex(_renameFunctionAppendPrependParamPattern);
            var match = regex.Match(@params);
            if (match?.Success == true)
            {
                var value = match.Groups["value"].Value;

                Function = (str) =>
                {
                    return append ? str + value : value + str;
                };
            }
            else
            {
                Error = $"{Name}({Params}) params are not valid!";
            }
        }

        private void SetPadFunction(string @params)
        {
            var regex = new System.Text.RegularExpressions.Regex(_renameFunctionPadParamPattern);
            var match = regex.Match(@params);
            if (match?.Success == true)
            {
                var padType = match.Groups["type"].Value;
                var padChar = match.Groups["char"].Value;
                var padLengthStr = match.Groups["length"].Value;
                var padLength = int.Parse(padLengthStr);

                Function = (str) =>
                {
                    return padType switch
                    {
                        "left" or "l" => str.PadLeft(padLength, padChar[0]),
                        "right" or "r" => str.PadRight(padLength, padChar[0]),
                        _ => str,
                    };
                };
            }
            else
            {
                Error = $"{Name}({Params}) params are not valid!";
            }
        }

        private void SetReplaceFunction(string @params)
        {
            var regex = new System.Text.RegularExpressions.Regex(_renameFunctionReplaceParamPattern);
            var match = regex.Match(@params);
            if (match?.Success == true)
            {
                var input = match.Groups["input"].Value;
                var replace = match.Groups["replace"].Value;

                Function = (str) =>
                {
                    return str.Replace(input, replace);
                };
            }
            else
            {
                Error = $"{Name}({Params}) params are not valid!";
            }
        }
        
        private void SetSeparateFunction(string @params)
        {
            var regex = new System.Text.RegularExpressions.Regex(_renameFunctionNoParamPattern);
            var match = regex.Match(@params);
            if (match?.Success == true)
            {
                Function = (input) =>
                {
                    if (string.IsNullOrWhiteSpace(input))
                        return input;

                    var sb = new System.Text.StringBuilder();
                    sb.Append(input[0]);

                    for (var i = 1; i < input.Length; i++)
                    {
                        char current = input[i];
                        char previous = input[i - 1];
                        char? next = i < input.Length - 1 ? input[i + 1] : null;

                        bool isCurrentUpper = char.IsUpper(current);
                        bool isPreviousLower = char.IsLower(previous);
                        bool isPreviousUpper = char.IsUpper(previous);
                        bool isCurrentDigit = char.IsDigit(current);
                        bool isPreviousDigit = char.IsDigit(previous);

                        bool isAcronymBoundary =
                            isPreviousUpper &&
                            isCurrentUpper &&
                            next.HasValue &&
                            char.IsLower(next.Value);

                        if (
                            (isCurrentUpper && isPreviousLower) ||        // camelCase boundary
                            isAcronymBoundary ||                          // XMLParser boundary
                            (isCurrentDigit && !isPreviousDigit) ||       // Word1
                            (!isCurrentDigit && isPreviousDigit)          // 1Word
                        )
                        {
                            sb.Append(' ');
                        }

                        sb.Append(current);
                    }

                    return sb.ToString();
                };
            }
            else
            {
                Error = $"{Name}({Params}) params are not valid!";
            }
        }
        
        private void SetTrimFunction(string @params)
        {
            var regex = new System.Text.RegularExpressions.Regex(_renameFunctionTrimParamPattern);
            var match = regex.Match(@params);
            if (match?.Success == true)
            {
                var trimType = match.Groups["type"].Value;

                Function = (str) =>
                {
                    // trim end whitespace
                    str = trimType switch
                    {
                        "left" or "l" => str.TrimStart(),
                        "right" or "r" => str.TrimEnd(),
                        _ => str.Trim(),
                    };
                    
                    // trim extra whitespace
                    str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+", " ");

                    return str;
                };
            }
            else
            {
                Error = $"{Name}({Params}) params are not valid!";
            }
        }
    }
}
