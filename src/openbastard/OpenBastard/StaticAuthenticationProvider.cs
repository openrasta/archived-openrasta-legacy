using OpenRasta.Security;

namespace OpenBastard
{
    public class StaticAuthenticationProvider : IAuthenticationProvider
    {
        public Credentials GetByUsername(string p)
        {
            return new Credentials() { Username = "username", Password = "password" };
        }
    }
}