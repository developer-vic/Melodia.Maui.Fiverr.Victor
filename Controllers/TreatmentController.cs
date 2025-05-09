
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MelodiaTherapy.Models;


namespace MelodiaTherapy.Controllers
{
    public class TreatmentController : DataController
    {
        public List<TreatmentModel>? Treatments { get; private set; }
        public List<string>? SongGuids { get; private set; }

        public TreatmentController() : base(DataType.Treatments)
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
            string path = Path.Combine(App.InternalPath, "sounds", DataType.Treatments.ToString().ToLower(), fileName);
            Console.WriteLine($"//{path}");
            return $"//{path}";
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