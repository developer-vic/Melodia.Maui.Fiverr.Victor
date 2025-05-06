using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MelodiaTherapy.Helpers
{
    public class AppData
    {
        private static readonly AppData _instance = new AppData();

        // Public fields (you can use properties if preferred)
        public static string ProfessionalDeviceName { get; set; } = string.Empty;
        public static bool IsPremiumLifetime { get; set; } = false;
        public static bool? IsFreeTrial { get; set; } = false;
        public static int PeriodFreeTrial { get; set; } = 0;
        public static bool IsPremium { get; set; } = false;
        public static string? SubscriptionId { get; set; }
        public static bool EntitlementIsActive { get; set; } = false;
        public static string? SubscriptionCode { get; set; }
        public static string AppUserID { get; set; } = string.Empty;
        public static DateTime? CreatedOn { get; set; }
        public static string? ExpirationDate { get; set; }
        public static string? OriginalPurchaseDate { get; set; }
        public static string? UniqueId { get; set; }
        public static string? Version { get; set; }
        public static string? BuildNumber { get; set; }

        // Private constructor
        private AppData() { }

        // Singleton instance
        public static AppData Instance => _instance;
    }
}