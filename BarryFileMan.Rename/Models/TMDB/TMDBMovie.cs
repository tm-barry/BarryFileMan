using System.Text.Json.Serialization;

namespace BarryFileMan.Rename.Models.TMDB
{
    public class TMDBMovie
    {
        [JsonPropertyName("adult")]
        public bool Adult { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string BackdropPath { get; set; } = string.Empty;

        [JsonPropertyName("belongs_to_collection")]
        public TMDBSearchCollectionResult? BelongsToCollection { get; set; }

        [JsonPropertyName("budget")]
        public double Budget { get; set; }

        [JsonPropertyName("genres")]
        public IEnumerable<TMDBGenre> Genres { get; set; } = Enumerable.Empty<TMDBGenre>();

        [JsonPropertyName("homepage")]
        public string Homepage { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("imdb_id")]
        public string IMDBId { get; set; } = string.Empty;

        [JsonPropertyName("original_language")]
        public string OriginalLanguage { get; set; } = string.Empty;

        [JsonPropertyName("original_title")]
        public string OriginalTitle { get; set; } = string.Empty;

        [JsonPropertyName("overview")]
        public string Overview { get; set; } = string.Empty;

        [JsonPropertyName("popularity")]
        public double Popularity { get; set; }

        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; } = string.Empty;

        [JsonPropertyName("production_companies")]
        public IEnumerable<TMDBProductionCompany> ProductionCompanies { get; set;} = Enumerable.Empty<TMDBProductionCompany>();

        [JsonPropertyName("production_countries")]
        public IEnumerable<TMDBCountry> ProductionCountries { get; set; } = Enumerable.Empty<TMDBCountry>();

        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; } = string.Empty;

        [JsonPropertyName("revenue")]
        public int Revenue { get; set; }

        [JsonPropertyName("runtime")]
        public int Runtime { get; set; }

        [JsonPropertyName("spoken_languages")]
        public IEnumerable<TMDBLanguage> Languages { get; set; } = Enumerable.Empty<TMDBLanguage>();

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("tagline")]
        public string Tagline {  get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("video")]
        public bool Video { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("vote_count")]
        public int VoteCount { get; set; }
    }
}
