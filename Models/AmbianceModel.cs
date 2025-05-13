using System.Text.Json.Serialization;
using System.Text.Json;

namespace MelodiaTherapy.Models
{
    public class AmbianceModel : AmbianceGridModel
    {
        [JsonPropertyName("isPremium")]
        public bool IsPremium { get; set; }

        [JsonIgnore]
        public int Icon
        {
            get
            {
                if (string.IsNullOrEmpty(IconCode)) return 0xf5dc;

                string hex = IconCode.StartsWith("0x") ? IconCode.Substring(2) : IconCode;
                return int.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out int result)
                    ? result
                    : 0xf5dc;
            }
        }

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

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

    }

}