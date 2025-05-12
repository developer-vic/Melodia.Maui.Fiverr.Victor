using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MelodiaTherapy.Models
{
    public class GridModel
    { 
        public string? Guid { get; set; }
        public string? SongGuid { get; set; }
        public bool IsPremium { get; set; }
        public string? IconCode { get; set; }
        public string? Icon { get; set; } // To hold icon name or code, as MAUI doesn't directly use IconData
        public string? Name { get; set; }
        public string? Description { get; set; }

        public GridModel(
            string? guid,
            string? songGuid,
            bool? isPremium,
            string? iconCode,
            string? icon, // MAUI uses string for icon codes, like FontAwesome, Material Icons, etc.
            string? name,
            string? description)
        {
            Guid = guid;
            SongGuid = songGuid;
            IsPremium = isPremium ?? false;
            IconCode = iconCode;
            Icon = icon;
            Name = name;
            Description = description;
        }
    }
}