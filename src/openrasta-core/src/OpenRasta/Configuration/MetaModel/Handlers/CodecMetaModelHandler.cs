using System.Linq;
using OpenRasta.Codecs;
using OpenRasta.Collections;
using OpenRasta.TypeSystem;

namespace OpenRasta.Configuration.MetaModel.Handlers
{
    public class CodecMetaModelHandler : AbstractMetaModelHandler
    {
        readonly ICodecRepository _codecRepository;

        public CodecMetaModelHandler(ICodecRepository codecRepository)
        {
            _codecRepository = codecRepository;
        }

        public override void PreProcess(IMetaModelRepository repository)
        {
            foreach (var codec in repository.ResourceRegistrations.SelectMany(x => x.Codecs))
            {
                if (codec.MediaTypes.Count == 0)
                    codec.MediaTypes.AddRange(MediaTypeAttribute.Find(codec.CodecType).Select(x => new MediaTypeModel
                    {
                        MediaType = x.MediaType, 
                        Extensions = x.Extensions != null ? x.Extensions.ToList() : null
                    }));
                if (codec.MediaTypes.Count == 0)
                    throw new OpenRastaConfigurationException("The codec doesn't have any media type associated explicitly in the meta model and doesnt have any MediaType attribute.");
            }
        }

        public override void Process(IMetaModelRepository repository)
        {
            var codecRegistrations = from resource in repository.ResourceRegistrations
                                     let resourceAsIType = resource.ResourceKey as IType
                                     let isStrict = resource.IsStrictRegistration
                                     from codec in resource.Codecs
                                     from mediaType in codec.MediaTypes
                                     select new CodecRegistration(
                                         codec.CodecType, 
                                         resource.ResourceKey, 
                                         isStrict, 
                                         mediaType.MediaType, 
                                         mediaType.Extensions, 
                                         codec.Configuration, 
                                         false);
            foreach (var reg in codecRegistrations)
                _codecRepository.Add(reg);
        }
    }
}