using Microsoft.Practices.Unity;
using OpenRasta.Configuration;
using OpenRasta.DI.Unity.Extensions;
using OpenRasta.DI.Unity.Sample.Domain;
using OpenRasta.DI.Unity.Sample.Resources;
using OpenRasta.Diagnostics;

namespace OpenRasta.DI.Unity.Sample
{
    /// <summary>
    /// OpenRasta will search for this class at initialization time and use it to find our Unity
    /// container.  OpenRasta will then look for our Bootstrapper and call Configure below.
    /// </summary>
    public class ResolverAccessor : IDependencyResolverAccessor
    {
        IDependencyResolver IDependencyResolverAccessor.Resolver
        {
            get { return Bootstrapper.Resolver; }
        }
    }

    public class Bootstrapper : IConfigurationSource
    {
        public static IUnityContainer Container { get; private set; }
        public static IDependencyResolver Resolver { get; private set; }

        static Bootstrapper()
        {
            // Create our root container including the TypeTracker extension which OpenRasta
            // requires.  The extension doesn't change any behaviour for this container, but it
            // enables OpenRastas child container to do what it needs.
            Container = new UnityContainer();
            Container.AddNewExtension<TypeTracker>();

            // This is where you register all your normal application services that are designed to
            // work with Unity and its default injection policies.
            Container.RegisterType<IInfoProvider, InfoProvider>();

            // Create the resolver that OpenRasta will use for all dependencies.  This actually
            // creates a child container so it doesn't intefere with our application level
            // behaviour.
            Resolver = new UnityDependencyResolver(Container);
        }

        public void Configure()
        {
            // Standard OpenRasta configuration.
            using (OpenRastaConfiguration.Manual)
            {
                // Add our handler, which depends on our application server IInfoProvider.
                ResourceSpace.Has
                    .ResourcesOfType<Info>()
                    .AtUri("/Info")
                    .HandledBy<InfoHandler>()
                    .AsXmlSerializer();

                // OpenRasta registers its default implementations in the child container which
                // takes precendence over our root container.  If to override one of OpenRastas
                // default implementations you need to register it here, using ResourceSpace.
                ResourceSpace.Uses
                    .CustomDependency<ILogger, CustomLogger>(DependencyLifetime.PerRequest);
            }
        }
    }
}
