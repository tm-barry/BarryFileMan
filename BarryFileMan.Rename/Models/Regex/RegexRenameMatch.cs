using BarryFileMan.Rename.Interfaces;

namespace BarryFileMan.Rename.Models.Regex
{
    public class RegexRenameMatch : BaseRenameMatch
    {
        public int Index { get; }
        public int Length { get; }

        public RegexRenameMatch(int index, int length)
        {
            Index = index;
            Length = length;
        }
    }
}
