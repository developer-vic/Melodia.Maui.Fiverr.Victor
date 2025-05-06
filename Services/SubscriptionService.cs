using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using MelodiaTherapy.Globals;
using MelodiaTherapy.Helpers;
using MelodiaTherapy.Models;

namespace MelodiaTherapy.Services;

public class SubscriptionService
{ 
    public static async Task<List<SubscriptionModel>> GetSubscriptionsAsync(string? promoCode = null)
    {
        string baseeUrl = Config.ApiUrl;
        string baseUrl = $"{baseeUrl}Subscriptions";
        string culture = "fr"; // default
        string deviceId = AppData.UniqueId ?? "";

        try
        {
            // Assuming you have a method to get the saved language, e.g., using Preferences from MAUI Essentials
            if (Preferences.ContainsKey("language"))
                culture = Preferences.Get("language", "fr");

            Console.WriteLine($"culture: {culture}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        string url = $"{baseUrl}/{culture}/{deviceId}/{promoCode ?? ""}";
        Console.WriteLine(url);

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.Add("ClientID", "ClientID");
        request.Headers.Add("ClientSecret", "ClientSecret");

        var response = await new HttpClient().SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var subscriptions = JsonSerializer.Deserialize<List<SubscriptionModel>>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return subscriptions ?? new List<SubscriptionModel>();
        }
        else
        {
            throw new Exception("Failed to load subscriptions");
        }
    }
}
