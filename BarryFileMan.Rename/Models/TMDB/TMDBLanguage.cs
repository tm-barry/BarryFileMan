using System.Text.Json.Serialization;

namespace BarryFileMan.Rename.Models.TMDB
{
    public class TMDBLanguage
    {
        [JsonPropertyName("english_name")]
        public string EnglishName { get; set; } = string.Empty;

        [JsonPropertyName("iso_639_1")]
        public string IsoName { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
