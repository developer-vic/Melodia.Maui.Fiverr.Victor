using MelodiaTherapy.Models;
using System.Text.Json;

namespace MelodiaTherapy.Services
{
    public static class MobileServices
    {
        public async static Task<List<T>> GetData<T>(string Filename)
        {
            string Json = await ReadFile(Filename);
            List<T> Data = JsonSerializer.Deserialize<List<T>>(Json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return Data;
        }

        public async static Task<string> ReadFile(string Filename)
        {
            using Stream fileStream = await FileSystem.OpenAppPackageFileAsync(Filename);
            using StreamReader reader = new StreamReader(fileStream);
            string Json = await reader.ReadToEndAsync();

            return Json;
        }
    }
}
