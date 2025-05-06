using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MelodiaTherapy.Globals
{
    public enum EnvironmentType
    {
        Development,
        Production
    }

    public static class Config
    {
        public static EnvironmentType Environment = EnvironmentType.Production;
        public static string Ip = "192.168.100.67";

        public static string ApiUrl =>
            Environment == EnvironmentType.Development
                ? $"https://{Ip}:52001/api/mobile/"
                : "https://server.melodiatherapy.com/api/mobile/";
    }

}