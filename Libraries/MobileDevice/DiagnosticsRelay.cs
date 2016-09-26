using System;
using System.Runtime.InteropServices;

namespace Banana.Core.Libraries.MobileDevice
{

    #region Enums

    internal enum diagnostics_relay_error_t
    {
        DIAGNOSTICS_RELAY_E_SUCCESS = 0,
        DIAGNOSTICS_RELAY_E_INVALID_ARG = -1,
        DIAGNOSTICS_RELAY_E_PLIST_ERROR = -2,
        DIAGNOSTICS_RELAY_E_MUX_ERROR = -3,
        DIAGNOSTICS_RELAY_E_UNKNOWN_REQUEST = -4,
        DIAGNOSTICS_RELAY_E_UNKNOWN_ERROR = -256
    }

    #endregion

    internal class DiagnosticsRelay
    {
        public const string DIAGNOSTICS_RELAY_SERVICE_NAME = "com.apple.mobile.diagnostics_relay";
        public const int DIAGNOSTICS_RELAY_ACTION_FLAG_WAIT_FOR_DISCONNECT = 1 << 1;
        public const int DIAGNOSTICS_RELAY_ACTION_FLAG_DISPLAY_PASS = 1 << 2;
        public const int DIAGNOSTICS_RELAY_ACTION_FLAG_DISPLAY_FAIL = 1 << 3;
        public const string DIAGNOSTICS_RELAY_REQUEST_TYPE_ALL = "All";
        public const string DIAGNOSTICS_RELAY_REQUEST_TYPE_WIFI = "WiFi";
        public const string DIAGNOSTICS_RELAY_REQUEST_TYPE_GAS_GAUGE = "GasGauge";
        public const string DIAGNOSTICS_RELAY_REQUEST_TYPE_NAND = "NAND";

        /// <summary>
        ///     Connects to the diagnostics_relay service on the specified device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern diagnostics_relay_error_t diagnostics_relay_client_new(IntPtr device, IntPtr service,
            ref IntPtr client);

        /// <summary>
        ///     Starts a new diagnostics_relay service on the specified device and connects to it.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern diagnostics_relay_error_t diagnostics_relay_client_start_service(IntPtr device,
            ref IntPtr client, IntPtr label);

        /// <summary>
        ///     Disconnects a diagnostics_relay client from the device and frees up the diagnostics_relay client data.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern diagnostics_relay_error_t diagnostics_relay_client_free(IntPtr client);

        /// <summary>
        ///     Sends the Goodbye request signaling the end of communication.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern diagnostics_relay_error_t diagnostics_relay_goodbye(IntPtr client);

        /// <summary>
        ///     Puts the device into deep sleep mode and disconnects from host.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern diagnostics_relay_error_t diagnostics_relay_sleep(IntPtr client);

        /// <summary>
        ///     Restart the device and optionally show a user notification.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern diagnostics_relay_error_t diagnostics_relay_restart(IntPtr client, int flags);

        /// <summary>
        ///     Shutdown of the device and optionally show a user notification.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern diagnostics_relay_error_t diagnostics_relay_shutdown(IntPtr client, int flags);

        /// <summary>
        ///     ???
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern diagnostics_relay_error_t diagnostics_relay_request_diagnostics(IntPtr client, IntPtr type,
            ref IntPtr diagnostics); //temp

        /// <summary>
        ///     ???
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern diagnostics_relay_error_t diagnostics_relay_query_mobilegestalt(IntPtr client, IntPtr keys,
            ref IntPtr result); //temp

        /// <summary>
        ///     ???
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern diagnostics_relay_error_t diagnostics_relay_query_ioregistry_entry(IntPtr client,
            IntPtr name, IntPtr class1, ref IntPtr result); //temp

        /// <summary>
        ///     ???
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern diagnostics_relay_error_t diagnostics_relay_query_ioregistry_plane(IntPtr client,
            IntPtr plane, ref IntPtr result); //temp
    }
}