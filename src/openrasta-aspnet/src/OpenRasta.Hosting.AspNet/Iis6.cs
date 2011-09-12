using System.Collections.Generic;
using System.Web.Configuration;

namespace OpenRasta.Hosting.AspNet
{
    public class Iis6 : Iis
    {
        public override IEnumerable<HttpHandlerRegistration> Handlers
        {
            get
            {
                var httpHandlers = (HttpHandlersSection)WebConfigurationManager.GetSection("system.web/httpHandlers");
                foreach (HttpHandlerAction section in httpHandlers.Handlers)
                {
                    var handler = new HttpHandlerRegistration(section.Verb, section.Path, section.Type);
                    if (IsHandlerRegistrationValid(handler))
                        yield return handler;
                }
            }
        }
    }
}