using System.Text.Json.Serialization;

namespace BarryFileMan.Rename.Models.TMDB
{
    public class TMDBMovie : TMDBMovieTv
    {
        [JsonPropertyName("belongs_to_collection")]
        public TMDBSearchCollectionResult? BelongsToCollection { get; set; }

        [JsonPropertyName("budget")]
        public double Budget { get; set; }

        [JsonPropertyName("imdb_id")]
        public string IMDBId { get; set; } = string.Empty;

        [JsonPropertyName("revenue")]
        public int Revenue { get; set; }

        [JsonPropertyName("runtime")]
        public int Runtime { get; set; }

        [JsonPropertyName("video")]
        public bool Video { get; set; }
    }
}
