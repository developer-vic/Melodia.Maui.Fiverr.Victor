using System.Text.Json;
using MelodiaTherapy.Models;

namespace MelodiaTherapy.Controllers
{
    public class ListenDurationController : DataController
    {
        public List<ListenDurationModel>? ListenDurations { get; private set; }

        public ListenDurationController() : base(DataType.DurationsNew)
        {
        }

        public async Task<bool> LoadDataFromJsonAsync()
        {
            string filePath = Path.Combine(App.InternalPath, "jsons", $"{Type.ToString().ToLower()}.json");

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return false;
            }

            try
            {
                string contents = await File.ReadAllTextAsync(filePath);
                ListenDurations = JsonSerializer.Deserialize<List<ListenDurationModel>>(contents) ?? new List<ListenDurationModel>();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading {Type}.json: {ex.Message}");
                return false;
            }
        }

        public static ListenDurationModel DefaultListenDurationModel => new ListenDurationModel
        {
            Duration = TimeSpan.FromMinutes(15),
            Description = "default",
            IsPremium = false
        };
    }
}