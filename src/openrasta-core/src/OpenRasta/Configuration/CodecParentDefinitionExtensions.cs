using System;
using OpenRasta.Codecs;
using OpenRasta.Configuration.Fluent;

namespace OpenRasta.Configuration
{
    public static class CodecParentDefinitionExtensions
    {
        public static ICodecDefinition TranscodedBy<TCodec>(this ICodecParentDefinition parent)
            where TCodec : ICodec
        {
            return parent.TranscodedBy<TCodec>(null);
        }

        public static ICodecDefinition TranscodedBy(this ICodecParentDefinition parent, Type codecType)
        {
            return parent.TranscodedBy(codecType, null);
        }
    }
}