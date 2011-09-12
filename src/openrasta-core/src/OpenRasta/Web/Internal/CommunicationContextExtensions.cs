using System;

namespace OpenRasta.Web.Internal
{
    public static class CommunicationContextExtensions
    {
        public static Uri GetRequestUriRelativeToRoot(this ICommunicationContext context)
        {
            return context.ApplicationBaseUri
                .EnsureHasTrailingSlash()
                .MakeRelativeUri(context.Request.Uri)
                .MakeAbsolute("http://localhost");
        }
        
    }
}