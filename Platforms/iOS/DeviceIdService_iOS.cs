using MelodiaTherapy.Interfaces;
using UIKit;

namespace MelodiaTherapy.Platforms.iOS
{
    public class DeviceIdService_iOS : IDeviceIdService
    {
        public string? GetDeviceId()
        {
            return UIDevice.CurrentDevice.IdentifierForVendor?.AsString();
        }
    }
}