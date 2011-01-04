using OpenRasta.DI;
using OpenRasta.Hosting;
using OpenRasta.Hosting.InMemory;
using OpenRasta.Testing;
using OpenRasta.Web;

namespace OpenRasta.Codecs
{
    public abstract class codec_context<TCodec> : context where TCodec:ICodec
    {
        public InMemoryHost Host { get; private set; }
        protected ICommunicationContext Context { get; private set; }
        protected HostManager HostManager { get; set; }
        protected abstract TCodec CreateCodec(ICommunicationContext context);

        protected void given_context()
        {
            Host = new InMemoryHost(null);
            HostManager = Host.HostManager;
            HostManager.SetupCommunicationContext(Context = new InMemoryCommunicationContext());
            DependencyManager.SetResolver(Host.Resolver);
        }
        
        protected override void TearDown()
        {
            DependencyManager.UnsetResolver();
        }
    }
}