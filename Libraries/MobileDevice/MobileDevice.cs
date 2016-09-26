using System;
using System.Runtime.InteropServices;
using Banana.Core.Libraries.MobileDevice.Callbacks;

namespace Banana.Core.Libraries.MobileDevice
{

    #region Enums

    internal enum idevice_error_t
    {
        IDEVICE_E_SUCCESS = 0,
        IDEVICE_E_INVALID_ARG = -1,
        IDEVICE_E_UNKNOWN_ERROR = -2,
        IDEVICE_E_NO_DEVICE = -3,
        IDEVICE_E_NOT_ENOUGH_DATA = -4,
        IDEVICE_E_BAD_HEADER = -5,
        IDEVICE_E_SSL_ERROR = -6
    }

    #endregion

    internal class MobileDevice
    {
        /// <summary>
        ///     Debugging
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void idevice_set_debug_level(int level);

        /// <summary>
        ///     Register a callback function that will be called when device add/remove events occur.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern idevice_error_t idevice_event_subscribe(DeviceEventCallback_t callback,
            out IntPtr user_data);

        /// <summary>
        ///     Release the event callback function that has been registered with idevice_event_subscribe()
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern idevice_error_t idevice_event_unsubscribe();

        /// <summary>
        ///     Get a list of currently available devices.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern idevice_error_t idevice_get_device_list(ref IntPtr devices, ref int count);

        /// <summary>
        ///     Creates an idevice_t structure for the device specified by udid, if the device is available.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern idevice_error_t idevice_new(ref IntPtr device, IntPtr udid);

        /// <summary>
        ///     Cleans up an idevice structure, then frees the structure itself.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern idevice_error_t idevice_free(IntPtr device);

        /// <summary>
        ///     Set up a connection to the given device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern idevice_error_t idevice_connect(IntPtr device, ushort port, ref IntPtr connection);

        /// <summary>
        ///     Set up a connection to the given device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern idevice_error_t idevice_disconnect(IntPtr connection);

        /// <summary>
        ///     Send data to a device via the given connection.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern idevice_error_t idevice_connection_send(IntPtr connection, IntPtr data, uint len,
            ref uint sent_bytes);

        /// <summary>
        ///     Receive data from a device via the given connection.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern idevice_error_t idevice_connection_receive_timeout(IntPtr connection, IntPtr data,
            uint len, ref uint recv_bytes, uint timeout);


        /// <summary>
        ///     Enables SSL for the given connection.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern idevice_error_t idevice_connection_enable_ssl(IntPtr connection);
    }
}