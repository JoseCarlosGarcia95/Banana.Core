using System;
using System.Collections.Generic;
using Banana.Core.Libraries.MobileDevice;
using Banana.Core.Libraries.Plist;
using Banana.Core.Libraries.StringUtils;

namespace Banana.Core.Applications
{
    public class ApplicationManager
    {
        /// <summary>
        ///     Get an specified application.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="BundleID"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static ApplicationInfo GetApplicationInfo(Connection.Connection conn, string BundleID,
            string Type = "User")
        {
            var appsInfo = GetApplicationList(conn, Type);

            foreach (var appInfo in appsInfo)
                if (appInfo.BundleId == BundleID)
                    return appInfo;
            throw new Exception("Application " + BundleID + " can't be found");
        }

        /// <summary>
        ///     Get a list with every installed application.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static List<ApplicationInfo> GetApplicationList(Connection.Connection conn, string Type = "User")
        {
            int i;
            IntPtr plist, apps, service, ipc, app, item, item_s;

            apps = new IntPtr();
            service = new IntPtr();
            ipc = new IntPtr();
            var final = new List<ApplicationInfo>();

            var lockdownd = conn.GenerateLockdowndService();
            Lockdownd.lockdownd_start_service(lockdownd, StringUtils.StringToPtr("com.apple.mobile.installation_proxy"),
                ref service);

            InstallationProxy.instproxy_client_new(conn.myDevice, service, ref ipc);

            // Browse file
            plist = InstallationProxy.instproxy_client_options_new();
            InstallationProxy.instproxy_client_options_add(plist, "ApplicationType", Type, null);
            var errorType = InstallationProxy.instproxy_browse(ipc, plist, ref apps);


            for (i = 0; i < Plist.plist_array_get_size(apps); i++)
            {
                item_s = new IntPtr();
                // Initialize ApplicationInfo
                var appInfo = new ApplicationInfo();


                app = Plist.plist_array_get_item(apps, i);

                item = Plist.plist_dict_get_item(app, "CFBundleIdentifier");
                Plist.plist_get_string_val(item, ref item_s);

                appInfo.BundleId = StringUtils.PtrToString(item_s);

                item = Plist.plist_dict_get_item(app, "CFBundleName");
                Plist.plist_get_string_val(item, ref item_s);

                appInfo.BundleDisplayName = StringUtils.PtrToString(item_s);

                final.Add(appInfo);
            }
            return final;
        }


        /// <summary>
        ///     Install an application from the jailed path.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="path"></param>
        public static void InstallApplicationFromIpa(Connection.Connection conn, string path)
        {
            IntPtr plist, service, ipc, status, user_data;

            if (!path.EndsWith(".ipa"))
                throw new Exception(
                    "This funtion only works with *.ipa files. Try InstallApplication if you want to install a directory");

            status = new IntPtr();
            service = new IntPtr();
            ipc = new IntPtr();
            user_data = new IntPtr();

            var lockdownd = conn.GenerateLockdowndService();
            Lockdownd.lockdownd_start_service(lockdownd, StringUtils.StringToPtr("com.apple.mobile.installation_proxy"),
                ref service);

            InstallationProxy.instproxy_client_new(conn.myDevice, service, ref ipc);

            // Browse file
            plist = InstallationProxy.instproxy_client_options_new();
            // InstallationProxy.instproxy_client_options_add(plist, "ApplicationType", Type, null);
            var errorType = InstallationProxy.instproxy_install(ipc, StringUtils.StringToPtr(path), plist,
                status, user_data);


            if (errorType != instproxy_error_t.INSTPROXY_E_SUCCESS)
                throw new Exception("Error #" + errorType);
        }
    }
}