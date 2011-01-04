using System;
using OpenRasta.Configuration.Fluent;

namespace OpenRasta.Configuration
{
    public static class LegacyHasExtensions
    {
        [Obsolete("The syntax has changed. Please use .And.AtUri instead.")]
        public static IUriDefinition AndAt(this IUriDefinition uriDefinition, string uri)
        {
            return uriDefinition.And.AtUri(uri);
        }

        [Obsolete("The syntax has changed. Please use .ForMediaType instead.")]
        public static ICodecWithMediaTypeDefinition AndForMediaType(this ICodecWithMediaTypeDefinition mediaTypeDef, string mediaType)
        {
            return mediaTypeDef.ForMediaType(mediaType);
        }

        [Obsolete("The syntax has changed. Please use .And.TranscodedBy<> instead.")]
        public static ICodecDefinition AndTranscodedBy<TCodec>(this IHandlerForResourceWithUriDefinition handlerDef, object parameter)
        {
            return handlerDef.TranscodedBy(typeof(TCodec), parameter);
        }

        [Obsolete("The syntax has changed. Please use .And.TranscodedBy<> instead.")]
        public static ICodecDefinition AndTranscodedBy<TCodec>(this IHandlerForResourceWithUriDefinition handlerDef)
        {
            return handlerDef.TranscodedBy(typeof(TCodec));
        }
    }
}