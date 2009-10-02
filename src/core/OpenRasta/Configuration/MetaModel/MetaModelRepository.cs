using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenRasta.Configuration.MetaModel.Handlers;
using OpenRasta.DI;

namespace OpenRasta.Configuration.MetaModel
{
    public class MetaModelRepository : IMetaModelRepository
    {
        readonly IMetaModelHandler[] _handlers;

        // TODO: Remove when impelemntation of array injection in containers is complete
        public MetaModelRepository(IDependencyResolver resolver) : this(resolver.ResolveAll<IMetaModelHandler>().ToArray())
        {
        }

        public MetaModelRepository(IMetaModelHandler[] handlers)
        {
            _handlers = handlers;
            ResourceRegistrations = new List<ResourceModel>();
            CustomRegistrations = new ArrayList();
        }

        public IList CustomRegistrations { get; set; }
        public IList<ResourceModel> ResourceRegistrations { get; set; }

        public void Process()
        {
            foreach (var handler in _handlers) handler.PreProcess(this);
            foreach (var handler in _handlers) handler.Process(this);
        }
    }
}