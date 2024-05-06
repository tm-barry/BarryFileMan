namespace BarryFileMan.Rename.Models.Regex
{
    public class RegexRenameProviderMatchOptions
    {
        public string RegexPattern { get; set; }

        public RegexRenameProviderMatchOptions() : this(string.Empty) { }
        public RegexRenameProviderMatchOptions(string regexPattern)
        {
            RegexPattern = regexPattern;
        }
    }
}
