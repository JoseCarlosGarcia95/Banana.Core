using System;
using System.Collections.Generic;
using Banana.Core.Libraries.MobileDevice;
using Banana.Core.Libraries.StringUtils;

namespace Banana.Core.Stream
{
    public class DeviceDirectory
    {
        /// <summary>
        ///     Delete directory.
        /// </summary>
        /// <param name="conn">iDevice connection class</param>
        /// <param name="path">Path to the file.</param>
        /// <param name="recursive">Subdirectories?</param>
        public static void DeleteDirectory(Connection.Connection conn, string path, bool recursive = false)
        {
            var afc = new IntPtr();
            var afcClient = new IntPtr();
            var lockdownd = conn.GenerateLockdowndService();


            Lockdownd.lockdownd_start_service(lockdownd,
                StringUtils.StringToPtr(conn.Jailbroken ? "com.apple.afc2" : "com.apple.afc"), ref afc);
            AFC.afc_client_new(conn.myDevice, afc, ref afcClient);

            if (!recursive)
                AFC.afc_remove_path_and_contents(afcClient, StringUtils.StringToPtr(path));
            else
                AFC.afc_remove_path(afcClient, StringUtils.StringToPtr(path));

            AFC.afc_client_free(afcClient);
            Lockdownd.lockdownd_service_descriptor_free(afc);
            conn.CloseLockdownd(lockdownd);
        }

        /// <summary>
        ///     Create a new directory with the name "path"
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="path"></param>
        public static void CreateDirectory(Connection.Connection conn, string path)
        {
            var afc = new IntPtr();
            var afcClient = new IntPtr();
            var lockdownd = conn.GenerateLockdowndService();

            Lockdownd.lockdownd_start_service(lockdownd,
                StringUtils.StringToPtr(conn.Jailbroken ? "com.apple.afc2" : "com.apple.afc"), ref afc);
            AFC.afc_client_new(conn.myDevice, afc, ref afcClient);
            AFC.afc_make_directory(afcClient, StringUtils.StringToPtr(path));

            AFC.afc_client_free(afcClient);
            Lockdownd.lockdownd_service_descriptor_free(afc);
            conn.CloseLockdownd(lockdownd);
        }

        /// <summary>
        ///     Rename folder.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void Rename(Connection.Connection conn, string from, string to)
        {
            var afc = new IntPtr();
            var afcClient = new IntPtr();
            var lockdownd = conn.GenerateLockdowndService();

            Lockdownd.lockdownd_start_service(lockdownd,
                StringUtils.StringToPtr(conn.Jailbroken ? "com.apple.afc2" : "com.apple.afc"), ref afc);
            AFC.afc_client_new(conn.myDevice, afc, ref afcClient);
            AFC.afc_rename_path(afcClient, StringUtils.StringToPtr(from), StringUtils.StringToPtr(to));

            AFC.afc_client_free(afcClient);
            Lockdownd.lockdownd_service_descriptor_free(afc);
            conn.CloseLockdownd(lockdownd);
        }

        /// <summary>
        ///     Get files in directory.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] GetPathInformation(Connection.Connection conn, string path)
        {
            var afc = new IntPtr();
            var afcClient = new IntPtr();
            var lockdownd = conn.GenerateLockdowndService();

            var i = 0;
            var directoryInfo = new List<string>();

            unsafe
            {
                string dirName;
                void** dirInfo = null;
                Lockdownd.lockdownd_start_service(lockdownd,
                    StringUtils.StringToPtr(conn.Jailbroken ? "com.apple.afc2" : "com.apple.afc"), ref afc);
                AFC.afc_client_new(conn.myDevice, afc, ref afcClient);
                AFC.afc_read_directory(afcClient, path, ref dirInfo);

                while ((dirInfo[i] != null) && ((dirName = StringUtils.PtrToString((IntPtr) dirInfo[i++])) != ""))
                    directoryInfo.Add(dirName);
            }

            return directoryInfo.ToArray();
        }


        public void CreateSymbolicLink(Connection.Connection conn, string path, string link)
        {
            var afc = new IntPtr();
            var afcClient = new IntPtr();
            var lockdownd = conn.GenerateLockdowndService();

            Lockdownd.lockdownd_start_service(lockdownd,
                StringUtils.StringToPtr(conn.Jailbroken ? "com.apple.afc2" : "com.apple.afc"), ref afc);
            AFC.afc_client_new(conn.myDevice, afc, ref afcClient);

            var error = AFC.afc_make_link(afcClient, afc_link_type_t.AFC_HARDLINK, StringUtils.StringToPtr(path),
                StringUtils.StringToPtr(link));

            if (error != afc_error_t.AFC_E_SUCCESS)
                throw new Exception("Fail creating symlink #" + error);
        }
    }
}