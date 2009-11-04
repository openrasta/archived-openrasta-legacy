using OpenRasta.Web;
using OpenRasta.Diagnostics;

namespace OpenRasta.DI.Unity.Sample.Domain
{
    /// <summary>
    /// An example application service which might come from your existing infrastructure and be
    /// shared with other applications.  We can depend on OpenRastas interfaces like IReqeust if
    /// desired.
    /// </summary>
    public interface IInfoProvider
    {
        Info Get();
    }

    public class InfoProvider : IInfoProvider
    {
        readonly IRequest request;
        readonly ILogger logger;

        public InfoProvider(IRequest request, ILogger logger)
        {
            this.request = request;
            this.logger = logger;
        }

        public Info Get()
        {
            return new Info
            {
                Logger = logger.GetType().Name,
                Provider = string.Format(GetType().Name),
                Url = string.Format(request.Uri.ToString())
            };
        }
    }
}
