using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MelodiaTherapy.Enums;
using MelodiaTherapy.Globals;
using MelodiaTherapy.Models;
using MelodiaTherapy.Services;
using Microsoft.Maui.Controls;

namespace MelodiaTherapy.Controllers
{
    public class AmbianceController : DataController
    {
        public List<AmbianceModel>? Ambiances { get; set; }
        public List<string>? SongGuids { get; private set; }

        public AmbianceController() : base(DataType.Ambiances)
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
                Ambiances = JsonSerializer.Deserialize<List<AmbianceModel>>(contents) ?? new List<AmbianceModel>();
                SongGuids = Ambiances.Select(a => a.SongGuid ?? string.Empty).ToList();
                Ambiances.Add(DefaultAmbianceModel);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading {Type}.json: {ex.Message}");
                return false;
            }
        }

        // retourne le chemin du son en local
        public static string GetLocalSongPath(AmbianceModel ambiance, TimeSpan duration)
        {
            string ext = ".mp3";
            string fileName = $"{ambiance.SongGuid}_{(int)duration.TotalMinutes}{ext}";
            return Path.Combine(Config.InternalPath, "sounds", DataType.Ambiances.ToString().ToLower(), fileName);
        }

        internal async Task<List<AmbianceModel>> LoadDemoAmbiances()
        {
            var localTreatments = await MobileServices.GetData<MobileAmbianceVM>("ambiances.json");
            Ambiances = localTreatments
                .Select(t => new AmbianceModel()
                {
                    Description = t.Description,
                    Guid = t.Guid.ToString(),
                    IconCode = t.IconCode,
                    IsPremium = t.IsPremium,
                    Name = t.Name,
                    SongGuid = t.SongGuid.ToString()
                }).ToList();
            return Ambiances;
        }

        public static AmbianceModel DefaultAmbianceModel => new AmbianceModel
        {
            Guid = string.Empty,
            SongGuid = string.Empty,
            IconCode = "Ban",
            Name = "Aucune",
            IsPremium = false
        };
    }
}