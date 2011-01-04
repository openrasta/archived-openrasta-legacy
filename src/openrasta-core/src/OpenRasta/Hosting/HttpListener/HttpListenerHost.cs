using System;
using System.Collections.Generic;
using System.Net;
using OpenRasta.DI;
using OpenRasta.Pipeline;

namespace OpenRasta.Hosting.HttpListener
{
    public class HttpListenerHost : MarshalByRefObject, IHost, IDisposable
    {
        bool _isDisposed;
        System.Net.HttpListener _listener;
        IDependencyResolverAccessor _resolverAccessor;
        Type _resolverFactory;

        ~HttpListenerHost()
        {
            Dispose(false);
        }

        public event EventHandler<IncomingRequestProcessedEventArgs> IncomingRequestProcessed = (s, e) => { };
        public event EventHandler<IncomingRequestReceivedEventArgs> IncomingRequestReceived = (s, e) => { };

        public event EventHandler Start = (s, e) => { };
        public event EventHandler Stop = (s, e) => { };
        public string ApplicationVirtualPath { get; private set; }

        public IDependencyResolverAccessor ResolverAccessor
        {
            get
            {
                if (_resolverFactory != null && _resolverAccessor == null)
                {
                    _resolverAccessor = (IDependencyResolverAccessor)Activator.CreateInstance(_resolverFactory);
                }

                return _resolverAccessor;
            }
        }

        public void Close()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Initialize(IEnumerable<string> prefixes, string appPathVDir, Type dependencyResolverFactory)
        {
            CheckNotDisposed();
            ApplicationVirtualPath = appPathVDir;

            _resolverFactory = dependencyResolverFactory;
            _listener = new System.Net.HttpListener();
            foreach (string prefix in prefixes)
                _listener.Prefixes.Add(prefix);
            HostManager.RegisterHost(this);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void ProcessRequest(IAsyncResult result)
        {
            if (_isDisposed)
                return;
            HttpListenerContext nativeContext;
            try
            {
                nativeContext = _listener.EndGetContext(result);
            }
            catch (HttpListenerException)
            {
                return;
            }
            QueueNextRequestPending();
            var ambientContext = new AmbientContext();
            var context = new HttpListenerCommunicationContext(this, nativeContext);
            try
            {
                using (new ContextScope(ambientContext))
                {
                    IncomingRequestReceived(this, new IncomingRequestReceivedEventArgs(context));
                }
            }
            finally
            {
                using (new ContextScope(ambientContext))
                {
                    IncomingRequestProcessed(this, new IncomingRequestProcessedEventArgs(context));
                }
            }
        }

        public void StartListening()
        {
            CheckNotDisposed();
            using (new ContextScope(new AmbientContext()))
            {
                Start(this, EventArgs.Empty);
            }
            _listener.Start();
            QueueNextRequestPending();
        }

        public void StopListening()
        {
            CheckNotDisposed();

            using (new ContextScope(new AmbientContext()))
            {
                Stop(this, EventArgs.Empty);
            }
            _listener.Stop();
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        public virtual bool ConfigureLeafDependencies(IDependencyResolver resolver)
        {
            return true;
        }

        public virtual bool ConfigureRootDependencies(IDependencyResolver resolver)
        {
            resolver.AddDependency<IContextStore, AmbientContextStore>(DependencyLifetime.Singleton);
            return true;
        }

        protected virtual void Dispose(bool fromDisposeMethod)
        {
            if (!_isDisposed)
            {
                if (fromDisposeMethod)
                {
                    if (_listener.IsListening)
                        StopListening();
                    HostManager.UnregisterHost(this);
                }
                _listener.Abort();
                _isDisposed = true;
            }
        }

        void CheckNotDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("HttpListenerHost");
        }

        void QueueNextRequestPending()
        {
            _listener.BeginGetContext(ProcessRequest, null);
        }
    }
}