using System;
using System.Runtime.InteropServices;

namespace Banana.Core.Libraries.MobileDevice
{

    #region Enums

    internal enum file_relay_error_t
    {
        FILE_RELAY_E_SUCCESS = 0,
        FILE_RELAY_E_INVALID_ARG = -1,
        FILE_RELAY_E_PLIST_ERROR = -2,
        FILE_RELAY_E_MUX_ERROR = -3,
        FILE_RELAY_E_INVALID_SOURCE = -4,
        FILE_RELAY_E_STAGING_EMPTY = -5,
        FILE_RELAY_E_PERMISSION_DENIED = -6,
        FILE_RELAY_E_UNKNOWN_ERROR = -256
    }

    #endregion

    internal class FileRelay
    {
        public const string FILE_RELAY_SERVICE_NAME = "com.apple.mobile.file_relay";

        /// <summary>
        ///     Connects to the file_relay service on the specified device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern file_relay_error_t file_relay_client_new(IntPtr device, IntPtr service, ref IntPtr client);

        /// <summary>
        ///     Starts a new file_relay service on the specified device and connects to it.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern file_relay_error_t file_relay_client_start_service(IntPtr device, ref IntPtr client,
            IntPtr label);

        /// <summary>
        ///     Disconnects a file_relay client from the device and frees up the file_relay client data.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern file_relay_error_t file_relay_client_free(IntPtr client);

        /// <summary>
        ///     Request data for the given sources.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern file_relay_error_t file_relay_request_sources(IntPtr client, string[] sources,
            ref IntPtr connection);

        /// <summary>
        ///     Request data for the given sources.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe file_relay_error_t file_relay_request_sources_timeout(IntPtr client, void** sources,
            ref IntPtr connection, uint timeout);
    }
}