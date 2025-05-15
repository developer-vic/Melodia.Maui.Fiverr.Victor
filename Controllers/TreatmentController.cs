using System.Text.Json; 
using MelodiaTherapy.Enums;
using MelodiaTherapy.Models;
using MelodiaTherapy.Services;
using MelodiaTherapy.Globals;


namespace MelodiaTherapy.Controllers
{
    public class TreatmentController : DataController
    {
        public List<TreatmentModel>? Treatments { get; set; }
        public List<string>? SongGuids { get; private set; }

        public TreatmentController() : base(DataType.Treatments)
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
                Treatments = JsonSerializer.Deserialize<List<TreatmentModel>>(contents) ?? new List<TreatmentModel>();

                // Add songGuids
                SongGuids = new List<string>();
                foreach (var treatment in Treatments)
                {
                    foreach (var url in treatment.TreatmentUrls)
                    {
                        if (!string.IsNullOrEmpty(url.SongGuid)) //&& !SongGuids.Contains(url.SongGuid)
                            SongGuids.Add(url.SongGuid);
                    }
                }

                Treatments.Add(DefaultTreatmentModel);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading {Type}.json: {ex.Message}");
                return false;
            }
        }

        // retourne le chemin du son en local
        public static string GetLocalSongPath(TreatmentModel treatment, ListenTypeModel listenMode, TimeSpan duration)
        {
            string ext = ".mp3";
            var songGuid = treatment.TreatmentUrls.FirstOrDefault(e => e.ListenTypeGuidId == listenMode.Guid)?.SongGuid;
            string fileName = $"{songGuid}_{(int)duration.TotalMinutes}{ext}";
            string path = Path.Combine(Config.InternalPath, "sounds", DataType.Treatments.ToString().ToLower(), fileName);
            Console.WriteLine($"//{path}");
            return $"//{path}";
        }

        internal async Task<List<TreatmentModel>> LoadDemoTreatments()
        {
            var localTreatments = await MobileServices.GetData<MobileTreatmentVM>("treatments.json");
            Treatments = localTreatments
                .Select(t => new TreatmentModel()
                {
                    Description = t.Description,
                    Guid = t.Guid.ToString(),
                    IconCode = t.IconCode,
                    IsPremium = t.IsPremium,
                    Name = t.Name,
                    TreatmentUrls = t.TreatmentUrls.Select(turl => new TreatmentUrl()
                    {
                        ListenTypeGuidId = turl.ListenTypeGuidID.ToString(),
                        SongGuid = turl.SongGuid.ToString()
                    }).ToList()
                }).ToList();
            return Treatments;
        }

        public static TreatmentModel DefaultTreatmentModel => new TreatmentModel
        {
            Guid = string.Empty,
            IsPremium = false,
            IconCode = "Ban",
            Name = "Aucun",
            Description = string.Empty,
            TreatmentUrls = new List<TreatmentUrl>()
        };
    }

}