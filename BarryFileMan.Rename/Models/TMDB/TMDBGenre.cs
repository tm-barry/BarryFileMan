using System.Text.Json.Serialization;

namespace BarryFileMan.Rename.Models.TMDB
{
    public class TMDBGenre
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName ("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class TMDBGenreContainer
    {
        [JsonPropertyName("genres")]
        public IEnumerable<TMDBGenre> Genres { get; set; } = Enumerable.Empty<TMDBGenre>();
    }
}
