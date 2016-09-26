using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Banana.Core.Properties;

namespace Banana.Core
{
    public class BananaManager
    {
        #region Constructor

        /// <summary>
        ///     Start an iteration with Banana.Core
        /// </summary>
        public BananaManager()
        {
            // Import resources
            ImportResources();

            // Load banana libs
            LoadBananaLibs();
        }

        #endregion

        #region Privates

        /// <summary>
        ///     Current release name. * Useful for new releases *
        /// </summary>
        private const string BananaCoreRelease = "PeelBanana2";

        /// <summary>
        ///     Banana core resources path.
        /// </summary>
        protected string BananaCoreFolder =
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Banana.Core";

        #endregion

        #region Methods

        #region External resources engine

        /// <summary>
        ///     Import libraries from resources.
        /// </summary>
        private void ImportResources()
        {
            if (!Directory.Exists(BananaCoreFolder + "\\libs\\" + BananaCoreRelease))
                Directory.CreateDirectory(BananaCoreFolder + "\\libs\\" + BananaCoreRelease);

            var resourceSet = Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry entry in resourceSet)
                if (entry.Value is byte[])
                {
                    var resource = (byte[]) entry.Value;

                    if (!File.Exists(BananaCoreFolder + "\\libs\\" + BananaCoreRelease + "\\" + entry.Key + ".dll"))
                        File.WriteAllBytes(
                            BananaCoreFolder + "\\libs\\" + BananaCoreRelease + "\\" + entry.Key + ".dll",
                            resource);
                    else
                    {
                        if (MD5Check(resource) !=
                            MD5Check(
                                File.ReadAllBytes(BananaCoreFolder + "\\libs\\" + BananaCoreRelease + "\\" + entry.Key +
                                                  ".dll")))
                            File.WriteAllBytes(
                                BananaCoreFolder + "\\libs\\" + BananaCoreRelease + "\\" + entry.Key + ".dll",
                                resource);
                    }
                }
                else if (entry.Value is string)
                {
                    if (!File.Exists(BananaCoreFolder + "\\libs\\" + BananaCoreRelease + "\\" + entry.Key + ".txt"))
                        File.AppendAllText(
                            BananaCoreFolder + "\\libs\\" + BananaCoreRelease + "\\" + entry.Key + ".txt",
                            (string) entry.Value);
                }
        }

        #endregion

        #region Library engine

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);

        /// <summary>
        ///     Load external libraries from banana.core folder
        /// </summary>
        private void LoadBananaLibs()
        {
            SetDllDirectory(BananaCoreFolder + "\\libs\\" + BananaCoreRelease);
        }

        private string MD5Check(byte[] src, bool upperCase = false)
        {
            var bytes = new byte[0];
            using (var md5 = MD5.Create())
            {
                bytes = md5.ComputeHash(src);
            }
            var result = new StringBuilder(bytes.Length*2);

            for (var i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));

            return result.ToString();
        }

        #endregion

        #endregion
    }
}