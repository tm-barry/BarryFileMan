namespace BarryFileMan.Rename.Models.TMDB
{
    public class TMDBMovieTvRenameProviderMatchOptions
    {
        public string APIKey { get; set; }
        public bool IncludeAdult { get; set; }
        public string Query { get; set; }
        public string Year { get; set; }
        public string Language { get; set; }

        public TMDBMovieTvRenameProviderMatchOptions() : this(string.Empty, false, string.Empty, string.Empty, string.Empty) { }
        public TMDBMovieTvRenameProviderMatchOptions(string apiKey, bool includeAdult, string query, string year, string language)
        {
            APIKey = apiKey;
            IncludeAdult = includeAdult;
            Query = query;
            Year = year;
            Language = language;
        }
    }
}
