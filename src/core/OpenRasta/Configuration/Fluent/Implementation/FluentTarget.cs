using OpenRasta.Configuration.MetaModel;
using OpenRasta.DI;
using OpenRasta.TypeSystem;

namespace OpenRasta.Configuration.Fluent.Implementation
{
    public class FluentTarget : IHas, IUses, IFluentTarget
    {
        readonly IMetaModelRepository _repository;
        readonly IDependencyResolver _resolver;

        public FluentTarget(IDependencyResolver resolver, IMetaModelRepository repository)
        {
            _resolver = resolver;
            _repository = repository;
        }

        public FluentTarget()
            : this(DependencyManager.GetService<IDependencyResolver>(), DependencyManager.GetService<IMetaModelRepository>())
        {
        }

        public IMetaModelRepository Repository
        {
            get { return _repository; }
        }

        public IDependencyResolver Resolver
        {
            get { return _resolver; }
        }

        public ITypeSystem TypeSystem
        {
            get { return _resolver.Resolve<ITypeSystem>(); }
        }
    }
}