using System;
using System.Runtime.InteropServices;

namespace Banana.Core.Libraries.MobileDevice
{

    #region Enums

    internal enum misagent_error_t
    {
        MISAGENT_E_SUCCESS = 0,
        MISAGENT_E_INVALID_ARG = -1,
        MISAGENT_E_PLIST_ERROR = -2,
        MISAGENT_E_CONN_FAILED = -3,
        MISAGENT_E_REQUEST_FAILED = -4,
        MISAGENT_E_UNKNOWN_ERROR = -256
    }

    #endregion

    internal class Misagent
    {
        public const string MISAGENT_SERVICE_NAME = "com.apple.misagent";

        /// <summary>
        ///     Connects to the misagent service on the specified device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern misagent_error_t misagent_client_new(IntPtr device, IntPtr service,
            ref IntPtr client);

        /// <summary>
        ///     Starts a new misagent service on the specified device and connects to it.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern misagent_error_t misagent_client_start_service(IntPtr device, ref IntPtr client,
            IntPtr label);

        /// <summary>
        ///     Disconnects an misagent client from the device and frees up the misagent client data.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern misagent_error_t misagent_client_free(IntPtr client);

        /// <summary>
        ///     Installs the given provisioning profile.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern misagent_error_t misagent_install(IntPtr client, IntPtr profile); //temp

        /// <summary>
        ///     Retrieves an array of all installed provisioning profiles.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern misagent_error_t misagent_copy(IntPtr client, ref IntPtr profile); //temp

        /// <summary>
        ///     Removes a given provisioning profile.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern misagent_error_t misagent_remove(IntPtr client, IntPtr profileID);

        /// <summary>
        ///     Retrieves the status code from the last operation.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int misagent_get_status_code(IntPtr client);
    }
}