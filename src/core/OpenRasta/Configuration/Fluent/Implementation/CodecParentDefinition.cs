using System;
using OpenRasta.Codecs;

namespace OpenRasta.Configuration.Fluent.Implementation
{
    public class CodecParentDefinition : ICodecParentDefinition
    {
        readonly ResourceDefinition _resourceDefinition;

        public CodecParentDefinition(ResourceDefinition registration)
        {
            _resourceDefinition = registration;
        }

        public ICodecDefinition TranscodedBy<TCodec>(object configuration) where TCodec : ICodec
        {
            return _resourceDefinition.TranscodedBy<TCodec>(configuration);
        }

        public ICodecDefinition TranscodedBy(Type type, object configuration)
        {
            return _resourceDefinition.TranscodedBy(type, configuration);
        }
    }
}