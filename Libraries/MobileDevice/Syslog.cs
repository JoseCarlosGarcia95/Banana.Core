using System;
using System.Runtime.InteropServices;
using Banana.Core.Libraries.MobileDevice.Callbacks;

namespace Banana.Core.Libraries.MobileDevice
{
    internal enum syslog_relay_error_t
    {
        SYSLOG_RELAY_E_SUCCESS = 0,
        SYSLOG_RELAY_E_INVALID_ARG = -1,
        SYSLOG_RELAY_E_MUX_ERROR = -2,
        SYSLOG_RELAY_E_SSL_ERROR = -3,
        SYSLOG_RELAY_E_UNKNOWN_ERROR = -256
    }

    internal class Syslog
    {
        /// <summary>
        ///     Connects to the syslog_relay service on the specified device.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="service"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern syslog_relay_error_t syslog_relay_client_new(IntPtr device, IntPtr service,
            ref IntPtr client);

        /// <summary>
        ///     Starts a new syslog_relay service on the specified device and connects to it.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="client"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern syslog_relay_error_t syslog_relay_client_start_service(IntPtr device, ref IntPtr client,
            IntPtr label);

        /// <summary>
        ///     Disconnects a syslog_relay client from the device and frees up the syslog_relay client data.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern syslog_relay_error_t syslog_relay_client_free(IntPtr client);


        /// <summary>
        ///     Starts capturing the syslog of the device using a callback.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern syslog_relay_error_t syslog_relay_start_capture(IntPtr client,
            DeviceSyslogCallback_t callback,
            out IntPtr user_data);


        /// <summary>
        ///     Stops capturing the syslog of the device using a callback.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern syslog_relay_error_t syslog_relay_stop_capture(IntPtr client);
    }
}