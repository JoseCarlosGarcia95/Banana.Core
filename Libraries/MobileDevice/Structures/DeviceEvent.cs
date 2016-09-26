using System;
using System.Runtime.InteropServices;

namespace Banana.Core.Libraries.MobileDevice.Structures
{
    public enum idevice_event_type
    {
        IDEVICE_DEVICE_ADD = 1,
        IDEVICE_DEVICE_REMOVE
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DeviceEvent
    {
        public idevice_event_type @event;
        public IntPtr udid;
        public int conn_type;
    }
}