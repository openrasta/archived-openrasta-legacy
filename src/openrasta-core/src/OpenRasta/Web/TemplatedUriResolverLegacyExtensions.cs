using System;
using System.Globalization;

namespace OpenRasta.Web
{
    public static class TemplatedUriResolverLegacyExtensions
    {
        [Obsolete("Please use the Add method.")]
        public static void AddUriMapping(this IUriResolver resolver, string uri, object resourceKey, CultureInfo ci, string uriName)
        {
            resolver.Add(new UriRegistration(uri, resourceKey, uriName, ci));
        }
    }
}