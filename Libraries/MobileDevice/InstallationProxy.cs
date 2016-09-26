using System;
using System.Runtime.InteropServices;

namespace Banana.Core.Libraries.MobileDevice
{

    #region Enums

    internal enum instproxy_error_t
    {
        INSTPROXY_E_SUCCESS = 0,
        INSTPROXY_E_INVALID_ARG = -1,
        INSTPROXY_E_PLIST_ERROR = -2,
        INSTPROXY_E_CONN_FAILED = -3,
        INSTPROXY_E_OP_IN_PROGRESS = -4,
        INSTPROXY_E_OP_FAILED = -5,
        INSTPROXY_E_RECEIVE_TIMEOUT = -6,
        INSTPROXY_E_UNKNOWN_ERROR = -256
    }

    #endregion

    internal class InstallationProxy
    {
        public const string INSTPROXY_SERVICE_NAME = "com.apple.mobile.installation_proxy";

        /// <summary>
        ///     Connects to the installation_proxy service on the specified device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern instproxy_error_t instproxy_client_new(IntPtr device, IntPtr service, ref IntPtr client);

        /// <summary>
        ///     Starts a new installation_proxy service on the specified device and connects to it.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern instproxy_error_t instproxy_client_start_service(IntPtr device, ref IntPtr client,
            IntPtr label);

        /// <summary>
        ///     Disconnects an installation_proxy client from the device and frees up the installation_proxy client data.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern instproxy_error_t instproxy_client_free(IntPtr client);

        /// <summary>
        ///     List installed applications.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern instproxy_error_t instproxy_browse(IntPtr client, IntPtr client_options, ref IntPtr result); //temp

        /// <summary>
        ///     Install an application on the device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern instproxy_error_t instproxy_install(IntPtr client, IntPtr pkg_path, IntPtr client_options,
            IntPtr status_cb, IntPtr user_data); //temp ???? void *user_data

        /// <summary>
        ///     Upgrade an application on the device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe instproxy_error_t instproxy_upgrade(IntPtr client, IntPtr pkg_path,
            IntPtr client_options, IntPtr status_cb, ref void* user_data); //temp ???? void *user_data

        /// <summary>
        ///     Uninstall an application from the device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern instproxy_error_t instproxy_uninstall(IntPtr client, IntPtr appid, IntPtr client_options,
            IntPtr status_cb, IntPtr user_data); //temp ???? void *user_data

        /// <summary>
        ///     List archived applications.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern instproxy_error_t instproxy_lookup_archives(IntPtr client, IntPtr client_options,
            ref IntPtr result); //temp

        /// <summary>
        ///     Archive an application on the device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern instproxy_error_t instproxy_archive(IntPtr client, IntPtr appid, IntPtr client_options,
            IntPtr status_cb, IntPtr user_data); //temp ???? void *user_data

        /// <summary>
        ///     Restore a previously archived application on the device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern instproxy_error_t instproxy_restore(IntPtr client, IntPtr appid, IntPtr client_options,
            IntPtr status_cb, IntPtr user_data); //temp ???? void *user_data

        /// <summary>
        ///     Removes a previously archived application from the device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe instproxy_error_t instproxy_remove_archive(IntPtr client, IntPtr appid,
            IntPtr client_options, IntPtr status_cb, ref void* user_data); //temp ???? void *user_data

        /// <summary>
        ///     Create a new client_options plist.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr instproxy_client_options_new();

        /// <summary>
        ///     Add one or more new key:value pairs to the given client_options
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void instproxy_client_options_add(IntPtr plist, string key, string value, string wtf);

        //WTF void 	instproxy_client_options_add (plist_t client_options,...)

        /// <summary>
        ///     Free client_options plist.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void instproxy_client_options_free(IntPtr client_options); //temp

        /// <summary>
        ///     Query the device for the path of an application.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern instproxy_error_t instproxy_client_get_path_for_bundle_identifier(IntPtr client,
            IntPtr bundle_id, ref IntPtr path);
    }
}