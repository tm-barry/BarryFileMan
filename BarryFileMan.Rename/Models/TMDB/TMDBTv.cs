using System.Text.Json.Serialization;

namespace BarryFileMan.Rename.Models.TMDB
{
    public class TMDBTv : TMDBMovieTv
    {
        [JsonPropertyName("created_by")]
        public IEnumerable<TMDBPerson> CreatedBy { get; set; } = Enumerable.Empty<TMDBPerson>();

        [JsonPropertyName("episode_run_time")]
        public IEnumerable<int> EpisodeRunTime { get; set; } = Enumerable.Empty<int>();

        [JsonPropertyName("languages")]
        public IEnumerable<string> Languages { get; set; } = Enumerable.Empty<string>();

        [JsonPropertyName("last_air_date")]
        public string LastAirDate { get; set; } = string.Empty;

        [JsonPropertyName("last_episode_to_air")]
        public TMDBTvEpisode? LastEpisodeToAir { get; set; }

        [JsonPropertyName("next_episode_to_air")]
        public string NextEpisodeToAir { get; set; } = string.Empty;

        [JsonPropertyName("networks")]
        public IEnumerable<TMDBProductionCompany> Networks { get; set; } = Enumerable.Empty<TMDBProductionCompany>();

        [JsonPropertyName("number_of_episodes")]
        public int NumberOfEpisodes { get; set; }

        [JsonPropertyName("number_of_seasons")]
        public int NumberOfSeasons { get; set; }

        [JsonPropertyName("origin_country")]
        public IEnumerable<string> OriginCountry { get; set; } = Enumerable.Empty<string>();

        [JsonPropertyName("seasons")]
        public IEnumerable<TMDBTvSeason> Seasons { get; set; } = Enumerable.Empty<TMDBTvSeason>();

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
    }
}
