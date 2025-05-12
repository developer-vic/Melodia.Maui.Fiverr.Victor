using System.Text.Json;
using System.Text.Json.Serialization;

namespace MelodiaTherapy.Models
{
    public class TreatmentModel : TreatmentGridModel
    {
        [JsonPropertyName("isPremium")]
        public bool IsPremium { get; set; }

        [JsonPropertyName("treatmentUrls")]
        public List<TreatmentUrl> TreatmentUrls { get; set; } = new();

        [JsonPropertyName("icon")]
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


        public TreatmentModel()
        {
            Name = Name?.ToUpperInvariant();
        }

        public static List<TreatmentModel>? FromJson(string json)
        {
            return JsonSerializer.Deserialize<List<TreatmentModel>>(json);
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static string ToJsonList(List<TreatmentModel> list)
        {
            return JsonSerializer.Serialize(list);
        }
    }

    public class TreatmentUrl
    {
        [JsonPropertyName("listenTypeGuidID")]
        public string? ListenTypeGuidId { get; set; }

        [JsonPropertyName("songGuid")]
        public string? SongGuid { get; set; }
    }

    public class TreatmentGridModel
    {
        [JsonPropertyName("guid")]
        public string? Guid { get; set; }

        [JsonPropertyName("iconCode")]
        public string? IconCode { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}