using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using OpenBastard.Hosting.Iis7;
using OpenBastard.Hosting.Iis7.WorkerProcess;

namespace OpenBastard.Environments.Iis7
{
    public abstract class Iis7Environment : FileBasedAspNetEnvironment
    {
        // Iis7Server _server;
        static int _previousRandom;
        Process _process;
        Mutex _processMutex;

        public Iis7Environment()
            : base(GetRandom(short.MaxValue - 1))
        {
        }

        protected abstract bool Integrated { get; }

        public override sealed void Initialize()
        {
            string webConfigContent = GetWebConfig();
            var assemblyPaths = GetAssemblyPaths();
            CopyFilesToDestination(webConfigContent, assemblyPaths);

            string mutexName = Guid.NewGuid().ToString();
            _processMutex = new Mutex(true, mutexName);
            int siteId = GetRandom(128);
            var processStart = new ProcessStartInfo
                {
                    Arguments = string.Format(@"""{0}"" ""{1}"" ""{2}"" ""{3}"" ""{4}""", 
                                              TemporaryFolder.FullName, 
                                              Port, 
                                              siteId, 
                                              Integrated, 
                                              mutexName), 
                    FileName = GetWorkerProcessName(), 
                    UseShellExecute = false, 
                    RedirectStandardInput = true, 
                    RedirectStandardOutput = true, 
                    CreateNoWindow = true
                };
            //Debugger.Launch();
            _process = Process.Start(processStart);

            _process.StandardOutput.ReadLine();
        }

        protected override void Dispose(bool disposing)
        {
            if (_process != null)
            {
                _process.StandardInput.WriteLine();
                _process.WaitForExit();
            }
            base.Dispose(disposing);
        }

        protected override string[] GetAssemblyPaths()
        {
            return Directory.GetFiles(Path.GetDirectoryName(typeof(Iis7Environment).Assembly.Location));
        }

        protected override string GetWebConfig()
        {
            return IisConfigFiles.webconfig;
        }

        static int GetRandom(int maxValue)
        {
            return _previousRandom = new Random(_previousRandom).Next(maxValue);
        }

        static string GetWorkerProcess32()
        {
            return typeof(WorkerProcess32).Assembly.Location;
        }

        static string GetWorkerProcess64()
        {
            return typeof(WorkerProcess64).Assembly.Location;
        }

        string GetWorkerProcessName()
        {
            int pointerSize = Marshal.SizeOf(typeof(IntPtr));
            if (pointerSize == 8)
                return GetWorkerProcess64();
            return GetWorkerProcess32();
        }
    }
}