namespace BarryFileMan.Rename.Models.Regex
{
    public class RegexRenameMatchGroupValue : BaseRenameMatchGroupValue
    {
        public int Index { get; }
        public int Length { get; }

        public RegexRenameMatchGroupValue(string value, int index, int length) 
            : base(value) 
        {
            Index = index;
            Length = length;
        }
    }
}
