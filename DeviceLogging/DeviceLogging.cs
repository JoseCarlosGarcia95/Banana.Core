using System;
using Banana.Core.Libraries.MobileDevice;
using Banana.Core.Libraries.MobileDevice.Callbacks;
using Banana.Core.Libraries.StringUtils;

namespace Banana.Core.DeviceLogging
{
    public class DeviceLogging
    {
        private readonly Connection.Connection _connection;

        private DeviceSyslogCallback_t _listener;

        private IntPtr _loggingService;

        /// <summary>
        ///     Starts logging every event that happens in your iDevice
        /// </summary>
        /// <param name="connection">Your device identification.</param>
        public DeviceLogging(Connection.Connection connection)
        {
            _connection = connection;
        }

        /// <summary>
        ///     Starts logging every event that happens in your iDevice
        /// </summary>
        /// <param name="connection">Your device identification.</param>
        /// <param name="listener">The function where the logging are going to be shown.</param>
        public DeviceLogging(Connection.Connection connection, DeviceSyslogCallback_t listener)
        {
            _connection = connection;
            SetListener(listener);
        }

        /// <summary>
        ///     Set a listener.
        /// </summary>
        /// <param name="listener"></param>
        public void SetListener(DeviceSyslogCallback_t listener)
        {
            _listener = listener;
        }

        /// <summary>
        ///     Start logging interface.
        /// </summary>
        public void Start()
        {
            if (_listener == null)
                throw new Exception("You have not initialize any listener.");

            IntPtr lockdownd = _connection.GenerateLockdowndService(),
                syslog_service = new IntPtr(),
                client = new IntPtr(),
                shit = new IntPtr();

            Console.WriteLine("OK1");
            if (Lockdownd.lockdownd_start_service(lockdownd, StringUtils.StringToPtr("com.apple.syslog_relay"),
                    ref syslog_service) != lockdownd_error_t.LOCKDOWN_E_SUCCESS)
                throw new Exception("Service \"com.apple.syslog_relay\" couldn't be started");

            Console.WriteLine("OK2");

            if (Syslog.syslog_relay_client_new(_connection.myDevice, syslog_service, ref client) !=
                syslog_relay_error_t.SYSLOG_RELAY_E_SUCCESS)
                throw new Exception("Syslog couldn't be started");

            Syslog.syslog_relay_start_capture(client, _listener, out shit);
            _loggingService = client;
        }

        /// <summary>
        ///     Stop logging interface.
        /// </summary>
        public void Stop()
        {
            Syslog.syslog_relay_stop_capture(_loggingService);
        }
    }
}