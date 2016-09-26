using System.Runtime.InteropServices;

namespace Banana.Core.Libraries.MobileDevice.Callbacks
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DeviceSyslogCallback_t(char c);
}