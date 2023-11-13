using BarryFileMan.Rename.Enums;
using BarryFileMan.Rename.Interfaces;
using BarryFileMan.Rename.Models;
using System.Text.RegularExpressions;

namespace BarryFileMan.Rename.Providers
{
    public abstract class BaseRenameProvider<TMatchOptions> : IRenameProvider<TMatchOptions> where TMatchOptions : class
    {
        private static readonly string _renameTagPattern = "(?:<(?<=<)(?<tag>.*?)(?=>)>)";

        public RenameProviderTypes ProviderType { get; }

        public BaseRenameProvider(RenameProviderTypes providerType)
        {
            ProviderType = providerType;
        }

        public abstract IEnumerable<IRenameMatch>? Match(string input, TMatchOptions? options);

        public abstract Task<IEnumerable<IRenameMatch>?> MatchAsync(string input, TMatchOptions? options);

        public RenameResult Rename(IEnumerable<IRenameMatch> matches, string renamePattern)
        {
            var renamedString = renamePattern;
            var errors = new List<string>();
            if (matches == null || !matches.Any())
            {
                errors.Add("No matches found!");
            }
            else
            {
                var regex = new System.Text.RegularExpressions.Regex(_renameTagPattern);
                var renamePatternMatches = regex.Matches(renamePattern);
                if (renamePatternMatches.Any())
                {
                    foreach (Match renameMatch in renamePatternMatches.Cast<Match>())
                    {
                        var renameTag = new RenameTag(renameMatch.Value, renameMatch.Groups["tag"].Value, renameMatch.Index, renameMatch.Length);
                        if (string.IsNullOrEmpty(renameTag.Error) && renameTag.Functions.All((function) => string.IsNullOrEmpty(function.Error)))
                        {
                            var match = matches.ElementAtOrDefault(renameTag.MatchIndex);
                            if (match != null)
                            {
                                IRenameMatchGroupValue? groupValue = null;
                                if (match.Groups.ContainsKey(renameTag.TagName))
                                {
                                    groupValue = match.Groups[renameTag.TagName].ElementAtOrDefault(renameTag.GroupIndex);
                                }

                                if (groupValue != null)
                                {
                                    var renamedTagStr = BaseRenameProvider<TMatchOptions>.CalculateRenamedTag(groupValue, renameTag);
                                    renamedString = renamedString.Replace(renameTag.Tag, renamedTagStr);
                                }
                            }
                            else
                            {
                                errors.Add($"No match found for <{renameTag.TagName}{{{renameTag.MatchIndex}}}...>");
                            }
                        }
                        else
                        {
                            if(!string.IsNullOrEmpty(renameTag.Error))
                            {
                                errors.Add(renameTag.Error);
                            }

                            foreach(var function in renameTag.Functions)
                            {
                                if(!string.IsNullOrEmpty(function.Error))
                                {
                                    errors.Add(function.Error);
                                }
                            }
                        }
                    }
                }
            }

            return new RenameResult(renamedString, errors.Count > 0 ? errors : null);
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

    internal class RenameTag
    {
        private static readonly string _renameInnerTagPattern = "\\G\\s*(?<tagName>(?:[a-zA-Z]|\\d)+)(?:{\\s*(?<matchIndex>\\d+)\\s*})?(?:\\[\\s*(?<groupIndex>\\d+)\\s*\\])?(?<function>.(?<functionName>(?:[a-zA-Z]|\\d)+)(?:\\((?<functionParams>.*?)\\)))*?\\s*$";

        public string Tag { get; }
        public string Value { get; }
        public int Index { get; }
        public int Length { get; }
        public string TagName { get; private set; } = string.Empty;
        public int MatchIndex { get; private set; } = 0;
        public int GroupIndex { get; private set; } = 0;
        public IList<RenameTagFunction> Functions { get; } = new List<RenameTagFunction>();
        public string? Error { get; private set; }

        public RenameTag(string tag, string value, int index, int length)
        {
            Tag = tag;
            Value = value;
            Index = index;
            Length = length;

            ParseValue(value);
        }

        private void ParseValue(string value)
        {
            var regex = new System.Text.RegularExpressions.Regex(_renameInnerTagPattern);
            var match = regex.Match(value);
            if(match?.Success == true)
            {
                Group? group = match.Groups["tagName"];
                if (group != null)
                {
                    TagName = group.Value;
                }

                group = match.Groups["matchIndex"];
                if (group != null && int.TryParse(group.Value, out int matchIndex))
                {
                    MatchIndex = matchIndex;
                }

                group = match.Groups["groupIndex"];
                if (group != null && int.TryParse(group.Value, out int groupIndex))
                {
                    GroupIndex = groupIndex;
                }

                var functionNames = match.Groups["functionName"]?.Captures;
                var functionParams = match.Groups["functionParams"]?.Captures;
                if (functionNames != null && functionParams != null && functionNames.Count == functionParams.Count)
                {
                    for(int i = 0; i < functionNames.Count; i++)
                    {
                        Functions.Add(new RenameTagFunction(functionNames[i].Value, functionParams[i].Value));
                    }
                }
            }
            else
            {
                Error = $"<{Value}> tag is invalid!";
            }
        }
    }

    internal class RenameTagFunction
    {
        private static readonly string _renameFunctionPadParamPattern = "\\G\\s*(?<type>right|left)\\s*,\\s*\'(?<char>.)\'\\s*,\\s*(?<length>\\d+)\\s*$";
        private static readonly string _renameFunctionReplacePattern = "\\G\\s*\'(?<input>.*)\'\\s*,\\s*\'(?<replace>.*)\'\\s*$";
        private static readonly string _renameFunctionTrimPattern = "\\G\\s*(?<type>left|right|both)\\s*$";

        public string Name { get; }
        public string Params { get; }
        public Func<string, string> Function { get; private set; } = (str) => str;
        public string? Error { get; private set; }


        public RenameTagFunction(string name, string @params)
        {
            Name = name;
            Params = @params;

            SetFunction(name, @params);
        }

        private void SetFunction(string name, string @params)
        {
            switch(name.ToLower())
            {
                case "pad":
                    SetPadFunction(@params);
                    break;
                case "replace":
                    SetReplaceFunction(@params);
                    break;
                case "trim":
                    SetTrimFunction(@params);
                    break;
                default:
                    Error = $"{name} is not a valid function!";
                    break;
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
                        "right" => str.PadRight(padLength, padChar[0]),
                        "left" => str.PadLeft(padLength, padChar[0]),
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
            var regex = new System.Text.RegularExpressions.Regex(_renameFunctionReplacePattern);
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
        private void SetTrimFunction(string @params)
        {
            var regex = new System.Text.RegularExpressions.Regex(_renameFunctionTrimPattern);
            var match = regex.Match(@params);
            if (match?.Success == true)
            {
                var trimType = match.Groups["type"].Value;

                Function = (str) =>
                {
                    return trimType switch
                    {
                        "left" => str.TrimStart(),
                        "right" => str.TrimEnd(),
                        _ => str.Trim(),
                    };
                };
            }
            else
            {
                Error = $"{Name}({Params}) params are not valid!";
            }
        }

    }
}
