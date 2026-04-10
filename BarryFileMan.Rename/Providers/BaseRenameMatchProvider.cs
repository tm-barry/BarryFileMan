using BarryFileMan.Rename.Enums;
using BarryFileMan.Rename.Interfaces;
using BarryFileMan.Rename.Models;
using System.Text.RegularExpressions;
using BarryFileMan.Rename.Constants;

namespace BarryFileMan.Rename.Providers
{
    public abstract class BaseRenameMatchProvider<TMatchOptions>(RenameProviderTypes providerType)
        : IRenameMatchProvider<TMatchOptions>
        where TMatchOptions : class
    {
        public RenameProviderTypes ProviderType { get; } = providerType;

        public abstract IEnumerable<IRenameMatch>? Match(TMatchOptions? options);
        public abstract Task<IEnumerable<IRenameMatch>?> MatchAsync(TMatchOptions? options);

        // =========================================================
        // Rename Engine
        // =========================================================

        public RenameResult Rename(
            IEnumerable<IRenameMatch> matches,
            string renamePattern,
            string? defaultTagFallbackValue = null)
        {
            var errors = new List<string>();

            var renameMatches = matches.ToList();
            if (renameMatches.Count == 0)
                return new RenameResult(renamePattern, ["No matches found!"]);

            var context = BuildContext(renameMatches);

            var result = renamePattern;
            foreach (var tag in ExtractTags(renamePattern))
            {
                try
                {
                    var expr = TagParser.ParseTag(tag.Content, defaultTagFallbackValue);
                    var evaluated = expr.Evaluate(context);

                    result = result.Replace(tag.Full, evaluated ?? tag.Full);
                }
                catch (Exception ex)
                {
                    errors.Add(ex.Message);
                }
            }

            return new RenameResult(result, errors.Count > 0 ? errors : null);
        }
        
        private static IEnumerable<(string Full, string Content)> ExtractTags(string input)
        {
            var results = new List<(string, string)>();

            int depth = 0;
            int start = -1;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '<')
                {
                    if (depth == 0)
                        start = i;

                    depth++;
                }
                else if (input[i] == '>')
                {
                    depth--;

                    if (depth == 0 && start != -1)
                    {
                        var full = input[start..(i + 1)];
                        var content = input[(start + 1)..i];
                        results.Add((full, content));
                    }
                }
            }

            return results;
        }

        // =========================================================
        // Context Builder
        // =========================================================

        private static RenameContext BuildContext(IEnumerable<IRenameMatch> matches)
        {
            var renameMatches = matches.ToList();

            var firstMatch = renameMatches.FirstOrDefault();
            var lastMatch = renameMatches.LastOrDefault();

            string GetValue(IRenameMatch? m, string key)
            {
                return m?.Groups.TryGetValue(key, out var list) == true
                    ? list.FirstOrDefault()?.Value ?? ""
                    : "";
            }

            return new RenameContext
            {
                Input = GetValue(firstMatch, GroupTags.Input),
                FileName = GetValue(firstMatch, GroupTags.File),
                Matches = renameMatches
            };
        }
    }

    public class RenameContext
    {
        public string Input { get; init; } = "";
        public string FileName { get; init; } = "";
        
        public List<IRenameMatch> Matches { get; init; } = [];
    }

    public class TagExpression(string source)
    {
        private string Source { get; } = source;
        public int MatchIndex { get; init; } = 0;
        public List<IFunction> Functions { get; } = [];
        public string? Fallback { get; init; }

        public string? Evaluate(RenameContext context)
        {
            var value = Resolve(context);

            if (value != null)
            {
                value = Functions.Aggregate(value, (current, fn) => fn.Apply(current, context));
            }

            return string.IsNullOrEmpty(value)
                ? Fallback
                : value;
        }

        private string? Resolve(RenameContext ctx)
        {
            var match = MatchIndex switch
            {
                -1 => ctx.Matches.LastOrDefault(),
                _ => ctx.Matches.ElementAtOrDefault(MatchIndex)
            };

            if (match == null)
                return null;

            return match.Groups.TryGetValue(Source, out var group)
                ? group.FirstOrDefault()?.Value ?? ""
                : null;
        }
    }

    public static class TagParser
    {
        public static TagExpression ParseTag(string raw, string? fallback)
        {
            raw = ExtractFallback(raw, ref fallback);

            var (cleaned, matchIndex) = ParseMatchIndex(raw);
            raw = cleaned;

            var parts = SplitByDotRespectingQuotes(raw);

            var tag = new TagExpression(parts[0])
            {
                Fallback = fallback,
                MatchIndex = matchIndex
            };

            for (var i = 1; i < parts.Count; i++)
            {
                tag.Functions.Add(FunctionParser.Parse(parts[i]));
            }

            return tag;
        }
        
        private static string ExtractFallback(string raw, ref string? fallback)
        {
            var fallbackIndex = raw.IndexOf("??", StringComparison.Ordinal);

            if (fallbackIndex < 0)
                return raw;

            var main = raw[..fallbackIndex].Trim();
            var fallbackPart = raw[(fallbackIndex + 2)..].Trim();

            fallback = ParseFallbackValue(fallbackPart);

            return main;
        }
        
        private static (string cleaned, int matchIndex) ParseMatchIndex(string raw)
        {
            var start = raw.IndexOf('{');
            var end = raw.IndexOf('}');

            if (start < 0 || end < 0 || end <= start)
                return (raw, 0);

            var inside = raw[(start + 1)..end].Trim();

            if (!int.TryParse(inside, out var index))
                index = 0;

            var cleaned = raw[..start] + raw[(end + 1)..];

            return (cleaned, index);
        }
        
        private static string ParseFallbackValue(string input)
        {
            input = input.Trim();

            // allow: 'text'
            if (input.StartsWith($"'") && input.EndsWith($"'") && input.Length >= 2)
                return input[1..^1];

            // allow: empty or raw text
            return input;
        }
        
        private static List<string> SplitByDotRespectingQuotes(string input)
        {
            var result = new List<string>();
            var current = new System.Text.StringBuilder();

            var inQuotes = false;
            var parenDepth = 0;

            foreach (var c in input)
            {
                switch (c)
                {
                    case '\'':
                        inQuotes = !inQuotes;
                        current.Append(c);
                        continue;

                    case '(' when !inQuotes:
                        parenDepth++;
                        current.Append(c);
                        continue;

                    case ')' when !inQuotes:
                        parenDepth--;
                        current.Append(c);
                        continue;

                    case '.' when !inQuotes && parenDepth == 0:
                        result.Add(current.ToString());
                        current.Clear();
                        continue;

                    default:
                        current.Append(c);
                        break;
                }
            }

            if (current.Length > 0)
                result.Add(current.ToString());

            return result;
        }
    }

    // =============================================================
    // Functions
    // =============================================================

    public interface IFunction
    {
        string Apply(string input, RenameContext context);
    }
    
    public interface IParam
    {
        string? Evaluate(RenameContext context);
    }
    
    public class LiteralParam(string value) : IParam
    {
        public string? Evaluate(RenameContext context) => value;
    }
    
    public class TagParam(TagExpression expression) : IParam
    {
        public string? Evaluate(RenameContext context)
            => expression.Evaluate(context);
    }

    public static class FunctionParser
    {
        public static IFunction Parse(string input)
        {
            var nameEnd = input.IndexOf('(');

            if (nameEnd == -1)
            {
                return input.ToLower() switch
                {
                    "trim" => new TrimFunction(),
                    "separate" => new SeparateFunction(),
                    _ => throw new Exception($"Unknown function: {input}")
                };
            }

            var name = input[..nameEnd].ToLower();
            var args = input[(nameEnd + 1)..^1];

            if (Aliases.TryGetValue(name, out var fullName))
                name = fullName;

            return name switch
            {
                "append" => new AppendFunction(Unquote(args)),
                "pad" => ParsePad(args),
                "prepend" => new PrependFunction(Unquote(args)),
                "replace" => ParseReplace(args),
                "separate" => new SeparateFunction(),
                "trim" => ParseTrim(args),
                _ => throw new Exception($"Unknown function: {name}")
            };
        }
        
        private static readonly Dictionary<string, string> Aliases = new()
        {
            { "ap", "append" },
            { "pd", "pad" },
            { "pp", "prepend" },
            { "rp", "replace" },
            { "sp", "separate" },
            { "tm", "trim" },
        };
        
        private static IParam ParseParam(string input)
        {
            input = input.Trim();
            
            if (input.StartsWith('<') && input.EndsWith('>'))
            {
                var inner = input[1..^1];
                return new TagParam(TagParser.ParseTag(inner, null));
            }
            
            return new LiteralParam(Unquote(input));
        }

        private static PadFunction ParsePad(string args)
        {
            var parts = SplitArgs(args);

            if (parts.Length < 3)
                throw new Exception($"pad() requires 3 arguments: type, char, length");

            var typeRaw = Unquote(parts[0]).ToLowerInvariant();
            var padChar = Unquote(parts[1]);

            if (string.IsNullOrEmpty(padChar))
                throw new Exception("pad() char cannot be empty");

            if (!int.TryParse(Unquote(parts[2]), out var length))
                throw new Exception("pad() length must be a number");

            var type = typeRaw switch
            {
                "left" or "l" => PadType.Left,
                "right" or "r" => PadType.Right,
                _ => throw new Exception($"Invalid pad type: {parts[0]}")
            };

            return new PadFunction(type, padChar[0], length);
        }

        private static ReplaceFunction ParseReplace(string args)
        {
            var parts = SplitArgs(args);

            var from = ParseParam(parts[0]);
            var to = ParseParam(parts[1]);

            return new ReplaceFunction(from, to);
        }
        
        private static TrimFunction ParseTrim(string args)
        {
            var value = Unquote(args).Trim().ToLowerInvariant();

            return value switch
            {
                "left" or "l" => new TrimFunction(TrimType.Left),
                "right" or "r" => new TrimFunction(TrimType.Right),
                "both" or "b" or "" => new TrimFunction(TrimType.Both),
                _ => throw new Exception($"Invalid trim type: {args}")
            };
        }

        private static string[] SplitArgs(string args)
        {
            var result = new List<string>();
            var current = new System.Text.StringBuilder();

            var inQuotes = false;
            var parenDepth = 0;
            var tagDepth = 0;

            foreach (var c in args)
            {
                switch (c)
                {
                    case '\'':
                        inQuotes = !inQuotes;
                        current.Append(c);
                        continue;

                    case '(' when !inQuotes:
                        parenDepth++;
                        current.Append(c);
                        continue;

                    case ')' when !inQuotes:
                        parenDepth--;
                        current.Append(c);
                        continue;

                    case '<' when !inQuotes:
                        tagDepth++;
                        current.Append(c);
                        continue;

                    case '>' when !inQuotes:
                        tagDepth--;
                        current.Append(c);
                        continue;

                    case ',' when !inQuotes && parenDepth == 0 && tagDepth == 0:
                        result.Add(current.ToString().Trim());
                        current.Clear();
                        continue;

                    default:
                        current.Append(c);
                        break;
                }
            }

            if (current.Length > 0)
                result.Add(current.ToString().Trim());

            return result.ToArray();
        }

        private static string Unquote(string value)
        {
            return value.Trim().Trim('\'');
        }
    }
    
    public class AppendFunction(string value) : IFunction
    {
        public string Apply(string input, RenameContext context) => input + value;
    }
    
    public enum PadType
    {
        Left,
        Right
    }

    public class PadFunction(PadType type, char padChar, int length) : IFunction
    {
        public string Apply(string input, RenameContext context)
        {
            return type switch
            {
                PadType.Left => input.PadLeft(length, padChar),
                PadType.Right => input.PadRight(length, padChar),
                _ => input
            };
        }
    }
    
    public class PrependFunction(string value) : IFunction
    {
        public string Apply(string input, RenameContext context) => value + input;
    }

    public class ReplaceFunction(IParam from, IParam to) : IFunction
    {
        public string Apply(string input, RenameContext context)
        {
            var fromVal = from.Evaluate(context);
            var toVal = to.Evaluate(context) ?? "";

            if (string.IsNullOrEmpty(fromVal))
                return input;

            return input.Replace(fromVal, toVal);
        }
    }
    
    public class SeparateFunction : IFunction
    {
        public string Apply(string input, RenameContext context)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var sb = new System.Text.StringBuilder();
            sb.Append(input[0]);

            for (var i = 1; i < input.Length; i++)
            {
                var current = input[i];
                var previous = input[i - 1];
                char? next = i < input.Length - 1 ? input[i + 1] : null;

                var isCurrentUpper = char.IsUpper(current);
                var isPreviousLower = char.IsLower(previous);
                var isPreviousUpper = char.IsUpper(previous);
                var isCurrentDigit = char.IsDigit(current);
                var isPreviousDigit = char.IsDigit(previous);

                var isAcronymBoundary =
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
        }
    }
    
    public enum TrimType
    {
        Both,
        Left,
        Right
    }

    public class TrimFunction(TrimType type = TrimType.Both) : IFunction
    {
        public string Apply(string input, RenameContext context)
        {
            var result = type switch
            {
                TrimType.Left => input.TrimStart(),
                TrimType.Right => input.TrimEnd(),
                _ => input.Trim()
            };

            return result;
        }
    }
}