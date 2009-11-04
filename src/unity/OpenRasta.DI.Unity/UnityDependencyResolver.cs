using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using OpenRasta.DI.Unity.Extensions;
using OpenRasta.Pipeline;
using System.Diagnostics;

namespace OpenRasta.DI.Unity
{
    /// <summary>
    /// Adapts the Unity DI container for use with OpenRasta.
    /// </summary>
    /// <remarks>
    /// If you provide a parent container, the resolver will create a child container which
    /// OpenRasta uses to resolve all dependencies.  The behaviour of the child container is
    /// modified to suit OpenRasta:
    /// 
    /// - Constructor selection.
    /// - Property injection.
    /// - Circular dependencies.
    /// - Required registration.
    /// 
    /// The parent container you provide must have the <see cref="TypeTracker"/> extension installed
    /// before any types are registered otherwise it will not work correctly with OpenRasta.
    /// </remarks>
    public class UnityDependencyResolver : DependencyResolverCore, IDependencyResolver
    {
        const string ContextKey = "OpenRasta.DI.Unity.UnityDependencyResolver.ContextKey";

        /// <summary>
        /// Our primary container.  Parents may be provided by the user and children will come and
        /// go per request but this is the one container we can rely on.
        /// </summary>
        readonly IUnityContainer container;

        /// <summary>
        /// Gets the primary container used by the resolver.
        /// </summary>
        /// <remarks>
        /// Access to the container is provided so that users can customise its behaviour to suit.
        /// A typical example would be adding interception to built types.
        /// </remarks>
        public IUnityContainer Container
        {
            get { return container; }
        }

        /// <summary>
        /// Creates a new instance with a new root container.
        /// </summary>
        public UnityDependencyResolver()
        {
            container = CreateContainer();
        }

        /// <summary>
        /// Creates a new instance as a child of the given parent container.
        /// </summary>
        /// <remarks>
        /// The parent container must have the <see cref="TypeTracker"/> extension installed.
        /// </remarks>
        public UnityDependencyResolver(IUnityContainer parent)
        {
            container = CreateChildContainer(parent);
        }

        static IUnityContainer CreateContainer()
        {
            var container = new UnityContainer();
            Extend(container);
            return container;
        }

        static IUnityContainer CreateChildContainer(IUnityContainer parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            if (parent.TypeTracker() == null)
                throw new ArgumentException("Parent container does not have the TypeTracker extension installed.");

            var child = parent.CreateChildContainer();
            Extend(child);
            return child;
        }

        static void Extend(IUnityContainer container)
        {
            container.AddExtension(new TypeTracker());
            container.AddExtension(new CycleDetector());
            container.AddExtension(new InjectionPolicies());
            container.AddExtension(new TypeRegistrationRequired());
        }

        public bool HasDependency(Type serviceType)
        {
            if (serviceType == null)
                return false;

            return OptionalContextContainer()
                .TypeTracker()
                .HasDependency(serviceType);
        }

        public bool HasDependencyImplementation(Type serviceType, Type concreteType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            if (concreteType == null)
                throw new ArgumentNullException("concreteType");

            return OptionalContextContainer()
                .TypeTracker()
                .HasDependencyImplementation(serviceType, concreteType);
        }

        public void HandleIncomingRequestProcessed()
        {
            if (!container.TypeTracker().HasDependency(typeof(IContextStore)))
                return;

            // If we have a per request container, dispose and remove it
            var context = container.Resolve<IContextStore>();
            var current = (IUnityContainer)context[ContextKey];

            if(current == null)
                return;

            current.Dispose();
            context[ContextKey] = null;
        }

        protected override void AddDependencyCore(Type serviceType, Type concreteType, DependencyLifetime lifetime)
        {
            if (lifetime == DependencyLifetime.Transient)
            {
                // Unity registers types as transient by default.
                container.RegisterType(serviceType, concreteType);
            }
            else if (lifetime == DependencyLifetime.Singleton)
            {
                container.RegisterType(serviceType, concreteType, new ContainerControlledLifetimeManager());
            }
            else if (lifetime == DependencyLifetime.PerRequest)
            {
                OptionalContextContainer().RegisterType(serviceType, concreteType, new PerRequestLifetimeManager(this));
            }
            else
            {
                throw new NotSupportedException("lifetime");
            }
        }

        protected override void AddDependencyCore(Type concreteType, DependencyLifetime lifetime)
        {
            AddDependencyCore(concreteType, concreteType, lifetime);
        }

        protected override void AddDependencyInstanceCore(Type serviceType, object instance, DependencyLifetime lifetime)
        {
            if (lifetime == DependencyLifetime.Singleton)
            {
                // Unity registers instances as singletons by default
                container.RegisterInstance(serviceType, instance);
            }
            else if (lifetime == DependencyLifetime.PerRequest)
            {
                RequiredContextContainer().RegisterInstance(serviceType, instance, new PerRequestLifetimeManager(this));
            }
            else
            {
                throw new NotSupportedException("lifetime");
            }
        }

        /// <summary>
        /// Ensures that there is a child container for the current request and returns it.
        /// </summary>
        IUnityContainer RequiredContextContainer()
        {
            if (!container.TypeTracker().HasDependency(typeof(IContextStore)))
                throw new InvalidOperationException("There is no current per request context to register in.");

            var context = container.Resolve<IContextStore>();
            var current = (IUnityContainer)context[ContextKey];

            if (current == null)
            {
                current = CreateChildContainer(container);
                context[ContextKey] = current;
            }

            return current;
        }

        /// <summary>
        /// Returns the child container for this request if there is one, otherwise returns the
        /// primary container.
        /// </summary>
        IUnityContainer OptionalContextContainer()
        {
            if (!container.TypeTracker().HasDependency(typeof(IContextStore)))
                return container;

            var context = container.Resolve<IContextStore>();
            var current = (IUnityContainer)context[ContextKey];

            return current ?? container;
        }

        protected override IEnumerable<TService> ResolveAllCore<TService>()
        {
            var types = OptionalContextContainer()
                .TypeTracker()
                .GetAllConcreteTypes(typeof (TService));

            var current = OptionalContextContainer();
            var services = new List<TService>();

            foreach (var type in types)
            {
                services.Add((TService)current.Resolve(type));
            }

            return services;
        }

        protected override object ResolveCore(Type serviceType)
        {
            return OptionalContextContainer().Resolve(serviceType);
        }
    }
}
