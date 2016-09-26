using System;
using System.Runtime.InteropServices;
using Banana.Core.Libraries.Plist;

namespace Banana.Core.Libraries.MobileDevice
{

    #region Enums

    internal enum lockdownd_error_t
    {
        LOCKDOWN_E_SUCCESS = 0,
        LOCKDOWN_E_INVALID_ARG = -1,
        LOCKDOWN_E_INVALID_CONF = -2,
        LOCKDOWN_E_PLIST_ERROR = -3,
        LOCKDOWN_E_PAIRING_FAILED = -4,
        LOCKDOWN_E_SSL_ERROR = -5,
        LOCKDOWN_E_DICT_ERROR = -6,
        LOCKDOWN_E_START_SERVICE_FAILED = -7,
        LOCKDOWN_E_NOT_ENOUGH_DATA = -8,
        LOCKDOWN_E_SET_VALUE_PROHIBITED = -9,
        LOCKDOWN_E_GET_VALUE_PROHIBITED = -10,
        LOCKDOWN_E_REMOVE_VALUE_PROHIBITED = -11,
        LOCKDOWN_E_MUX_ERROR = -12,
        LOCKDOWN_E_ACTIVATION_FAILED = -13,
        LOCKDOWN_E_PASSWORD_PROTECTED = -14,
        LOCKDOWN_E_NO_RUNNING_SESSION = -15,
        LOCKDOWN_E_INVALID_HOST_ID = -16,
        LOCKDOWN_E_INVALID_SERVICE = -17,
        LOCKDOWN_E_INVALID_ACTIVATION_RECORD = -18,
        LOCKDOWN_E_PAIRING_DIALOG_PENDING = -20,
        LOCKDOWN_E_USER_DENIED_PAIRING = -21,
        LOCKDOWN_E_UNKNOWN_ERROR = -256
    }

    #endregion

    internal class Lockdownd
    {
        #region Useful connection functions

        /// <summary>
        ///     Creates a new lockdownd client for the device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern lockdownd_error_t lockdownd_client_new(IntPtr device, ref IntPtr lockdown_s, IntPtr label);

        /// <summary>
        ///     Creates a new lockdownd client for the device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern lockdownd_error_t lockdownd_client_new_with_handshake(IntPtr device,
            ref IntPtr lockdown_s, IntPtr label);

        /// <summary>
        ///     Closes the lockdownd client session if one is running and frees up the lockdownd_client struct.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern lockdownd_error_t lockdownd_client_free(IntPtr lockdown_s);

        /// <summary>
        ///     Query the type of the service daemon.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern lockdownd_error_t lockdownd_query_type(ref IntPtr lockdown_s, ref IntPtr type);

        /// <summary>
        ///     Retrieves a preferences plist using an optional domain and/or key name.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern lockdownd_error_t lockdownd_get_value(IntPtr lockdown_s, IntPtr domain, IntPtr key,
            ref IntPtr plist_list); //temp

        /// <summary>
        ///     Sets a preferences value using a plist and optional by domain and/or key name.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern lockdownd_error_t lockdownd_set_value(ref IntPtr lockdown_s, IntPtr domain, IntPtr key,
            ref IntPtr plist_list); //temp

        /// <summary>
        ///     Removes a preference node by domain and/or key name.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern lockdownd_error_t lockdownd_remove_value(ref IntPtr lockdown_s, IntPtr domain, IntPtr key);

        /// <summary>
        ///     Requests to start a service and retrieve it's port on success.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern lockdownd_error_t lockdownd_start_service(IntPtr lockdown_s, IntPtr identifier,
            ref IntPtr service);

        /// <summary>
        ///     Requests to start a service and retrieve it's port on success.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern lockdownd_error_t lockdownd_start_service_with_escrow_bag(IntPtr lockdown_s,
            IntPtr identifier, ref IntPtr service);

        /// <summary>
        ///     Opens a session with lockdownd and switches to SSL mode if device wants it.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern lockdownd_error_t lockdownd_start_session(IntPtr client, IntPtr host_id,
            ref IntPtr session_id, ref int ssl_enabled);

        /// <summary>
        ///     Closes the lockdownd session by sending the StopSession request.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern lockdownd_error_t lockdownd_stop_session(IntPtr client, IntPtr session_id);

        /// <summary>
        ///     Sends a plist to lockdownd.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern lockdownd_error_t lockdownd_send(IntPtr client, ref IntPtr plist_list); //temp

        /// <summary>
        ///     Receives a plist from lockdownd.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern lockdownd_error_t lockdownd_receive(IntPtr client, ref IntPtr plist_list); //temp

        /// <summary>
        ///     Creates a new lockdownd client for the device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern lockdownd_error_t lockdownd_pair(IntPtr lockdown_s, IntPtr pair_record);

        /// <summary>
        ///     Creates a new lockdownd client for the device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern lockdownd_error_t lockdownd_validate_pair(IntPtr lockdown_s, IntPtr pair_record);

        /// <summary>
        ///     Delete lockdownd client for the device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern lockdownd_error_t lockdownd_unpair(IntPtr lockdown_s, IntPtr pair_record);

        /// <summary>
        ///     Activates the device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern lockdownd_error_t lockdownd_activate(IntPtr lockdown_s, IntPtr activation_record); //temp

        /// <summary>
        ///     Deactivates the device, returning it to the locked “Activate with iTunes” screen.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern lockdownd_error_t lockdownd_deactivate(IntPtr lockdown_s);

        /// <summary>
        ///     Enter recovery mode
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern lockdownd_error_t lockdownd_enter_recovery(IntPtr lockdown_s);

        /// <summary>
        ///     Sends the Goodbye request to lockdownd signaling the end of communication.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern lockdownd_error_t lockdownd_goodbye(IntPtr lockdown_s);

        /// <summary>
        ///     Sets the label to send for requests to lockdownd.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void lockdownd_client_set_label(IntPtr control, IntPtr label);

        /// <summary>
        ///     Returns the unique id of the device from lockdownd.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern lockdownd_error_t lockdownd_get_device_udid(IntPtr lockdown_s, ref IntPtr udid);

        /// <summary>
        ///     Get device name
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern lockdownd_error_t lockdownd_get_device_name(IntPtr client, ref IntPtr device_name);

        /// <summary>
        ///     Calculates and returns the data classes the device supports from lockdownd.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern lockdownd_error_t lockdownd_get_sync_data_classes(IntPtr client, ref IntPtr classes,
            int count);

        /// <summary>
        ///     Frees memory of an allocated array of data classes as returned by lockdownd_get_sync_data_classes()
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern lockdownd_error_t lockdownd_data_classes_free(ref IntPtr classes);

        /// <summary>
        ///     Frees memory of a service descriptor as returned by lockdownd_start_service()
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern lockdownd_error_t lockdownd_service_descriptor_free(IntPtr service);

        public static string lockdownd_get_string_value(IntPtr lockdownd_handle, string domain, string key)
        {
            IntPtr val_node = IntPtr.Zero, ptrval = IntPtr.Zero;
            lockdownd_get_value(lockdownd_handle, StringUtils.StringUtils.StringToPtr(domain),
                StringUtils.StringUtils.StringToPtr(key), ref val_node);

            if ((val_node == null) || (Plist.Plist.plist_get_node_type(val_node) != plist_type.PLIST_STRING))
                throw new Exception("Unable to find " + key);
            Plist.Plist.plist_get_string_val(val_node, ref ptrval);
            Plist.Plist.plist_free(val_node);
            return StringUtils.StringUtils.PtrToString(ptrval);
        }

        public static string lockdownd_get_xml_value_from_domain(IntPtr lockdownd_handle, string domain)
        {
            IntPtr val_node = IntPtr.Zero, ptrval = IntPtr.Zero;
            lockdownd_get_value(lockdownd_handle, StringUtils.StringUtils.StringToPtr(domain), IntPtr.Zero, ref val_node);

            if (val_node == null)
                throw new Exception("Unable to find " + domain);
            uint length = 0;
            Plist.Plist.plist_to_xml(val_node, out ptrval, ref length);
            Plist.Plist.plist_free(val_node);
            return StringUtils.StringUtils.PtrToString(ptrval);
        }

        public static string lockdownd_get_string_value(IntPtr lockdownd_handle, string key)
        {
            IntPtr val_node = IntPtr.Zero, ptrval = IntPtr.Zero;
            lockdownd_get_value(lockdownd_handle, IntPtr.Zero, StringUtils.StringUtils.StringToPtr(key), ref val_node);

            if ((val_node == null) || (Plist.Plist.plist_get_node_type(val_node) != plist_type.PLIST_STRING))
                throw new Exception("Unable to find " + key);
            Plist.Plist.plist_get_string_val(val_node, ref ptrval);
            Plist.Plist.plist_free(val_node);
            return StringUtils.StringUtils.PtrToString(ptrval);
        }

        public static byte lockdownd_get_boolean_value(IntPtr lockdownd_handle, string key)
        {
            var val_node = IntPtr.Zero;

            lockdownd_get_value(lockdownd_handle, IntPtr.Zero, StringUtils.StringUtils.StringToPtr(key), ref val_node);

            if (val_node == null)
                throw new Exception("Unable to find " + key);
            byte resp = 0;
            Plist.Plist.plist_get_bool_val(val_node, ref resp);
            Plist.Plist.plist_free(val_node);
            return resp;
        }

        #endregion
    }
}