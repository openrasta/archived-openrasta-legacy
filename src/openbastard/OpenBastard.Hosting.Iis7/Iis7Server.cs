using System;
using System.IO;
using System.Runtime.InteropServices;

namespace OpenBastard.Hosting.Iis7
{
    public class Iis7Server : IDisposable
    {
        readonly string _appHostConfigPath;
        readonly string _rootWebConfigPath;
        bool _disposed;


        public Iis7Server(string physicalPath, int port, int siteId, bool useIntegratedPipeline)
        {
            string appPoolName = "AppPool" + port;
            _appHostConfigPath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName() + ".config");
            _rootWebConfigPath = Environment.ExpandEnvironmentVariables(@"%windir%\Microsoft.Net\Framework\v2.0.50727\config\web.config");

            File.WriteAllText(_appHostConfigPath, 
                              String.Format(IisConfigFiles.applicationHost, 
                                            port, 
                                            physicalPath, 
                                            siteId, 
                                            appPoolName, 
                                            useIntegratedPipeline ? "Integrated" : "Classic"));
        }

        ~Iis7Server()
        {
            Dispose(false);
        }

        public void Start()
        {
            CheckDisposed();
            HostableWebCore.Activate(_appHostConfigPath, _rootWebConfigPath, Guid.NewGuid().ToString());
        }

        public void Stop()
        {
            ((IDisposable)this).Dispose();
        }

        void IDisposable.Dispose()
        {
            CheckDisposed();
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException("The server has already been disposed");
        }

        void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                HostableWebCore.Shutdown(false);
            }
        }

        public static class HostableWebCore
        {
            static bool _isActivated;

            /// <summary>
            /// Specifies if Hostable WebCore ha been activated
            /// </summary>
            public static bool IsActivated
            {
                get { return _isActivated; }
            }

            // public void Activate(string appHostConfig, string rootWebConfig, string instanceName){}
            /// <summary>
            /// Activate the HWC
            /// </summary>
            /// <param name="appHostConfig">Path to ApplicationHost.config to use</param>
            /// <param name="rootWebConfig">Path to the Root Web.config to use</param>
            /// <param name="instanceName">Name for this instance</param>
            public static void Activate(string appHostConfig, string rootWebConfig, string instanceName)
            {
                if (_isActivated)
                    return;
                int result = HWebCore64.WebCoreActivate(appHostConfig, rootWebConfig, instanceName);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                _isActivated = true;
            }

            /// <summary>
            /// Shutdown HWC
            /// </summary>
            public static void Shutdown(bool immediate)
            {
                if (_isActivated)
                {
                    int result = HWebCore64.WebCoreShutdown(immediate);
                    if (result != 0)
                        Marshal.ThrowExceptionForHR(result);
                    _isActivated = false;
                }
            }

            static class HWebCore64
            {
                [DllImport(@"inetsrv\hwebcore.dll")]
                public static extern int WebCoreActivate(
                    [In, MarshalAs(UnmanagedType.LPWStr)] string appHostConfigPath, 
                    [In, MarshalAs(UnmanagedType.LPWStr)] string rootWebConfigPath, 
                    [In, MarshalAs(UnmanagedType.LPWStr)] string instanceName);

                [DllImport(@"inetsrv\hwebcore.dll")]
                public static extern int WebCoreShutdown(bool immediate);
            }
        }
    }
}