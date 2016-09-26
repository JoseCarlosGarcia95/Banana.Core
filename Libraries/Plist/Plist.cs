using System;
using System.Runtime.InteropServices;

namespace Banana.Core.Libraries.Plist
{

    #region Enum

    internal enum plist_type
    {
        PLIST_BOOLEAN = 0, /**< Boolean, scalar type */
        PLIST_UINT = 1, /**< Unsigned integer, scalar type */
        PLIST_REAL = 2, /**< Real, scalar type */
        PLIST_STRING = 3, /**< ASCII string, scalar type */
        PLIST_ARRAY = 4, /**< Ordered array, structured type */
        PLIST_DICT = 5, /**< Unordered dictionary (key/value pair), structured type */
        PLIST_DATE = 6, /**< Date, scalar type */
        PLIST_DATA = 7, /**< Binary data, scalar type */
        PLIST_KEY = 8, /**< Key in dictionaries (ASCII String), scalar type */
        PLIST_NONE = 9 /**< No type */
    }

    #endregion

    internal class Plist
    {
        #region Useful functions for plist functions

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern plist_type plist_get_node_type(IntPtr node);

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void plist_get_string_val(IntPtr node, ref IntPtr val);


        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void plist_get_bool_val(IntPtr node, ref byte val);

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void plist_free(IntPtr plist);

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void plist_to_xml(IntPtr plist, out IntPtr plist_xml, ref uint length);

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void plist_to_bin(IntPtr plist, ref IntPtr s, ref uint length);

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void plist_from_xml(string plist_xml, int length, ref IntPtr plist);

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void plist_from_bin(string plist_xml, int length, ref IntPtr plist);

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr plist_array_get_item(IntPtr plist, int index);

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int plist_array_get_size(IntPtr plist);

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr plist_dict_get_item(IntPtr node, string key);

        #endregion
    }
}