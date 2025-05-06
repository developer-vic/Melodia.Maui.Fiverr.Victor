using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MelodiaTherapy.Globals;
using MelodiaTherapy.Models;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;


namespace MelodiaTherapy.Services
{
    public class LanguageService
    {
        public static string? CurrentLanguage = "en"; // TODO: Implement logic to get the current language
        private static string? _defaultLocale;
        public static Dictionary<string, string> IsoLangs = new();

        private static async Task GetDefaultLocaleAsync()
        {
            try
            {
                _defaultLocale = System.Globalization.CultureInfo.CurrentCulture.Name;
                Console.WriteLine($"defaultLocale: {_defaultLocale}");
            }
            catch (Exception)
            {
                Console.WriteLine("Error obtaining default locale");
            }
        }

        public static async Task<string?> GetLanguageCodeAsync()
        {
            await GetDefaultLocaleAsync();
            return _defaultLocale?.Split('-')[0];
        }

        public static string GetNativeLanguage(string lngCode)
        {
            return IsoLangs.TryGetValue(lngCode, out var native) ? native : lngCode;
        }

        public static async Task InitDataAsync()
        {
            await FetchLanguagesJsonAsync();
            await LoadDataFromJsonAsync();
        }

        private static async Task<Dictionary<string, object>?> JsonToContentsAsync()
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("isoLang.json");
                using var reader = new StreamReader(stream);
                var contents = await reader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(contents);
            }
            catch (Exception e)
            {
                Console.WriteLine("loading isoLang from json error");
                Console.WriteLine(e);
                return null;
            }
        }

        private static async Task<bool> LoadDataFromJsonAsync()
        {
            var contents = await JsonToContentsAsync();
            if (contents != null)
            {
                var json = JsonConvert.SerializeObject(contents);
                var isoLangRoot = JsonConvert.DeserializeObject<IsoLangRoot>(json);
                if (isoLangRoot?.Items != null)
                {
                    IsoLangs = isoLangRoot.Items;
                    return true;
                }
            }
            return false;
        }

        public static List<string> AllLanguagesCodes = new();

        public static async Task<bool> FetchLanguagesJsonAsync()
        {
            string url = $"{Config.ApiUrl}languages";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("ClientID", "ClientID");
            client.DefaultRequestHeaders.Add("ClientSecret", "ClientSecret");

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);

                var languagesList = new List<string>();
                foreach (var element in items)
                {
                    if (element.TryGetValue("code", out var code))
                    {
                        languagesList.Add(code);
                    }
                }

                Preferences.Set("languages", string.Join(",", languagesList));
                AllLanguagesCodes = languagesList;
                return true;
            }
            else
            {
                throw new Exception("Failed to load languages");
            }
        }

        public static async Task ReportNotTranslatedAsync(string text, string language)
        {
            // Optional local logging (disabled as in original code)
            /*
            var path = Path.Combine(FileSystem.AppDataDirectory, "notTranslated.txt");
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }

            var content = await File.ReadAllTextAsync(path);
            if (!content.Contains(text + Environment.NewLine))
            {
                await File.AppendAllTextAsync(path, text + Environment.NewLine);
            }
            */
        }

    }
}