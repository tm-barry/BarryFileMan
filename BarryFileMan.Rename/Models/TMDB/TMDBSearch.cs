using System.Text.Json.Serialization;

namespace BarryFileMan.Rename.Models.TMDB
{
    public class TMDBSearchBase<T>
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("results")]
        public IEnumerable<T> Results { get; set; } = Enumerable.Empty<T>();

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("total_results")]
        public int TotalResults { get; set; }
    }

    public class TMDBSearchMovieTV : TMDBSearchBase<TMDBSearchMovieTVResult> { }

    public class TMDBSearchCollection : TMDBSearchBase<TMDBSearchCollectionResult> { }

    public class TMDBSearchResultBase
    {
        [JsonPropertyName("adult")]
        public bool Adult { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string BackdropPath { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("original_language")]
        public string OriginalLanguage { get; set; } = string.Empty;

        [JsonPropertyName("original_name")]
        public string OriginalName { get; set; } = string.Empty;

        [JsonPropertyName("overview")]
        public string Overview { get; set; } = string.Empty;

        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; } = string.Empty;
    }

    public class TMDBSearchMovieTVResult : TMDBSearchResultBase
    {
        [JsonPropertyName("title")]
        public string Title { set { Name = value; } }

        [JsonPropertyName("original_title")]
        public string OriginalTitle { set { OriginalName = value; } }

        [JsonPropertyName("first_air_date")]
        public string FirstAirDate { set { ReleaseDate = value; } }

        [JsonPropertyName("genre_ids")]
        public IEnumerable<int> GenreIds { get; set; } = Enumerable.Empty<int>();

        [JsonPropertyName("origin_country")]
        public IEnumerable<string> OriginCountry { get; set; } = Enumerable.Empty<string>();

        [JsonPropertyName("popularity")]
        public double Popularity { get; set; }

        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; } = string.Empty;

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("vote_count")]
        public int VoteCount { get; set; }
    }

    public class TMDBSearchCollectionResult : TMDBSearchResultBase { }
}
