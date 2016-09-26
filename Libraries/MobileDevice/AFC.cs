using System;
using System.Runtime.InteropServices;

namespace Banana.Core.Libraries.MobileDevice
{

    #region Enums

    internal enum afc_error_t
    {
        AFC_E_SUCCESS = 0,
        AFC_E_UNKNOWN_ERROR = 1,
        AFC_E_OP_HEADER_INVALID = 2,
        AFC_E_NO_RESOURCES = 3,
        AFC_E_READ_ERROR = 4,
        AFC_E_WRITE_ERROR = 5,
        AFC_E_UNKNOWN_PACKET_TYPE = 6,
        AFC_E_INVALID_ARG = 7,
        AFC_E_OBJECT_NOT_FOUND = 8,
        AFC_E_OBJECT_IS_DIR = 9,
        AFC_E_PERM_DENIED = 10,
        AFC_E_SERVICE_NOT_CONNECTED = 11,
        AFC_E_OP_TIMEOUT = 12,
        AFC_E_TOO_MUCH_DATA = 13,
        AFC_E_END_OF_DATA = 14,
        AFC_E_OP_NOT_SUPPORTED = 15,
        AFC_E_OBJECT_EXISTS = 16,
        AFC_E_OBJECT_BUSY = 17,
        AFC_E_NO_SPACE_LEFT = 18,
        AFC_E_OP_WOULD_BLOCK = 19,
        AFC_E_IO_ERROR = 20,
        AFC_E_OP_INTERRUPTED = 21,
        AFC_E_OP_IN_PROGRESS = 22,
        AFC_E_INTERNAL_ERROR = 23,
        AFC_E_MUX_ERROR = 30,
        AFC_E_NO_MEM = 31,
        AFC_E_NOT_ENOUGH_DATA = 32,
        AFC_E_DIR_NOT_EMPTY = 33,
        AFC_E_FORCE_SIGNED_TYPE = -1
    }

    public enum afc_file_mode_t
    {
        AFC_FOPEN_RDONLY = 0x00000001,
        AFC_FOPEN_RW = 0x00000002,
        AFC_FOPEN_WRONLY = 0x00000003,
        AFC_FOPEN_WR = 0x00000004,
        AFC_FOPEN_APPEND = 0x00000005,
        AFC_FOPEN_RDAPPEND = 0x00000006
    }

    internal enum afc_link_type_t
    {
        AFC_HARDLINK = 1,
        AFC_SYMLINK = 2
    }

    internal enum afc_lock_op_t
    {
        AFC_LOCK_SH = 1 | 4,
        AFC_LOCK_EX = 2 | 4,
        AFC_LOCK_UN = 8 | 4
    }

    #endregion

    internal unsafe class AFC
    {
        #region AFC Device information

        /// <summary>
        ///     Get device information for a connected client.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern afc_error_t afc_get_device_info(IntPtr client, ref IntPtr device_information);

        #endregion

        #region AFC service operations

        /// <summary>
        ///     Makes a connection to the AFC service on the device
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern afc_error_t afc_client_new(IntPtr device, IntPtr service, ref IntPtr client);

        /// <summary>
        ///     Starts a new AFC service on the specified device and connects to it
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern afc_error_t afc_client_start_service(IntPtr device, ref IntPtr client, ref string label);

        /// <summary>
        ///     Frees up an AFC client
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern afc_error_t afc_client_free(IntPtr client);

        #endregion

        #region AFC FileSystem

        /// <summary>
        ///     Gets a directory listing of the directory requested.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern afc_error_t afc_read_directory(IntPtr client, string path, ref void** directory_information);

        /// <summary>
        ///     Gets information about a specific file.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern afc_error_t afc_get_file_info(IntPtr client, string filename,
            ref void** file_information);

        /// <summary>
        ///     Opens a file on the device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern afc_error_t afc_file_open(IntPtr client, string filename, afc_file_mode_t file_mode,
            ref ulong handle);

        /// <summary>
        ///     Closes a file on the device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern afc_error_t afc_file_close(IntPtr client, ulong handle);

        /// <summary>
        ///     Locks or unlocks a file on the device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern afc_error_t afc_file_lock(IntPtr client, ulong handle, afc_lock_op_t operation);

        /// <summary>
        ///     Attempts to the read the given number of bytes from the given file.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern afc_error_t afc_file_read(IntPtr client, ulong handle,
            IntPtr data, uint length,
            out uint bytes_read);

        /// <summary>
        ///     Writes a given number of bytes to a file.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern afc_error_t afc_file_write(IntPtr client, ulong handle, IntPtr data, uint length,
            ref uint bytes_written);

        /// <summary>
        ///     Seeks to a given position of a pre-opened file on the device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern afc_error_t afc_file_seek(IntPtr client, ulong handle, long offset, int whence);

        /// <summary>
        ///     Returns current position in a pre-opened file on the device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern afc_error_t afc_file_tell(IntPtr client, ulong handle, ref ulong position);

        /// <summary>
        ///     Sets the size of a file on the device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern afc_error_t afc_file_truncate(IntPtr client, ulong handle, ulong newsize);

        /// <summary>
        ///     Deletes a file or directory.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern afc_error_t afc_remove_path(IntPtr client, IntPtr path);

        /// <summary>
        ///     Renames a file or directory on the device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern afc_error_t afc_rename_path(IntPtr client, IntPtr from, IntPtr to);

        /// <summary>
        ///     Renames a file or directory on the device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern afc_error_t afc_make_directory(IntPtr client, IntPtr path);

        /// <summary>
        ///     Sets the size of a file on the device without prior opening it.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern afc_error_t afc_truncate(IntPtr client, IntPtr path, ulong newsize);

        /// <summary>
        ///     Creates a hard link or symbolic link on the device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern afc_error_t afc_make_link(IntPtr client, afc_link_type_t linktype, IntPtr target,
            IntPtr linkname);

        /// <summary>
        ///     Sets the modification time of a file on the device.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern afc_error_t afc_set_file_time(IntPtr client, IntPtr path, ulong mtime);

        /// <summary>
        ///     Deletes a file or directory including possible contents.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern afc_error_t afc_remove_path_and_contents(IntPtr client, IntPtr path);

        /// <summary>
        ///     Get a specific key of the device info list for a client connection.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern afc_error_t afc_get_device_info_key(IntPtr client, IntPtr key, ref IntPtr value);

        /// <summary>
        ///     Frees up a char dictionary as returned by some AFC functions.
        /// </summary>
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern afc_error_t afc_dictionary_free(ref IntPtr dictionary);

        #endregion
    }
}