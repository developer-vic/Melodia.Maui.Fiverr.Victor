using System.Text.Json.Serialization;
using System.Text.Json;

namespace MelodiaTherapy.Models
{
    public class AmbianceModel : AmbianceGridModel
    {
        [JsonPropertyName("isPremium")]
        public bool IsPremium { get; set; }

        [JsonPropertyName("icon")]
        public string Icon => IconCode ?? "music.png";

        public AmbianceModel()
        {
            Name = Name?.ToUpperInvariant();
        }

        public static List<AmbianceModel>? FromJson(string json)
        {
            return JsonSerializer.Deserialize<List<AmbianceModel>>(json);
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static string ToJsonList(List<AmbianceModel> list)
        {
            return JsonSerializer.Serialize(list);
        }
    }
    public class AmbianceGridModel
    {
        [JsonPropertyName("guid")]
        public string? Guid { get; set; }

        [JsonPropertyName("songGuid")]
        public string? SongGuid { get; set; }

        [JsonPropertyName("iconCode")]
        public string? IconCode { get; set; }

        [JsonIgnore]
        public int? IconCodeInt => int.TryParse(IconCode, out var code) ? code : null;

        [JsonPropertyName("name")]
        public string? Name { get; set; }
        
    }

}