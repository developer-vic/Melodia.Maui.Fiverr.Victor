using MelodiaTherapy.Globals;
using MelodiaTherapy.Models;
using MelodiaTherapy.Pages;
using MelodiaTherapy.Services;
using Newtonsoft.Json;

namespace MelodiaTherapy.Controllers;


public class LanguageController
{
    private Dictionary<string, string> labels = new();
    private const string defaultText = "Votre langue...";
    public string SelectedLanguage { get; set; } = Global.CurrentLanguage;
    public bool TranslateMode { get; set; } = false;

    public async Task InitializeAsync()
    {
        await ChangeDisplayLanguageAsync(SelectedLanguage);
    }

    public async Task ChangeDisplayLanguageAsync(string language)
    {
        Global.DataReady = false;
        Preferences.Set("languageChoosen", false);

        await FetchDataToJsonAsync(language);
        await LoadDataFromJsonAsync();

        Preferences.Set("language", language);
        Global.CurrentLanguage = language;

         DataService.DownloadJsonsAsync();
         DataService.LoadJsonsAsync();

        Global.DataReady = true;
    }

    public async Task<string> GetLanguageAsync()
    {
        var lng = await LanguageService.GetLanguageCodeAsync();
        return Preferences.Get("language", lng ?? defaultText);
    }

    public async Task<bool> FetchDataToJsonAsync(string language)
    {
        try
        {
            var baseUrl = "https://server.melodiatherapy.com/api/mobile/";
            var argUrl = $"labels/{language}";
            var url = baseUrl + argUrl;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("ClientID", "ClientID");
            client.DefaultRequestHeaders.Add("ClientSecret", "ClientSecret");

            // GET request does not need Content-Type
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var filePath = Path.Combine(Global.InternalPath, "jsons", "labels.json");
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                await File.WriteAllTextAsync(filePath, json);
                Console.WriteLine($"{filePath} downloaded");
                return true;
            }
            else
            {
                throw new Exception($"Failed to download labels.json for {url} {response.StatusCode} : {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching data to json: {ex.Message}");
            return false;
        }
    }

    public async Task<List<dynamic>?> JsonToContentsAsync()
    {
        try
        {
            var filePath = Path.Combine(Global.InternalPath, "jsons", "labels.json");
            var contents = await File.ReadAllTextAsync(filePath);
            var jsonResponse = JsonConvert.DeserializeObject<List<dynamic>>(contents);
            return jsonResponse;
        }
        catch (Exception ex)
        {
            Console.WriteLine("loading labels from json error");
            Console.WriteLine(ex.ToString());
            return null;
        }
    }

    public async Task<bool> LoadDataFromJsonAsync()
    {
        var contents = await JsonToContentsAsync();
        if (contents != null)
        {
            var labelsList = contents.ConvertAll(c => LabelModel.FromJson(c));
            var items = new Dictionary<string, string>();
            foreach (var label in labelsList)
            {
                items[label.LabelId] = label.Translation;
            }
            labels = items;
            return true;
        }
        return false;
    }

    public string GetLabel(string code)
    {
        labels.TryGetValue(code, out var translation);
        if (translation == null)
        {
            Global.ReportNotTranslated(code, Global.CurrentLanguage);
        }
        return TranslateMode ? (translation != null ? "vvv" : "***") : translation ?? code;
    }

    public Task<bool> JsonExistsAsync()
    {
        var filePath = Path.Combine(Global.InternalPath, "jsons", "labels.json");
        return Task.FromResult(File.Exists(filePath));
    }

    public void NavigateNext(INavigation navigation)
    {
        if (SelectedLanguage == defaultText) return;
        navigation.PushAsync(new StartPage());
    }

    internal Dictionary<string, string> GetAllLabels()
    {
        return labels;
    }
}
