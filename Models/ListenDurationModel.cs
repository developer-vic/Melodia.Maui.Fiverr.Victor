using System.Text.Json;
using System.Text.Json.Serialization;

namespace MelodiaTherapy.Models
{
    public class ListenDurationModel
    {
        [JsonPropertyName("length")]
        public int LengthInMinutes
        {
            get => (int)Duration.TotalMinutes;
            set => Duration = TimeSpan.FromMinutes(value);
        }

        [JsonIgnore]
        public TimeSpan Duration { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("isPremium")]
        public bool IsPremium { get; set; }

        public static ListenDurationModel FromJson(JsonElement json)
        {
            return new ListenDurationModel
            {
                LengthInMinutes = json.GetProperty("length").GetInt32(),
                Description = json.GetProperty("description").GetString(),
                IsPremium = json.GetProperty("isPremium").GetBoolean()
            };
        }

        public static List<ListenDurationModel> FromJsonList(string json)
        {
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.EnumerateArray().Select(FromJson).ToList();
        }

        public string ToJson() => JsonSerializer.Serialize(this);

        public static string ToJsonList(List<ListenDurationModel> list) =>
            JsonSerializer.Serialize(list);
    }

}