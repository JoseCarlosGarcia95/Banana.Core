using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Banana.Core.Libraries.MobileDevice;
using Banana.Core.Libraries.StringUtils;

namespace Banana.Core.Stream
{
    public class DeviceFile
    {
        #region Constructor

        /// <summary>
        ///     Create a new deviicefile.
        /// </summary>
        /// <param name="conn">Device connection.</param>
        /// <param name="path">Path to the file.</param>
        /// <param name="mode">File access mode.</param>
        public DeviceFile(Connection.Connection conn, string path, afc_file_mode_t mode)
        {
            var afc = new IntPtr();
            _afcClient = new IntPtr();
            var lockdownd = conn.GenerateLockdowndService();

            Lockdownd.lockdownd_start_service(lockdownd,
                StringUtils.StringToPtr(conn.Jailbroken ? "com.apple.afc2" : "com.apple.afc"), ref afc);
            AFC.afc_client_new(conn.myDevice, afc, ref _afcClient);

            AFC.afc_file_open(_afcClient, path, mode, ref _handle);
            Lockdownd.lockdownd_service_descriptor_free(afc);
            conn.CloseLockdownd(lockdownd);

            _deviceFileInfo = new Dictionary<string, string>();
            GenerateDeviceFileInfo(path);
        }

        #endregion

        #region Private

        /// <summary>
        ///     Easy function to make accesible information from DeviceFile
        /// </summary>
        /// <param name="path"></param>
        private void GenerateDeviceFileInfo(string path)
        {
            var i = 0;
            var fileInfo = new List<string>();

            unsafe
            {
                string dirName;
                void** fInfo = null;
                AFC.afc_get_file_info(_afcClient, path, ref fInfo);

                while ((fInfo[i] != null) && ((dirName = StringUtils.PtrToString((IntPtr) fInfo[i++])) != ""))
                    fileInfo.Add(dirName);
            }

            for (i = 0; i < fileInfo.Count; i = i + 2)
                _deviceFileInfo.Add(fileInfo[i], fileInfo[i + 1]);
        }

        #endregion

        #region Close function

        /// <summary>
        ///     Close a file to unlock it.
        /// </summary>
        public void Close()
        {
            AFC.afc_file_close(_afcClient, _handle);
            AFC.afc_client_free(_afcClient);
        }

        #endregion

        #region Private

        /// <summary>
        ///     File handle ID.
        /// </summary>
        private readonly ulong _handle;

        /// <summary>
        ///     AFC client type.
        /// </summary>
        private readonly IntPtr _afcClient;

        /// <summary>
        ///     An array with device file info.
        /// </summary>
        private readonly Dictionary<string, string> _deviceFileInfo;

        #endregion

        #region Output functions

        /// <summary>
        ///     Get content from start to end.
        /// </summary>
        /// <returns></returns>
        public byte[] ReadToEnd()
        {
            var data = Marshal.AllocHGlobal((int) Length);
            uint bytesRead = 0;
            AFC.afc_file_read(_afcClient, _handle, data, Length, out bytesRead);

            var managedArray = new byte[bytesRead];
            Marshal.Copy(data, managedArray, 0, (int) bytesRead);

            Marshal.FreeHGlobal(data);
            return managedArray;
        }


        /// <summary>
        ///     Write information to device.
        /// </summary>
        /// <param name="s"></param>
        public void Write(byte[] s)
        {
            Write(Encoding.UTF8.GetString(s));
        }

        public void Write(string s)
        {
            uint bytesWritten = 0;
            AFC.afc_file_write(_afcClient, _handle, StringUtils.StringToPtr(s), (uint) s.Length, ref bytesWritten);

            if (bytesWritten != s.Length)
                throw new Exception("An error happen! You have tried to write " + s.Length + " bytes and only " +
                                    bytesWritten + " bytes were written");
        }


        /// <summary>
        ///     Useful function to get content from a file
        /// </summary>
        /// <param name="offset">Start point</param>
        /// <param name="end">End point</param>
        /// <returns></returns>
        public byte[] Read(uint start, uint end)
        {
            AFC.afc_file_seek(_afcClient, _handle, start, 0);

            var data = Marshal.AllocHGlobal((int) Length);
            uint bytesRead = 0;
            AFC.afc_file_read(_afcClient, _handle, data, end - start, out bytesRead);

            var managedArray = new byte[bytesRead];
            Marshal.Copy(data, managedArray, 0, (int) bytesRead);

            Marshal.FreeHGlobal(data);

            return managedArray;
        }

        /// <summary>
        ///     Set file pointer to 'pos'
        /// </summary>
        /// <param name="pos"></param>
        public void SetPosition(int pos)
        {
            AFC.afc_file_seek(_afcClient, _handle, pos, 0);
        }

        /// <summary>
        ///     Reset file pointer.
        /// </summary>
        public void Rewind()
        {
            AFC.afc_file_seek(_afcClient, _handle, 0, 0);
        }

        #endregion

        #region File information 'get' functions

        /// <summary>
        ///     File size.
        /// </summary>
        public uint Length
        {
            get { return Convert.ToUInt32(_deviceFileInfo["st_size"]); }
        }

        /// <summary>
        ///     File creation time.
        /// </summary>
        public DateTime CreationTime
        {
            get
            {
                var unix =
                    Convert.ToUInt64(_deviceFileInfo["st_birthtime"].Substring(0,
                        _deviceFileInfo["st_birthtime"].Length - 9));
                var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(unix).ToLocalTime();
                return dtDateTime;
            }
        }

        /// <summary>
        ///     Last modified time.
        /// </summary>
        public DateTime LastModifiedTime
        {
            get
            {
                var unix =
                    Convert.ToUInt64(_deviceFileInfo["st_mtime"].Substring(0, _deviceFileInfo["st_mtime"].Length - 9));
                var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(unix).ToLocalTime();
                return dtDateTime;
            }
        }

        #endregion

        #region Static functions

        /// <summary>
        ///     Copy a file from your computer to your device.
        /// </summary>
        /// <param name="hostPath"></param>
        /// <param name="devicePath"></param>
        public static void CopyFileFromHostToDevice(Connection.Connection conn, string hostPath, string devicePath)
        {
            string fileData;
            var rd = new StreamReader(hostPath, Encoding.Default, true);
            fileData = rd.ReadToEnd();
            rd.Close();


            var file = new DeviceFile(conn, devicePath, afc_file_mode_t.AFC_FOPEN_APPEND);
            file.Write(fileData);
            file.Close();
        }


        /// <summary>
        ///     Copy a file from your device to your computer.
        /// </summary>
        /// <param name="hostPath"></param>
        /// <param name="devicePath"></param>
        public static void CopyfileFromDeviceToHost(Connection.Connection conn, string hostPath, string devicePath)
        {
            var file = new DeviceFile(conn, devicePath, afc_file_mode_t.AFC_FOPEN_RDONLY);
            var fileData = file.ReadToEnd();
            file.Close();

            var wr = new BinaryWriter(new StreamWriter(hostPath).BaseStream);
            wr.Write(fileData);
            wr.Close();
        }

        #endregion
    }
}