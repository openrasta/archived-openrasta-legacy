using OpenRasta.Configuration;
using OpenRasta.DI;
using OpenRasta.Hosting.HttpListener;

namespace OpenBastard.Environments
{
    public class HttpListenerEnvironment : HttpWebRequestEnvironment
    {
        HttpListenerHost _host;

        public HttpListenerEnvironment() : base(6687)
        {
        }

        public override string Name
        {
            get { return "HttpListener environment"; }
        }

        public override void Dispose()
        {
            _host.Close();
            _host = null;
        }

        public override void Initialize()
        {
            _host = new HttpListenerHostWithConfiguration(new Configurator());
            _host.Initialize(new[] { "http://+:" + Port + "/" }, "/", null);
            _host.StartListening();
        }
    }

    public class HttpListenerHostWithConfiguration : HttpListenerHost
    {
        readonly IConfigurationSource _configuration;

        public HttpListenerHostWithConfiguration(IConfigurationSource configuration)
        {
            _configuration = configuration;
        }

        public override bool ConfigureRootDependencies(IDependencyResolver resolver)
        {
            bool result = base.ConfigureRootDependencies(resolver);
            if (result && _configuration != null)
                resolver.AddDependencyInstance<IConfigurationSource>(_configuration);
            return result;
        }
    }
}