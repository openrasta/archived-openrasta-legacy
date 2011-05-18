using OpenRasta.Configuration;
using OpenRasta.Configuration.Fluent;
using OpenRasta.DI;

namespace OpenRasta.Codecs.jsonp
{
    public class JsonPConfigurator
    {
        IDependencyResolver _dependencyResolver;

        public JsonPConfigurator(IUses uses)
        {
            uses.UriDecorator<JsonPUriDecorator>();
            _dependencyResolver = DependencyManager.GetService<IDependencyResolver>();
            _dependencyResolver.AddDependency(typeof(JsonPConfiguration), DependencyLifetime.Singleton);
            WithQueryString("jsonp");
            WithJsonCodec<JsonDataContractCodec>();
        }

        public JsonPConfigurator WithQueryString(string querystringName)
        {
            var cfg = _dependencyResolver.Resolve<JsonPConfiguration>();
            cfg.QueryString = querystringName;
            return this;
        }

        public JsonPConfigurator WithJsonCodec<TCodec> ()
            where TCodec : IMediaTypeWriter
        {

            var cfg = _dependencyResolver.Resolve<JsonPConfiguration>();
            cfg.CodecType = typeof(JsonPCodec<TCodec>);
            _dependencyResolver.AddDependency(typeof(TCodec), DependencyLifetime.Transient);

            return this;
        }
    }
}