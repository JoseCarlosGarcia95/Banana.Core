using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Banana.Core.Libraries.StringUtils
{
    internal class StringUtils
    {
        /// <summary>
        ///     IntPtr to normal ansi string.
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        internal static string PtrToString(IntPtr ptr)
        {
            var final = FixString(Marshal.PtrToStringAnsi(ptr));

            return final;
        }

        /// <summary>
        ///     Patch random characters error.
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        private static string FixString(string ptr)
        {
            var bytes = Encoding.Default.GetBytes(ptr);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        ///     Get a pointer from string.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        internal static IntPtr StringToPtr(string x)
        {
            return Marshal.StringToHGlobalAnsi(x);
        }
    }
}