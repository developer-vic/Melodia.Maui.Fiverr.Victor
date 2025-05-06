using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MelodiaTherapy.Services
{
    public class InternetService
    {
        public static async Task<bool> IsConnectedToInternetAsync()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return false;

            try
            {
                var hostEntry = await Dns.GetHostEntryAsync("google.com");
                return hostEntry.AddressList.Any(ip => ip.AddressFamily == AddressFamily.InterNetwork || ip.AddressFamily == AddressFamily.InterNetworkV6);
            }
            catch
            {
                return false;
            }
        }

        public static Task<bool> IsConnectedToWifiAsync()
        {
            try
            {
                return Task.FromResult(NetworkInterface.GetAllNetworkInterfaces()
                    .Any(ni =>
                        ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 &&
                        ni.OperationalStatus == OperationalStatus.Up));
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                Console.WriteLine($"Error checking WiFi connection: {ex.Message}");
            }
            return Task.FromResult(false);
        }

        public static Task<bool> IsConnectedToMobileDataAsync()
        {
            try
            {
                return Task.FromResult(NetworkInterface.GetAllNetworkInterfaces()
                    .Any(ni =>
                        ni.NetworkInterfaceType == NetworkInterfaceType.Wwanpp ||
                        ni.NetworkInterfaceType == NetworkInterfaceType.Wwanpp2 &&
                        ni.OperationalStatus == OperationalStatus.Up));
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                Console.WriteLine($"Error checking mobile data connection: {ex.Message}");
            }
            return Task.FromResult(false);
        }

        internal static async Task<bool> IsConnectedAsync()
        {
            var isConnected = await IsConnectedToInternetAsync();
            if (!isConnected)
            {
                return false;
            }

            var isWifi = await IsConnectedToWifiAsync();
            var isMobileData = await IsConnectedToMobileDataAsync();

            return isWifi || isMobileData;
        }
    }
}