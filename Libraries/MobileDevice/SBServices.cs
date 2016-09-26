using System;
using System.Runtime.InteropServices;

namespace Banana.Core.Libraries.MobileDevice
{
    internal enum sbservices_error_t
    {
        SBSERVICES_E_SUCCESS = 0,
        SBSERVICES_E_INVALID_ARG = -1,
        SBSERVICES_E_PLIST_ERROR = -2,
        SBSERVICES_E_CONN_FAILED = -3,
        SBSERVICES_E_UNKNOWN_ERROR = -256
    }

    internal class SBServices
    {
        /// <summary>
        ///     Starts a new sbservices service on the specified device and connects to it.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern sbservices_error_t sbservices_client_start_service(IntPtr device, ref IntPtr sbclient,
            string label);

        /// <summary>
        ///     Get the icon of the specified app as PNG data.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe sbservices_error_t sbservices_get_icon_pngdata(IntPtr sbclient, string bundleId,
            ref char* pngdata, ref ulong pngsize);


        /// <summary>
        ///     Get the home screen wallpaper as PNG data.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe sbservices_error_t sbservices_get_home_screen_wallpaper_pngdata(IntPtr client,
            ref char* pngdata, ref ulong pngsize);

        /// <summary>
        ///     Gets the icon state of the connected device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern sbservices_error_t sbservices_get_icon_state(IntPtr client, ref IntPtr state,
            string format_version);


        /// <summary>
        ///  Set icon state of the connected device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern sbservices_error_t sbservices_set_icon_state(IntPtr client, IntPtr newstate);
    }
}