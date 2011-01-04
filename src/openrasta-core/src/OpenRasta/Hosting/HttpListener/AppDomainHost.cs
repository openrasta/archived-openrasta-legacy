using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using OpenRasta.Hosting.HttpListener;

namespace OpenRasta.Hosting
{
    public class AppDomainHost<T> : IDisposable
        where T : HttpListenerHost
    {
        readonly IEnumerable<string> _prefixes;
        readonly Type _resolver;
        readonly string _virtualDir;

        public AppDomainHost(IEnumerable<string> prefixes, string virtualDir, Type resolver)
        {
            _prefixes = prefixes;
            _virtualDir = virtualDir;
            _resolver = resolver;
        }

        ~AppDomainHost()
        {
            Dispose();
        }

        public AppDomain HostAppDomain { get; set; }
        public bool IsDisposed { get; private set; }
        public T Listener { get; protected set; }

        public void Initialize()
        {
            if (IsDisposed) throw new ObjectDisposedException("The controller has already been disposed.");
            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory, 
                ShadowCopyFiles = "true"
            };

            HostAppDomain = AppDomain.CreateDomain(Guid.NewGuid().ToString(), AppDomain.CurrentDomain.Evidence, appDomainSetup);

            Listener = (T)HostAppDomain.CreateInstanceAndUnwrap(typeof(T).Assembly.FullName, 
                                                      typeof(T).FullName, 
                                                      false, 
                                                      BindingFlags.Instance | BindingFlags.Public, 
                                                      null, 
                                                      null, 
                                                      CultureInfo.CurrentCulture, 
                                                      null, 
                                                      HostAppDomain.Evidence);
            Listener.Initialize(_prefixes, _virtualDir, _resolver);
        }

        public void StartListening()
        {
            if (IsDisposed) throw new ObjectDisposedException("The controller has already been disposed.");

            Listener.StartListening();
        }

        public void StopListening()
        {
            if (IsDisposed) throw new ObjectDisposedException("The controller has already been disposed.");

            Listener.StopListening();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                try
                {
                    Listener.Close();
                    AppDomain.Unload(HostAppDomain);
                }
                catch
                {
                }
            }
            if (disposing)
            {
                HostAppDomain = null;
            }
        }
    }
}