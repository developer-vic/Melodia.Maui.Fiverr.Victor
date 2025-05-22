using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MelodiaTherapy.Models;

namespace MelodiaTherapy.Helpers
{
    public static class Constants
    {
        public const double MobileWidth = 750.0;
        public const double WidthConstraint = 1000.0;

        // TO DO: add the entitlement ID from the RevenueCat dashboard that is activated upon successful in-app purchase.
        public const string EntitlementID = "Premium";

        // TO DO: add the Apple API key for your app from the RevenueCat dashboard: https://app.revenuecat.com
        public const string AppleApiKey = "appl_PtiyNhduxNiKfPWlVLFcRcxvjVT";

        // TO DO: add the Google API key for your app from the RevenueCat dashboard: https://app.revenuecat.com
        public const string GoogleApiKey = "goog_cQyjOgdIcgWEVgKiFWoHSKOnkda";

        // TO DO: add the Amazon API key for your app from the RevenueCat dashboard: https://app.revenuecat.com
        public const string AmazonApiKey = "";

        public const string PrivacyPolicyLink = "https://www.melodiatherapy.com/fr/privacy-fr/";
        public const string TermsAndConditionsLink = "https://www.melodiatherapy.com/fr/terms-of-service-fr/";
        public const string LegalNoticesLink = "https://www.melodiatherapy.com/fr/legal-notices-fr/";

        public static readonly TimeSpan InactivityDuration = TimeSpan.FromSeconds(3);
        // public static readonly TimeSpan InactivityDuration = TimeSpan.FromSeconds(10);

        public static int? TreatmentId;
        public static int? AmbianceId;
        public static int? ThemeId;
        public static int? ListenTypeId;
        public static int? ListenDurationId;
    }

}