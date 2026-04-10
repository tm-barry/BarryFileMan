using System.Collections.Frozen;

namespace BarryFileMan.Rename.Constants;

public static class GroupTags
{
    public const string Input = "Input";
    public const string File = "File";
    public const string Match = "Match";

    public static readonly FrozenSet<string> ReservedTags =
        FrozenSet.ToFrozenSet(new[]
        {
            Input,
            File,
            Match
        });

}