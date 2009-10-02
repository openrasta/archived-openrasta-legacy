using OpenRasta.Handlers;

namespace OpenRasta.Configuration.MetaModel.Handlers
{
    public class HandlerMetaModelHandler : AbstractMetaModelHandler
    {
        readonly IHandlerRepository _repository;

        public HandlerMetaModelHandler(IHandlerRepository repository)
        {
            _repository = repository;
        }

        public override void Process(IMetaModelRepository repository)
        {
            foreach (var resource in repository.ResourceRegistrations)
                foreach (var handler in resource.Handlers)
                    _repository.AddResourceHandler(resource.ResourceKey, handler);
        }
    }
}