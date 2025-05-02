namespace MelodiaTherapy.Services
{
   public static class DataService
    {
        public static event Action<bool>? DataReadyChanged;
        public static bool IsReady { get; private set; } = false;

        public static async Task InitStartData()
        {
            await Task.Delay(2000); // TODO: Simulate fetching data
            IsReady = true;
            DataReadyChanged?.Invoke(IsReady);
        }
    }
}