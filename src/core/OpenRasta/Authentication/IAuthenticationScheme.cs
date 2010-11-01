using OpenRasta.Web;

namespace OpenRasta.Authentication
{
    public interface IAuthenticationScheme
    {
        string Name { get; }

        AuthenticationResult Authenticate(IRequest request);

        void Challenge(IResponse response);
    }
}