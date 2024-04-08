using System.Text.Json.Serialization;

namespace BarryFileMan.Rename.Models.TMDB
{
    public class TMDBConfiguration
    {
        [JsonPropertyName("images")]
        public TMDBConfigurationImages Images { get; set; } = new();

        [JsonPropertyName("change_keys")]
        public IEnumerable<string> ChangeKeys { get; set; } = Enumerable.Empty<string>();
    }

    public class TMDBConfigurationImages
    {
        [JsonPropertyName("base_url")]
        public string BaseUrl { get; set; } = string.Empty;

        [JsonPropertyName("secure_base_url")]
        public string SecureBaseUrl { get; set; } = string.Empty;

        [JsonPropertyName("backdrop_sizes")]
        public IEnumerable<string> BackdropSizes { get; set; } = Enumerable.Empty<string>();

        [JsonPropertyName("logo_sizes")]
        public IEnumerable<string> LogoSizes { get; set; } = Enumerable.Empty<string>();

        [JsonPropertyName("poster_sizes")]
        public IEnumerable<string> PosterSizes { get; set; } = Enumerable.Empty<string>();

        [JsonPropertyName("profile_sizes")]
        public IEnumerable<string> ProfileSizes { get; set; } = Enumerable.Empty<string>();

        [JsonPropertyName("still_sizes")]
        public IEnumerable<string> StillSizes { get; set; } = Enumerable.Empty<string>();
    }
}
