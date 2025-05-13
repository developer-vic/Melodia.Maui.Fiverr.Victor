using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace MelodiaTherapy.Models
{
    public class ListenTypeModel : ListenTypeGridModel
    {
        [JsonIgnore]
        public int Icon
        {
            get
            {
                if (string.IsNullOrEmpty(IconCode)) return GetFallbackIcon();

                string hex = IconCode.StartsWith("0x") ? IconCode.Substring(2) : IconCode;
                return int.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out int result)
                    ? result : GetFallbackIcon();
            }
        }

        public static ListenTypeModel FromJson(JsonElement json)
        {
            var iconCode = json.GetProperty("iconCode").GetString();
            int.TryParse(iconCode, out int codePoint);

            return new ListenTypeModel
            {
                Guid = json.GetProperty("guid").GetString(),
                IconCode = iconCode,
                Name = json.GetProperty("name").GetString(),
                Description = json.GetProperty("description").GetString(),
                // Icon is dynamically resolved
            };
        }

        public static List<ListenTypeModel> FromJsonList(string json)
        {
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.EnumerateArray().Select(FromJson).ToList();
        }

        public string ToJson() => JsonSerializer.Serialize(this);

        public static string ToJsonList(List<ListenTypeModel> list) =>
            JsonSerializer.Serialize(list);

        private int GetFallbackIcon()
        {
            return IconCode == "fa-solid fa-headphones" ? 0xf025 : 0xf028;
        }
    }

    public class ListenTypeGridModel
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