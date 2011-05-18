using OpenRasta.Configuration.Fluent;

namespace OpenRasta.Codecs.jsonp
{
    public static class JsonPConfigurationExtensionMethods
    {
        public static JsonPConfigurator JsonP(this IUses uses)
        {
            return new JsonPConfigurator(uses);
        }
    }
}