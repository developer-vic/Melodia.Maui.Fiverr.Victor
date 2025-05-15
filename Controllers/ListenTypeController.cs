using MelodiaTherapy.Models;
using MelodiaTherapy.Services;
using MelodiaTherapy.Globals;
using MelodiaTherapy.Enums;
using System.Text.Json;

namespace MelodiaTherapy.Controllers
{

    public class ListenTypeController : DataController
    {
        public List<ListenTypeModel>? ListenTypes { get; set; }

        public ListenTypeController() : base(DataType.ListenTypes)
        {
        }

        public async Task<bool> LoadDataFromJsonAsync()
        {
            string filePath = Path.Combine(Config.InternalPath, "jsons", $"{Type.ToString().ToLower()}.json");

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return false;
            }

            try
            {
                string contents = await File.ReadAllTextAsync(filePath);
                ListenTypes = JsonSerializer.Deserialize<List<ListenTypeModel>>(contents) ?? new List<ListenTypeModel>();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading {Type}.json: {ex.Message}");
                return false;
            }
        }

        internal async Task<List<ListenTypeModel>> LoadDemoListenings()
        {
            var localListenTypes = await MobileServices.GetData<MobileListenTypeVM>("listentypes.json");
            ListenTypes = localListenTypes.Select(t => new ListenTypeModel()
            {
                Description = t.Description,
                Guid = t.Guid.ToString(),
                IconCode = t.IconCode,
                Name = t.Name,
            }).ToList();
            return ListenTypes;
        }

        public static ListenTypeModel DefaultListenTypeModel => new ListenTypeModel
        {
            Guid = string.Empty,
            IconCode = string.Empty,
            Name = string.Empty,
            Description = string.Empty
        };
    }

}