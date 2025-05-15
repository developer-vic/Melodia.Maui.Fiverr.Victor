using MelodiaTherapy.Enums;
using MelodiaTherapy.Globals;
using MelodiaTherapy.Helpers;
using MelodiaTherapy.Models;
using MelodiaTherapy.Services; 

namespace MelodiaTherapy.Controllers
{
    public class DataController
    {
        protected readonly string BaseUrl = Config.ApiUrl;
        protected readonly Dictionary<string, string> RequestHeaders = new()
    {
        { "Content-Type", "application/json" },
        { "ClientID", "ClientID" },
        { "ClientSecret", "ClientSecret" }
    };

        public DataType Type { get; private set; }

        public DataController(DataType type = DataType.Data)
        {
            Type = type;
        }

        public async Task<bool> FetchDataToJsonAsync(string language)
        {
            try
            {
                string argUrl = $"/{language}";
                string deviceId = $"/{AppData.UniqueId}";
                string url = $"{BaseUrl}{Type.ToString().ToLower()}{argUrl}{deviceId}";

                using var httpClient = new HttpClient();
                foreach (var header in RequestHeaders)
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);

                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string path = Path.Combine(Config.InternalPath, "jsons", $"{Type.ToString().ToLower()}.json");
                    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                    await File.WriteAllTextAsync(path, await response.Content.ReadAsStringAsync());

                    Console.WriteLine($"{path} downloaded");
                    return true;
                }
                else
                {
                    throw new Exception($"Failed to download {Type}.json");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> DownloadImagesAsync(List<ThemeModel> items)
        {
            if (items == null || items.Count == 0)
            {
                Console.WriteLine("No images to process");
                return false;
            }

            var listImages = new Dictionary<string, string>();
            string ext = "jpg";

            foreach (var theme in items)
            {
                if (theme != null && !string.IsNullOrEmpty(theme.ImageUrl)
                    && !string.IsNullOrEmpty(theme.Guid))
                {
                    var imageUrl = theme.ImageUrl.Replace($".{ext}", $".small.{ext}").Replace(" ", "%20");
                    string filePath = Path.Combine(Config.InternalPath, "images", Type.ToString().ToLower(), $"{theme.Guid}.{ext}");

                    if (!File.Exists(filePath))
                        listImages[theme.Guid] = imageUrl;
                }
            }

            Console.WriteLine($"{listImages.Count} image(s) to download, wait please");

            foreach (var entry in listImages)
            {
                await DownloadFileAsync(entry.Value, entry.Key, $"images/{Type.ToString().ToLower()}", ext);
            }

            return true;
        }

        public static async Task<bool> DownloadFileAsync(string url, string fileName, string directory, string ext)
        {
            InternetService.NeedConnection = true;

            while (!await InternetService.IsConnectedAsync())
            {
                await Task.Delay(500);
            }

            InternetService.NeedConnection = false;

            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return false;

                byte[] data = await response.Content.ReadAsByteArrayAsync();

                string filePath = Path.Combine(Config.InternalPath, directory, $"{fileName}.{ext}");
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                await File.WriteAllBytesAsync(filePath, data);

                Console.WriteLine($"DOWNLOADED: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading file {directory}/{fileName}.{ext}: {ex.Message}");
                return false;
            }
        }

        public bool JsonExistsAsync()
        {
            string path = Path.Combine(Config.InternalPath, "jsons", $"{Type.ToString().ToLower()}.json");
            return File.Exists(path);
        }
    }
}