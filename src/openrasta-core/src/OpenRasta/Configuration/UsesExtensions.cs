using OpenRasta.Configuration.Fluent;
using OpenRasta.Configuration.MetaModel;
using OpenRasta.DI;
using OpenRasta.Pipeline;
using OpenRasta.Web.UriDecorators;

namespace OpenRasta.Configuration
{
    public static class UsesExtensions
    {
        /// <summary>
        /// Registers a custom dependency that can be used for leveraging dependency injection.
        /// </summary>
        /// <typeparam name="TService">The type of the service to register</typeparam>
        /// <typeparam name="TConcrete">The concrete type used when the service type is requested</typeparam>
        /// <param name="anchor"></param>
        /// <param name="lifetime">The lifetime of the object.</param>
        public static void CustomDependency<TService, TConcrete>(this IUses anchor, DependencyLifetime lifetime) where TConcrete : TService
        {
            var fluentTarget = (IFluentTarget)anchor;

            fluentTarget.Repository.CustomRegistrations.Add(new DependencyRegistrationModel(typeof(TService), typeof(TConcrete), lifetime));
        }

        /// <summary>
        /// Adds a contributor to the pipeline.
        /// </summary>
        /// <typeparam name="TPipeline">The type of the pipeline contributor to register.</typeparam>
        /// <param name="anchor"></param>
        public static void PipelineContributor<TPipeline>(this IUses anchor) where TPipeline : class, IPipelineContributor
        {
            anchor.Resolver.AddDependency<IPipelineContributor, TPipeline>();
        }

        /// <summary>
        /// Adds a URI decorator to process incoming URIs.
        /// </summary>
        /// <typeparam name="TDecorator">The type of the URI decorator.</typeparam>
        /// <param name="anchor"></param>
        public static void UriDecorator<TDecorator>(this IUses anchor) where TDecorator : class, IUriDecorator
        {
            var fluentTarget = (IFluentTarget)anchor;

            fluentTarget.Repository.CustomRegistrations.Add(new DependencyRegistrationModel(typeof(IUriDecorator), typeof(TDecorator), DependencyLifetime.Transient));
        }
    }
}