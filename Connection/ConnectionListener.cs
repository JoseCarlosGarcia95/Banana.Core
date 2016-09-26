using System;
using Banana.Core.Exceptions;
using Banana.Core.Libraries.MobileDevice;
using Banana.Core.Libraries.MobileDevice.Callbacks;

namespace Banana.Core.Connection
{
    public class ConnectionListener
    {
        public DeviceEventCallback_t OnDeviceEvent;

        public void Start()
        {
            if (OnDeviceEvent == null)
                throw new InvalidDelegateVoid("OnDeviceEvent hasn't got any value.");

            IntPtr ptr;
            MobileDevice.idevice_set_debug_level(1);
            MobileDevice.idevice_event_subscribe(OnDeviceEvent, out ptr);
        }
    }
}