using System.Text.Json.Serialization;

namespace BarryFileMan.Rename.Models.TMDB
{
    public class TMDBCountry
    {
        [JsonPropertyName("iso_3166_1")]
        public string IsoName { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
