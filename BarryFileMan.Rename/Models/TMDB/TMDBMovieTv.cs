using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace BarryFileMan.Rename.Models.TMDB
{
    public class TMDBMovieTv
    {
        [JsonPropertyName("adult")]
        public bool Adult { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string BackdropPath { get; set; } = string.Empty;

        [JsonPropertyName("first_air_date")]
        public string FirstAirDate { set { ReleaseDate = value; } }

        [JsonPropertyName("genres")]
        public IEnumerable<TMDBGenre> Genres { get; set; } = Enumerable.Empty<TMDBGenre>();

        [JsonPropertyName("homepage")]
        public string Homepage { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("original_language")]
        public string OriginalLanguage { get; set; } = string.Empty;

        [JsonPropertyName("original_name")]
        public string OriginalName { get; set; } = string.Empty;

        [JsonPropertyName("original_title")]
        public string OriginalTitle { set { OriginalName = value; } }

        [JsonPropertyName("overview")]
        public string Overview { get; set; } = string.Empty;

        [JsonPropertyName("popularity")]
        public double Popularity { get; set; }

        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; } = string.Empty;

        [JsonPropertyName("production_companies")]
        public IEnumerable<TMDBProductionCompany> ProductionCompanies { get; set; } = Enumerable.Empty<TMDBProductionCompany>();

        [JsonPropertyName("production_countries")]
        public IEnumerable<TMDBCountry> ProductionCountries { get; set; } = Enumerable.Empty<TMDBCountry>();

        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; } = string.Empty;

        [JsonPropertyName("spoken_languages")]
        public IEnumerable<TMDBLanguage> SpokenLanguages { get; set; } = Enumerable.Empty<TMDBLanguage>();

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("tagline")]
        public string Tagline { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { set { Name = value; } }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("vote_count")]
        public int VoteCount { get; set; }
    }
}
