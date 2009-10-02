using System;
using System.IO;

namespace OpenBastard.Environments.Iis7
{
    public abstract class FileBasedAspNetEnvironment : HttpWebRequestEnvironment
    {
        DirectoryInfo _tempFolder;

        public FileBasedAspNetEnvironment(int port)
            : base(port)
        {
        }

        ~FileBasedAspNetEnvironment()
        {
            if (!Disposed)
                Dispose(false);
        }

        public DirectoryInfo TemporaryFolder
        {
            get { return _tempFolder == null ? (_tempFolder = GetTempFolder()) : _tempFolder; }
            set { _tempFolder = value; }
        }

        protected bool Disposed { get; set; }

        public override void Dispose()
        {
            if (!Disposed)
            {
                GC.SuppressFinalize(this);
                Dispose(true);
            }
        }

        protected void CopyFilesToDestination(string webConfigContent, string[] assemblyPaths)
        {
            // copy web.config
            File.WriteAllText(Path.Combine(TemporaryFolder.FullName, "web.config"), webConfigContent);

            string destinationPath = Path.Combine(TemporaryFolder.FullName, "Bin");
            if (!Directory.Exists(destinationPath))
                Directory.CreateDirectory(destinationPath);
            foreach (string file in assemblyPaths)
                File.Copy(file, Path.Combine(destinationPath, Path.GetFileName(file)));
        }

        protected virtual void Dispose(bool disposing)
        {
            Disposed = true;
            try
            {
                _tempFolder.Delete(true);
            }
            catch
            {
            }
        }

        protected abstract string[] GetAssemblyPaths();

        protected DirectoryInfo GetTempFolder()
        {
            string folder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            return Directory.CreateDirectory(folder);
        }

        protected abstract string GetWebConfig();
    }
}