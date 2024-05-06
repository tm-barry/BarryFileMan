namespace BarryFileMan.Rename.Models.TMDB
{
    public class TMDBRenameProviderMatchOptions
    {
        public string APIKey { get; set; }

        public TMDBRenameProviderMatchOptions() : this(string.Empty) { }
        public TMDBRenameProviderMatchOptions(string apiKey)
        {
            APIKey = apiKey;
        }
    }
}
