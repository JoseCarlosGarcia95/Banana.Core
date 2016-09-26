using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Banana.Core.Libraries.MobileDevice;
using Banana.Core.Libraries.StringUtils;
using PNGDecrush;

namespace Banana.Core.Applications
{
    public class ApplicationInfo
    {
        public string BundleDisplayName;
        public string BundleId;

        /// <summary>
        ///     Generate application icon and return the url.
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public unsafe string GenerateImageIcon(Connection.Connection conn)
        {
            var sbclient = new IntPtr();
            char* pngData = null;
            ulong pngSize = 0;

            if (!Directory.Exists("cache"))
                Directory.CreateDirectory("cache");
            if (!Directory.Exists("cache\\icons"))
                Directory.CreateDirectory("cache\\icons");
            if (!File.Exists("cache\\icons\\" + BundleId + ".png"))
            {
                var errorType = SBServices.sbservices_client_start_service(conn.myDevice, ref sbclient,
                    "Banana");

                if (errorType != sbservices_error_t.SBSERVICES_E_SUCCESS)
                    throw new Exception("Error while we generate the image " + errorType);

                SBServices.sbservices_get_icon_pngdata(sbclient, BundleId, ref pngData, ref pngSize);

                if (pngSize > 0)
                {
                    var newData = new byte[pngSize];
                    Marshal.Copy((IntPtr) pngData, newData, 0, (int) pngSize);
                    var wr = new BinaryWriter(File.Create("cache\\icons\\" + BundleId + ".png"));
                    wr.Write(newData);
                    wr.Close();

                    using (var input = File.OpenRead("cache\\icons\\" + BundleId + ".png"))
                    {
                        using (var output = File.Create("cache\\icons\\" + BundleId + ".clean.png"))
                        {
                            try
                            {
                                PNGDecrusher.Decrush(input, output);
                            }
                            catch (InvalidDataException)
                            {
                            }
                        }
                    }
                    File.Delete("cache\\icons\\" + BundleId + ".clean.png");
                }
            }
            return "cache\\icons\\" + BundleId + ".png";
        }


        /// <summary>
        ///     Uninstall an application.
        /// </summary>
        public void Uninstall(Connection.Connection conn)
        {
            var status = new IntPtr();
            var service = new IntPtr();
            var ipc = new IntPtr();
            var userData = new IntPtr();
            var final = new List<ApplicationInfo>();

            var lockdownd = conn.GenerateLockdowndService();
            Lockdownd.lockdownd_start_service(lockdownd, StringUtils.StringToPtr("com.apple.mobile.installation_proxy"),
                ref service);

            InstallationProxy.instproxy_client_new(conn.myDevice, service, ref ipc);

            // Browse file
            var plist = InstallationProxy.instproxy_client_options_new();
            InstallationProxy.instproxy_client_options_add(plist, "ApplicationType", "User", null);

            var errorType = InstallationProxy.instproxy_uninstall(ipc, StringUtils.StringToPtr(BundleId), plist, status,
                userData);

            if (errorType != instproxy_error_t.INSTPROXY_E_SUCCESS)
                throw new Exception("\"" + BundleId + "\" couldn't be uninstalled #" + errorType);
        }

        /// <summary>
        ///     Get the path of an app.
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public string GetJailedFilePath(Connection.Connection conn)
        {
            IntPtr plist, service, ipc, path;

            path = new IntPtr();
            service = new IntPtr();
            ipc = new IntPtr();
            var final = new List<ApplicationInfo>();

            var lockdownd = conn.GenerateLockdowndService();
            Lockdownd.lockdownd_start_service(lockdownd, StringUtils.StringToPtr("com.apple.mobile.installation_proxy"),
                ref service);

            InstallationProxy.instproxy_client_new(conn.myDevice, service, ref ipc);

            // Browse file
            plist = InstallationProxy.instproxy_client_options_new();
            InstallationProxy.instproxy_client_options_add(plist, "ApplicationType", "User", null);
            var errorType = InstallationProxy.instproxy_client_get_path_for_bundle_identifier(ipc,
                StringUtils.StringToPtr(BundleId), ref path);

            if (errorType != instproxy_error_t.INSTPROXY_E_SUCCESS)
                throw new Exception("Error while getting app path #" + errorType);

            return StringUtils.PtrToString(path);
        }
    }
}