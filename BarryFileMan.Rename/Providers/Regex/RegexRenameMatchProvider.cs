using BarryFileMan.Rename.Enums;
using BarryFileMan.Rename.Exceptions;
using BarryFileMan.Rename.Extensions;
using BarryFileMan.Rename.Interfaces;
using BarryFileMan.Rename.Models.Regex;
using System.Text.RegularExpressions;

namespace BarryFileMan.Rename.Providers.Regex
{
    public class RegexRenameMatchProvider : BaseRenameMatchProvider<RegexRenameProviderMatchOptions>
    {
        public RegexRenameMatchProvider() : base(RenameProviderTypes.Regex) { }

        public override IEnumerable<IRenameMatch>? Match(RegexRenameProviderMatchOptions? options)
        {
            var regexPattern = options?.RegexPattern;
            if (!regexPattern.IsValidRegex(out var regex, out var error))
            {
                throw new InvalidRegexException(error);
            }

            var matches = regex.Matches(options?.Input ?? string.Empty);
            List<RegexRenameMatch>? renameMatches = null;
            if (matches.Any())
            {
                renameMatches = new List<RegexRenameMatch>();
                foreach (Match match in matches.Cast<Match>())
                {
                    var renameMatch = new RegexRenameMatch(match.Index, match.Length);
                    var groups = match.Groups;
                    foreach (Group group in groups.Cast<Group>())
                    {
                        var groupName = group.Name;
                        if (groupName == "0")
                        {
                            groupName = "Match";
                        }

                        // Add tag if not already there
                        if (!renameMatch.Groups.ContainsKey(groupName))
                        {
                            renameMatch.Groups.Add(groupName, new List<IRenameMatchGroupValue>());
                        }

                        // Add capture values to tag
                        var captures = group.Captures;
                        foreach (Capture capture in captures.Cast<Capture>())
                        {
                            renameMatch.Groups[groupName].Add(new RegexRenameMatchGroupValue(capture.Value, capture.Index, capture.Length));
                        }
                    }

                    // Add match to collection
                    renameMatches.Add(renameMatch);
                }
            }

            return renameMatches;
        }

        public override Task<IEnumerable<IRenameMatch>?> MatchAsync(RegexRenameProviderMatchOptions? options)
        {
            return Task.Run(() => Match(options));
        }
    }
}
