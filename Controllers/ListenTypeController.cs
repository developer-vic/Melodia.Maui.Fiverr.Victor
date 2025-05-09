
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using MelodiaTherapy.Models;

namespace MelodiaTherapy.Controllers
{

    public class ListenTypeController : DataController
    {
        public List<ListenTypeModel>? ListenTypes { get; private set; }

        public ListenTypeController() : base(DataType.ListenTypes)
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
                ListenTypes = JsonSerializer.Deserialize<List<ListenTypeModel>>(contents) ?? new List<ListenTypeModel>();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading {Type}.json: {ex.Message}");
                return false;
            }
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