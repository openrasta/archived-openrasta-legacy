using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenRasta.Hosting.AspNet
{
    public abstract class Iis
    {
        public abstract IEnumerable<HttpHandlerRegistration> Handlers { get; }

        public bool IsHandlerAlreadyRegisteredForRequest(string httpVerb, Uri requestUri)
        {
            return Handlers.Any(x => x.Matches(httpVerb, requestUri));
        }

        public bool IsHandlerRegistrationValid(HttpHandlerRegistration registration)
        {
            return !string.IsNullOrEmpty(registration.Path) && registration.Path != "*" && !registration.Type.Contains(typeof(DefaultHttpHandler).FullName);
        }
    }
}