using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;

namespace OpenRasta.Hosting.AspNet
{
    public class Iis7 : Iis6
    {
        const string MICROSOFT_WEB_ADMINISTRATION = "Microsoft.Web.Administration, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";

        public override IEnumerable<HttpHandlerRegistration> Handlers
        {
            get
            {
                if (HttpRuntime.UsingIntegratedPipeline)
                    return GetNativeHandlers();
                return base.Handlers;
            }
        }

        IEnumerable<HttpHandlerRegistration> GetNativeHandlers()
        {
            IEnumerable handlers;
            try
            {
                var configManagerType = Type.GetType("Microsoft.Web.Administration.WebConfigurationManager, " + MICROSOFT_WEB_ADMINISTRATION);
                var configurationSectionType = Type.GetType("Microsoft.Web.Administration.ConfigurationSection, " + MICROSOFT_WEB_ADMINISTRATION);
                var method = configManagerType.GetMethod("GetSection", new[] { typeof(string) });

                var configSection = method.Invoke(null, new object[] { "system.webServer/handlers" });

                handlers = (IEnumerable)configurationSectionType.GetMethod("GetCollection", new Type[0]).Invoke(configSection, null);
            }
            catch (Exception)
            {
                yield break;
            }
            foreach (var handler in handlers)
            {
                var attribValueMethod = handler.GetType().GetMethod("GetAttributeValue", new[] { typeof(string) });
                var verb = (string)attribValueMethod.Invoke(handler, new object[] { "verb" });
                var path = (string)attribValueMethod.Invoke(handler, new object[] { "path" });
                var type = (string)attribValueMethod.Invoke(handler, new object[] { "type" });
                var handlerReg = new HttpHandlerRegistration(verb, path, type);
                if (IsHandlerRegistrationValid(handlerReg))
                {
                    yield return handlerReg;
                }
            }
        }
    }
}