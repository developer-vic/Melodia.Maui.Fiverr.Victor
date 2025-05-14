
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MelodiaTherapy.Models;
using MelodiaTherapy.Controllers;
using Microsoft.Maui.Controls;
using MelodiaTherapy.Services;

namespace MelodiaTherapy.Controllers
{

    public class ThemeController : DataController
    {
        public List<ThemeModel> Themes { get; set; } = new();
        public List<string> SongGuids { get; private set; } = new();

        public ThemeController() : base(DataType.Themes) { }

        public async Task<bool> LoadDataFromJsonAsync()
        {
            try
            {
                string path = Path.Combine(App.InternalPath, "jsons", $"{Type.ToString().ToLower()}.json");

                if (!File.Exists(path))
                    return false;

                string contents = await File.ReadAllTextAsync(path);
                var list = ThemeModel.FromJsonList(contents);

                Themes = list;
                SongGuids = [.. list.Select(x => x.SongGuid)];
                Themes.Add(DefaultThemeModel);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading {Type} from JSON: {ex.Message}");
                return false;
            }
        }

        public static string GetLocalImagePath(ThemeModel theme)
        {
            return Path.Combine("//" + App.InternalPath, "images", DataType.Themes.ToString().ToLower(), $"{theme.Guid}.jpg");
        }

        public static string GetLocalSongPath(ThemeModel theme, TimeSpan duration)
        {
            string fileName = $"{theme.SongGuid}_{(int)duration.TotalMinutes}.mp3";
            return Path.Combine("//" + App.InternalPath, "sounds", DataType.Themes.ToString().ToLower(), fileName);
        }

        internal async Task<List<ThemeModel>> LoadDemoThemes()
        {
            var localThemes = await MobileServices.GetData<MobileThemeVM>("themes.json");
            Themes = localThemes.Select(t => new ThemeModel()
            {
                Guid = t.Guid.ToString(),
                ImageUrl = t.ImageUrl,
                IsPremium = t.IsPremium,
                Name = t.Name,
                SongGuid = t.SongGuid.ToString()
            }).ToList();
            return Themes;
        }

        public static ThemeModel DefaultThemeModel => new()
        {
            Guid = "",
            SongGuid = "",
            IsPremium = false,
            ImageUrl = "https://static.melodiatherapy.com/Themes/002 Les dieux de l ether.jpg",
            Name = "AUCUN"
        };
    }

}