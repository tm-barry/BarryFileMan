using System.Text.Json.Serialization;

namespace BarryFileMan.Rename.Models.TMDB
{
    public class TMDBResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; } = true;

        [JsonPropertyName("status_code")]
        public int StatusCode { get; set; }

        [JsonPropertyName("status_message")]
        public string StatusMessage { get; set; } = string.Empty;
    }
}
