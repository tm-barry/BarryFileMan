using System.Text.RegularExpressions;

namespace BarryFileMan.Rename.Helpers;

public static partial class RenameHelper
{
    public static string GetNextAvailableFileName(
        string name,
        HashSet<string> used)
    {
        var baseName = NormalizeBaseName(name);

        if (used.Add(baseName))
            return baseName;

        var i = 1;
        string candidate;

        do
        {
            candidate = $"{baseName} ({i})";
            i++;
        }
        while (!used.Add(candidate));

        return candidate;
    }

    public static string NormalizeBaseName(string name)
    {
        var match = NormalizedBaseNameRegex().Match(name);
        return match.Groups[1].Value;
    }

    [GeneratedRegex(@"^(.*?)( \(\d+\))?$")]
    private static partial Regex NormalizedBaseNameRegex();
}