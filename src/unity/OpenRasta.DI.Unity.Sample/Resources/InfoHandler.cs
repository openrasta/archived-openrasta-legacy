using OpenRasta.DI.Unity.Sample.Domain;

namespace OpenRasta.DI.Unity.Sample.Resources
{
    /// <summary>
    /// A standard OpenRasta handler.
    /// </summary>
    public class InfoHandler
    {
        readonly IInfoProvider provider;

        public InfoHandler(IInfoProvider provider)
        {
            this.provider = provider;
        }

        public Info Get()
        {
            var info = provider.Get();

            info.Handler = string.Format(GetType().Name);

            return info;
        }
    }
}
