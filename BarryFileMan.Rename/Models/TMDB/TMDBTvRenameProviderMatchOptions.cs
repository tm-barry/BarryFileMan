namespace BarryFileMan.Rename.Models.TMDB
{
    public class TMDBTvRenameProviderMatchOptions : TMDBMovieTvRenameProviderMatchOptions
    {
        public string Season { get; set; }
        public string Episode { get; set; }

        public TMDBTvRenameProviderMatchOptions() : this(string.Empty, false, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty) { }
        public TMDBTvRenameProviderMatchOptions(string apiKey, bool includeAdult, string query, string year, string season, string episode, string language)
            : base(apiKey, includeAdult, query, year, language)
        {
            Season = season;
            Episode = episode;
        }
    }
}
