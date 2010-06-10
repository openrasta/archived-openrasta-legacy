using System;
using System.Collections.Generic;
using OpenRasta.Configuration;
using OpenRasta.DI;
using OpenRasta.Diagnostics;
using OpenRasta.Pipeline;
using OpenRasta.Web;

namespace OpenRasta.Hosting
{
    public class HostManager : IDisposable
    {
        static readonly IDictionary<IHost, HostManager> _registrations = new Dictionary<IHost, HostManager>();
        readonly object _syncRoot = new object();

        static HostManager()
        {
            Log = DependencyManager.IsAvailable
                ? DependencyManager.GetService<ILogger>()
                : new TraceSourceLogger();
        }

        HostManager(IHost host)
        {
            Host = host;
            Host.Start += HandleHostStart;
            Host.IncomingRequestReceived += HandleHostIncomingRequestReceived;
            Host.IncomingRequestProcessed += HandleIncomingRequestProcessed;
        }

        public IHost Host { get; private set; }
        public bool IsConfigured { get; private set; }

        public IDependencyResolver Resolver { get; private set; }
        static ILogger Log { get; set; }

        public static HostManager RegisterHost(IHost host)
        {
            if (host == null) throw new ArgumentNullException("host");

            Log.WriteInfo("Registering host of type {0}", host.GetType());

            var manager = new HostManager(host);

            lock (_registrations)
                _registrations.Add(host, manager);
            return manager;
        }

        public static void UnregisterHost(IHost host)
        {
            Log.WriteInfo("Unregistering host of type {0}", host.GetType());
            HostManager managerToDispose = null;
            lock (_registrations)
            {
                if (_registrations.ContainsKey(host))
                {
                    managerToDispose = _registrations[host];
                    _registrations.Remove(host);
                }
            }
            if (managerToDispose != null)
                managerToDispose.Dispose();
        }

        public void InvalidateConfiguration()
        {
            lock (_syncRoot)
            {
                IsConfigured = false;
            }
        }

        public void SetupCommunicationContext(ICommunicationContext context)
        {
            Log.WriteDebug("Adding communication context data");
            Resolver.AddDependencyInstance<ICommunicationContext>(context, DependencyLifetime.PerRequest);
            Resolver.AddDependencyInstance<IRequest>(context.Request, DependencyLifetime.PerRequest);
            Resolver.AddDependencyInstance<IResponse>(context.Response, DependencyLifetime.PerRequest);
        }

        public void Dispose()
        {
            Host.Start -= HandleHostStart;
            Host.IncomingRequestReceived -= HandleHostIncomingRequestReceived;
            Host.IncomingRequestProcessed -= HandleIncomingRequestProcessed;
        }

        void AssignResolver()
        {
            Resolver = Host.ResolverAccessor != null
                           ? Host.ResolverAccessor.Resolver
                           : new InternalDependencyResolver();
            if (!Resolver.HasDependency<IDependencyResolver>())
                Resolver.AddDependencyInstance(typeof(IDependencyResolver), Resolver);
            Log.WriteDebug("Using dependency resolver of type {0}", Resolver.GetType());
        }

        void Configure()
        {
            IsConfigured = false;
            AssignResolver();
            ThreadScopedAction(() =>
            {
                RegisterRootDependencies();

                VerifyContextStoreRegistered();

                RegisterCoreDependencies();

                RegisterLeafDependencies();

                ExecuteConfigurationSource();

                IsConfigured = true;
            });
        }

        void ExecuteConfigurationSource()
        {
            if (Resolver.HasDependency<IConfigurationSource>())
            {
                var configSource = Resolver.Resolve<IConfigurationSource>();
                Log.WriteDebug("Using configuration source {0}", configSource.GetType());
                configSource.Configure();
            }
            else
            {
                Log.WriteDebug("Not using any configuration source.");
            }
        }

        void RegisterCoreDependencies()
        {
            var registrar =
                Resolver.ResolveWithDefault<IDependencyRegistrar>(() => new DefaultDependencyRegistrar());
            Log.WriteInfo("Using dependency registrar of type {0}.", registrar.GetType());
            registrar.Register(Resolver);
        }

        void RegisterLeafDependencies()
        {
            Log.WriteDebug("Registering host's leaf dependencies.");
            if (!Host.ConfigureLeafDependencies(Resolver))
                throw new OpenRastaConfigurationException("Leaf dependencies configuration by host has failed.");
        }

        void RegisterRootDependencies()
        {
            Log.WriteDebug("Registering host's root dependencies.");
            if (!Host.ConfigureRootDependencies(Resolver))
                throw new OpenRastaConfigurationException("Root dependencies configuration by host has failed.");
        }

        void ThreadScopedAction(Action action)
        {
            bool resolverSet = false;
            try
            {
                DependencyManager.SetResolver(Resolver);
                resolverSet = true;
                action();
            }
            finally
            {
                if (resolverSet)
                    DependencyManager.UnsetResolver();
            }
        }

        void VerifyConfiguration()
        {
            if (!IsConfigured)
                lock (_syncRoot)
                    if (!IsConfigured)
                        Configure();
        }

        void VerifyContextStoreRegistered()
        {
            if (!Resolver.HasDependency<IContextStore>())
                throw new OpenRastaConfigurationException("The host didn't register a context store.");
        }

        protected virtual void HandleHostIncomingRequestReceived(object sender, IncomingRequestEventArgs e)
        {
            VerifyConfiguration();
            Log.WriteDebug("Incoming host request for " + e.Context.Request.Uri);
            ThreadScopedAction(() =>
            {
                // register the required dependency in the web context
                var context = e.Context;
                SetupCommunicationContext(context);
                Resolver.AddDependencyInstance<IHost>(Host, DependencyLifetime.PerRequest);

                Resolver.Resolve<IPipeline>().Run(context);
            });
        }

        protected virtual void HandleHostStart(object sender, EventArgs e)
        {
            VerifyConfiguration();
        }

        protected virtual void HandleIncomingRequestProcessed(object sender, IncomingRequestProcessedEventArgs e)
        {
            Log.WriteDebug("Request finished.");
            ThreadScopedAction(() => Resolver.HandleIncomingRequestProcessed());
        }
    }
}
