using System;
using System.Linq;
using OpenRasta.DI;

namespace OpenRasta.Configuration.MetaModel.Handlers
{
    public class DependencyRegistrationMetaModelHandler : AbstractMetaModelHandler
    {
        readonly IDependencyResolver _resolver;

        public DependencyRegistrationMetaModelHandler(IDependencyResolver resolver)
        {
            _resolver = resolver;
        }

        public override void PreProcess(IMetaModelRepository repository)
        {
            foreach (var model in repository.CustomRegistrations.OfType<DependencyRegistrationModel>())
                _resolver.AddDependency(model.ServiceType, model.ConcreteType, model.Lifetime);
        }
    }
}