using MelodiaTherapy.Interfaces;
using Android.Provider;

namespace MelodiaTherapy.Platforms.Android
{
    public class DeviceIdService_Android : IDeviceIdService
    {
        public string? GetDeviceId()
        {
            var context = Platform.AppContext;
            return Settings.Secure.GetString(context.ContentResolver, Settings.Secure.AndroidId);
        }
    }
}