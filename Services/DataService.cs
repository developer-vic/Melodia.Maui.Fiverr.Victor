using MelodiaTherapy.Models;

namespace MelodiaTherapy.Services
{
    public class DataService
    {
        public static event Action<bool>? DataReadyChanged;
        public static bool IsReady { get; private set; } = false;

        public static async void InitStartData()
        {
            //await Task.Delay(2000); // TODO: Simulate fetching data
            IsReady = true;
            DataReadyChanged?.Invoke(IsReady);
        }

        internal static async void DownloadJsonsAsync()
        {
            //await Task.Delay(2000); // TODO: Simulate fetching data
            IsReady = true;
        }

        internal static async void LoadJsonsAsync()
        {
            //await Task.Delay(2000); // TODO: Simulate fetching data
            IsReady = true;
        }

        internal static async void SendDeviceInfoAsync()
        {
            //await Task.Delay(2000); // TODO: Simulate sending data
        }

        internal static async void SendStatistics(TreatmentModel selectedTreatment, AmbianceModel selectedAmbiance, ThemeModel selectedTheme, ListenDurationModel selectedListeningDuration, ListenTypeModel selectedListeningMode)
        {
            //await Task.Delay(2000); // TODO: Simulate sending data
        }
    }
}