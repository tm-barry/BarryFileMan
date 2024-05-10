namespace BarryFileMan.Rename.Models.Regex
{
    public class RegexRenameProviderMatchOptions
    {
        public string RegexPattern { get; set; }

        public string Input { get; set; }

        public RegexRenameProviderMatchOptions() : this(string.Empty, string.Empty) { }
        public RegexRenameProviderMatchOptions(string regexPattern, string input)
        {
            RegexPattern = regexPattern;
            Input = input;
        }
    }
}
