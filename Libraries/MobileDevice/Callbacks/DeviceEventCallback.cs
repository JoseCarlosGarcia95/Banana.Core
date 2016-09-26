using System;
using System.Runtime.InteropServices;
using Banana.Core.Libraries.MobileDevice.Structures;

namespace Banana.Core.Libraries.MobileDevice.Callbacks
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DeviceEventCallback_t(ref DeviceEventCallback callback_info);

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DeviceEventCallback
    {
        internal DeviceEvent @event;
        internal IntPtr user_data;
    }
}