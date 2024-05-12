using System.Text.Json.Serialization;

namespace BarryFileMan.Rename.Models.TMDB
{
    public class TMDBTvEpisode
    {
        [JsonPropertyName("air_date")]
        public string AirDate { get; set; } = string.Empty;

        [JsonPropertyName("crew")]
        public IEnumerable<TMDBPerson> Crew { get; set; } = Enumerable.Empty<TMDBPerson>();

        [JsonPropertyName("episode_number")]
        public int EpisodeNumber { get; set; }

        [JsonPropertyName("guest_stars")]
        public IEnumerable<TMDBPerson> GuestStars { get; set; } = Enumerable.Empty<TMDBPerson>();

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("overview")]
        public string Overview { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("production_code")]
        public string ProductionCode { get; set; } = string.Empty;

        [JsonPropertyName("runtime")]
        public int Runtime { get; set; }

        [JsonPropertyName("season_number")]
        public int SeasonNumber { get; set; }

        [JsonPropertyName("still_path")]
        public string StillPath { get; set; } = string.Empty;

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("vote_count")]
        public int VoteCount { get; set; }

        [JsonPropertyName("show_id")]
        public int? ShowId { get; set; }
    }
}
