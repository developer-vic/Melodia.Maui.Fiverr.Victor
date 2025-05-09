using System.Text.Json;
using System.Text.Json.Serialization;

namespace MelodiaTherapy.Models
{
    public class ThemeModel
    {
        [JsonPropertyName("guid")]
        public string? Guid { get; set; }

        [JsonPropertyName("songGuid")]
        public string? SongGuid { get; set; }

        [JsonPropertyName("isPremium")]
        public bool IsPremium { get; set; }

        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }

        private string? _name;

        [JsonPropertyName("name")]
        public string? Name
        {
            get => _name;
            set => _name = value?.ToUpper();
        }

        public static ThemeModel FromJson(JsonElement json)
        {
            return new ThemeModel
            {
                Guid = json.GetProperty("guid").GetString(),
                SongGuid = json.GetProperty("songGuid").GetString(),
                IsPremium = json.GetProperty("isPremium").GetBoolean(),
                ImageUrl = json.GetProperty("imageUrl").GetString(),
                Name = json.GetProperty("name").GetString(),
            };
        }

        public static List<ThemeModel> FromJsonList(string json)
        {
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.EnumerateArray().Select(FromJson).ToList();
        }

        public string ToJson() => JsonSerializer.Serialize(this);

        public static string ToJsonList(List<ThemeModel> list) =>
            JsonSerializer.Serialize(list);
    }
}