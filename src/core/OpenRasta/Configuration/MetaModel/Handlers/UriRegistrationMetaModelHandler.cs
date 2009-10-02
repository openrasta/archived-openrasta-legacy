using OpenRasta.Web;

namespace OpenRasta.Configuration.MetaModel.Handlers
{
    public class UriRegistrationMetaModelHandler : AbstractMetaModelHandler
    {
        readonly IUriResolver _uriResolver;

        public UriRegistrationMetaModelHandler(IUriResolver uriResolver)
        {
            _uriResolver = uriResolver;
        }

        public override void Process(IMetaModelRepository repository)
        {
            foreach (var resource in repository.ResourceRegistrations)
                foreach (var uriRegistration in resource.Uris)
                    _uriResolver.AddUriMapping(uriRegistration.Uri, 
                                               resource.ResourceKey, 
                                               uriRegistration.Language, 
                                               uriRegistration.Name);
        }
    }
}