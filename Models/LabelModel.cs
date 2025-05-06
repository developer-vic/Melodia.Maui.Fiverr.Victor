using System.Collections.Generic;
using System.Text.Json;

namespace MelodiaTherapy.Models
{
    public class LabelModel
    {
        public string LabelId { get; set; } = "";
        public string Translation { get; set; } = "";

        public static List<LabelModel> FromJson(string jsonStr)
        {
            var jsonArray = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(jsonStr);
            var result = new List<LabelModel>();

            if (jsonArray != null)
            {
                foreach (var item in jsonArray)
                {
                    result.Add(new LabelModel
                    {
                        LabelId = item.ContainsKey("key") ? item["key"] ?? "" : "",
                        Translation = item.ContainsKey("text") ? item["text"] ?? "" : ""
                    });
                }
            }

            return result;
        }
    }
}