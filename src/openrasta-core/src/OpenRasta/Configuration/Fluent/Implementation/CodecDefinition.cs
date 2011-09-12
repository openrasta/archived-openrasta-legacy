using System;
using OpenRasta.Configuration.MetaModel;
using OpenRasta.Web;

namespace OpenRasta.Configuration.Fluent.Implementation
{
    public class CodecDefinition : ICodecDefinition
    {
        readonly CodecModel _codecRegistration;

        public CodecDefinition(ResourceDefinition resourceDefinition, Type codecType, object configuration)
        {
            ResourceDefinition = resourceDefinition;
            _codecRegistration = new CodecModel(codecType, configuration);
            ResourceDefinition.Registration.Codecs.Add(_codecRegistration);
        }

        public ICodecParentDefinition And
        {
            get { return ResourceDefinition; }
        }

        public ResourceDefinition ResourceDefinition { get; set; }

        public ICodecWithMediaTypeDefinition ForMediaType(MediaType mediaType)
        {
            var model = new MediaTypeModel { MediaType = mediaType };
            _codecRegistration.MediaTypes.Add(model);

            return new CodecMediaTypeDefinition(this, model);
        }
    }
}