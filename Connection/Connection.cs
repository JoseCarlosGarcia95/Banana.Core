using System;
using System.IO;
using System.Runtime.InteropServices;
using Banana.Core.Exceptions;
using Banana.Core.Libraries.MobileDevice;
using Banana.Core.Libraries.MobileDevice.Callbacks;
using Banana.Core.Libraries.Plist;
using Banana.Core.Libraries.StringUtils;
using PNGDecrush;

namespace Banana.Core.Connection
{
    public class Connection
    {
        internal IntPtr myDevice;

        public Connection(DeviceEventCallback cb)
        {
            myDevice = new IntPtr();
            var ex = MobileDevice.idevice_new(ref myDevice, cb.@event.udid);


            if (ex != idevice_error_t.IDEVICE_E_SUCCESS)
                throw new ErrorOnConnect("Connection exception: " + ex);

            UniqueDeviceId = StringUtils.PtrToString(cb.@event.udid);
            GenerateDeviceName();
            GenerateJailbreakStatus();
        }

        /// <summary>
        ///     UDID from iDevice
        /// </summary>
        public string UniqueDeviceId { get; private set; }

        /// <summary>
        ///     User device name.
        /// </summary>
        public string DeviceName { get; private set; }

        /// <summary>
        ///     Retrieve jailbreak status.
        /// </summary>
        public bool Jailbroken { get; private set; }

        /// <summary>
        ///     Get iOS version in this format: 9.3.2
        /// </summary>
        public string GetProductVersion => GetLockdowndValue("ProductVersion");

        /// <summary>
        ///     Get iBoot build
        /// </summary>
        public string GetFirmwareVersion => GetLockdowndValue("FirmwareVersion");

        /// <summary>
        ///     Get iOS build hexnumber
        /// </summary>
        public string GetBuildNumber => GetLockdowndValue("BuildVersion");

        /// <summary>
        ///     Get activation state.
        /// </summary>
        public string GetActivationState => GetLockdowndValue("ActivationState");

        /// <summary>
        ///     Device class return iPod, iPad or iPhone
        /// </summary>
        public string GetDeviceClass => GetLockdowndValue("DeviceClass");

        /// <summary>
        ///     Get the device type. Ex: iPhone7,1
        /// </summary>
        public string GetProductType => GetLockdowndValue("ProductType");


        /// <summary>
        ///     Retrieve the color of your device.
        /// </summary>
        public string GetDevicecolor => GetLockdowndValue("DeviceColor");

        /// <summary>
        ///     Retrieve the serial number of the device
        /// </summary>
        public string GetSerialNumber => GetLockdowndValue("SerialNumber");

        /// <summary>
        ///     Retrieve the model number of the device
        /// </summary>
        public string GetModelNumber => GetLockdowndValue("ModelNumber");

        /// <summary>
        ///     Retrieve the model number of the device
        /// </summary>
        public string GetHardwareModel => GetLockdowndValue("HardwareModel");

        /// <summary>
        ///     Retrieve the region info of the device
        /// </summary>
        public string GetRegionInfo => GetLockdowndValue("RegionInfo");


        /// <summary>
        ///     Get a lockdownd information from key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type">String, Boolean, Int</param>
        /// <returns></returns>
        public string GetLockdowndValue(string key, string type = "String")
        {
            switch (type)
            {
                case "String":
                    return Lockdownd.lockdownd_get_string_value(GenerateLockdowndService(), key);
                case "Boolean":
                    return Lockdownd.lockdownd_get_boolean_value(GenerateLockdowndService(), key).ToString();
                case "Domain":
                    return Lockdownd.lockdownd_get_xml_value_from_domain(GenerateLockdowndService(), key);
            }

            return null;
        }

        public string GetLockdowndValueFromDomain(string key, string domain, string type = "String")
        {
            switch (type)
            {
                case "String":
                    return Lockdownd.lockdownd_get_string_value(GenerateLockdowndService(), domain, key);
            }

            return null;
            return null;
        }

        /// <summary>
        ///     Shutdown iOS device.
        /// </summary>
        public void Shutdown()
        {
            IntPtr lockdownd = GenerateLockdowndService(), service, drc;

            service = new IntPtr();
            drc = new IntPtr();

            if (
                Lockdownd.lockdownd_start_service(lockdownd,
                    StringUtils.StringToPtr(DiagnosticsRelay.DIAGNOSTICS_RELAY_SERVICE_NAME), ref service) !=
                lockdownd_error_t.LOCKDOWN_E_SUCCESS)
                throw new Exception("Can't start diagnostic service.");

            if (DiagnosticsRelay.diagnostics_relay_client_new(myDevice, service, ref drc) !=
                diagnostics_relay_error_t.DIAGNOSTICS_RELAY_E_SUCCESS)
                throw new Exception("Diagnostic fail on starts");

            DiagnosticsRelay.diagnostics_relay_shutdown(drc, DiagnosticsRelay.DIAGNOSTICS_RELAY_ACTION_FLAG_DISPLAY_FAIL);
        }

        /// <summary>
        ///     Restart iOS device.
        /// </summary>
        public void Restart()
        {
            IntPtr lockdownd = GenerateLockdowndService(), service, drc;

            service = new IntPtr();
            drc = new IntPtr();

            if (
                Lockdownd.lockdownd_start_service(lockdownd,
                    StringUtils.StringToPtr(DiagnosticsRelay.DIAGNOSTICS_RELAY_SERVICE_NAME), ref service) !=
                lockdownd_error_t.LOCKDOWN_E_SUCCESS)
                throw new Exception("Can't start diagnostic service.");

            if (DiagnosticsRelay.diagnostics_relay_client_new(myDevice, service, ref drc) !=
                diagnostics_relay_error_t.DIAGNOSTICS_RELAY_E_SUCCESS)
                throw new Exception("Diagnostic fail on starts");

            DiagnosticsRelay.diagnostics_relay_restart(drc, DiagnosticsRelay.DIAGNOSTICS_RELAY_ACTION_FLAG_DISPLAY_FAIL);
        }

        /// <summary>
        ///     Sleep iOS device.
        /// </summary>
        public void Sleep()
        {
            IntPtr lockdownd = GenerateLockdowndService(), service, drc;

            service = new IntPtr();
            drc = new IntPtr();

            if (
                Lockdownd.lockdownd_start_service(lockdownd,
                    StringUtils.StringToPtr(DiagnosticsRelay.DIAGNOSTICS_RELAY_SERVICE_NAME), ref service) !=
                lockdownd_error_t.LOCKDOWN_E_SUCCESS)
                throw new Exception("Can't start diagnostic service.");

            if (DiagnosticsRelay.diagnostics_relay_client_new(myDevice, service, ref drc) !=
                diagnostics_relay_error_t.DIAGNOSTICS_RELAY_E_SUCCESS)
                throw new Exception("Diagnostic fail on starts");

            DiagnosticsRelay.diagnostics_relay_sleep(drc);
        }

        /// <summary>
        ///     Retrieve an xml list with icons.
        /// </summary>
        /// <returns></returns>
        public string GetSBIconState()
        {
            var sbclient = new IntPtr();
            var plist = new IntPtr();
            var xml = new IntPtr();

            uint length = 0;

            var errorType = SBServices.sbservices_client_start_service(myDevice, ref sbclient,
                "Banana");

            if (errorType != sbservices_error_t.SBSERVICES_E_SUCCESS)
                throw new Exception("Error while we generate the image " + errorType);

            SBServices.sbservices_get_icon_state(sbclient, ref plist, "2");
            Plist.plist_to_xml(plist, out xml, ref length);


            return StringUtils.PtrToString(xml);
        }

        /// <summary>
        ///     Download the current wallpaper.
        /// </summary>
        /// <returns></returns>
        public unsafe string DownloadWallpaper()
        {
            var sbclient = new IntPtr();
            char* pngData = null;
            ulong pngSize = 0;
            var BundleID = new Random().Next().ToString();

            if (!Directory.Exists("cache"))
                Directory.CreateDirectory("cache");
            if (!Directory.Exists("cache\\wallpapers"))
                Directory.CreateDirectory("cache\\wallpapers");

            var errorType = SBServices.sbservices_client_start_service(myDevice, ref sbclient,
                "Banana");

            if (errorType != sbservices_error_t.SBSERVICES_E_SUCCESS)
                throw new Exception("Error while we generate the image " + errorType);

            SBServices.sbservices_get_home_screen_wallpaper_pngdata(sbclient, ref pngData, ref pngSize);
            if (pngSize > 0)
            {
                var newData = new byte[pngSize];
                Marshal.Copy((IntPtr) pngData, newData, 0, (int) pngSize);
                var wr = new BinaryWriter(File.Create("cache\\wallpapers\\" + BundleID + ".png"));
                wr.Write(newData);
                wr.Close();

                using (var input = File.OpenRead("cache\\wallpapers\\" + BundleID + ".png"))
                {
                    using (var output = File.Create("cache\\wallpapers\\" + BundleID + ".clean.png"))
                    {
                        try
                        {
                            PNGDecrusher.Decrush(input, output);
                        }
                        catch (InvalidDataException)
                        {
                            // decrushing failed, either an invalid PNG or it wasn't crushed
                        }
                    }
                }
                File.Delete("cache\\wallpapers\\" + BundleID + ".clean.png");
            }
            else
            {
                return null;
            }

            return "cache\\wallpapers\\" + BundleID + ".png";
        }

        /// <summary>
        ///     Get a diagnostic from a specified service.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetDiagnostic(string type = "All")
        {
            IntPtr lockdownd = GenerateLockdowndService(), service, drc, plist, xml;

            service = new IntPtr();
            drc = new IntPtr();
            xml = new IntPtr();
            uint size = 0;
            plist = new IntPtr();

            if (
                Lockdownd.lockdownd_start_service(lockdownd,
                    StringUtils.StringToPtr(DiagnosticsRelay.DIAGNOSTICS_RELAY_SERVICE_NAME), ref service) !=
                lockdownd_error_t.LOCKDOWN_E_SUCCESS)
                throw new Exception("Can't start diagnostic service.");

            if (DiagnosticsRelay.diagnostics_relay_client_new(myDevice, service, ref drc) !=
                diagnostics_relay_error_t.DIAGNOSTICS_RELAY_E_SUCCESS)
                throw new Exception("Diagnostic fail on starts");

            if (DiagnosticsRelay.diagnostics_relay_request_diagnostics(drc, StringUtils.StringToPtr(type), ref plist) !=
                diagnostics_relay_error_t.DIAGNOSTICS_RELAY_E_SUCCESS)
                throw new Exception("Bad type");

            Plist.plist_to_xml(plist, out xml, ref size);

            return StringUtils.PtrToString(xml);
        }

        /// <summary>
        ///     Call to lockdownd to generate device name.
        /// </summary>
        private void GenerateDeviceName()
        {
            var str = new IntPtr();
            var lockdownd = GenerateLockdowndService();

            Lockdownd.lockdownd_get_device_name(lockdownd, ref str);

            CloseLockdownd(lockdownd);

            DeviceName = StringUtils.PtrToString(str);
        }

        private void GenerateJailbreakStatus()
        {
            var lockdowndPtr = GenerateLockdowndService();
            var afc2 = new IntPtr();

            var ex = Lockdownd.lockdownd_start_service(lockdowndPtr, StringUtils.StringToPtr("com.apple.afc2"), ref afc2);

            Jailbroken = ex == lockdownd_error_t.LOCKDOWN_E_SUCCESS;

            if (ex != lockdownd_error_t.LOCKDOWN_E_SUCCESS)
                Lockdownd.lockdownd_service_descriptor_free(afc2);

            CloseLockdownd(lockdowndPtr);
        }

        /// <summary>
        ///     Connect to device with Apple protocol
        /// </summary>
        /// <returns></returns>
        internal IntPtr GenerateLockdowndService()
        {
            var lockdowndPtr = new IntPtr();
            lockdownd_error_t ex;
            do
            {
                ex = Lockdownd.lockdownd_client_new_with_handshake
                (myDevice, ref lockdowndPtr,
                    StringUtils.StringToPtr("Banana.Core"));
            } while (ex == lockdownd_error_t.LOCKDOWN_E_PAIRING_DIALOG_PENDING);

            if (ex != lockdownd_error_t.LOCKDOWN_E_SUCCESS)
                throw new ErrorOnGenerateLockdowndService("Lockdownd exception: " + ex);


            return lockdowndPtr;
        }

        /// <summary>
        ///     Clear lockdownd service
        /// </summary>
        /// <param name="lockdownd"></param>
        internal void CloseLockdownd(IntPtr lockdownd)
        {
            Lockdownd.lockdownd_client_free(lockdownd);
        }
    }
}