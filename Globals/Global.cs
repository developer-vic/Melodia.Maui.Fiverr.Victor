using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MelodiaTherapy.Globals
{
    public static class Global
    {
        public static string InternalPath = FileSystem.AppDataDirectory;
        public static string CurrentLanguage = "en";
        internal static bool DataReady;

        internal static void ReportNotTranslated(string code, string currentLanguage)
        { 
            // TODO: Simulate report data
        }
    }
}