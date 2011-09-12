using System;
using OpenRasta.Codecs;

namespace OpenRasta.Configuration.Fluent
{
    public interface ICodecParentDefinition :INoIzObject
    {
        ICodecDefinition TranscodedBy<TCodec>(object configuration)
            where TCodec : ICodec;

        ICodecDefinition TranscodedBy(Type type, object configuration);
    }
}