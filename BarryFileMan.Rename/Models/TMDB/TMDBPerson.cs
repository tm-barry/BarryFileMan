using System.Text.Json.Serialization;

namespace BarryFileMan.Rename.Models.TMDB
{
    public class TMDBPerson
    {
        [JsonPropertyName("character")]
        public string Character { get; set; } = string.Empty;

        [JsonPropertyName("order")]
        public int Order { get; set; }

        [JsonPropertyName("job")]
        public string Job { get; set; } = string.Empty;

        [JsonPropertyName("department")]
        public string Department { get; set; } = string.Empty;

        [JsonPropertyName("credit_id")]
        public string CreditId { get; set; } = string.Empty;

        [JsonPropertyName("adult")]
        public bool Adult { get; set; }

        [JsonPropertyName("gender")]
        public int Gender { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("known_for_department")]
        public string KnownForDepartment { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("original_name")]
        public string OriginalName { get; set; } = string.Empty;

        [JsonPropertyName("popularity")]
        public double Popularity { get; set; }

        [JsonPropertyName("profile_path")]
        public string ProfilePath { get; set; } = string.Empty;
    }
}
