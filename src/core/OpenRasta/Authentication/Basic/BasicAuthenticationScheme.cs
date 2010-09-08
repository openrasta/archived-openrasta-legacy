using OpenRasta.Web;

namespace OpenRasta.Authentication.Basic
{
    public class BasicAuthenticationScheme : IAuthenticationScheme
    {
        const string SCHEME = "Basic";

        private readonly IBasicAuthenticator _basicAuthenticator;

        public string Name { get { return SCHEME; } }

        public BasicAuthenticationScheme(IBasicAuthenticator basicAuthenticator)
        {
            _basicAuthenticator = basicAuthenticator;
        }

        public AuthenticationResult Authenticate(IRequest request)
        {
            BasicAuthRequestHeader credentials = ExtractBasicHeader(request.Headers["Authorization"]);

            if (credentials != null)
            {
                return _basicAuthenticator.Authenticate(credentials);
            }

            return new AuthenticationResult.MalformedCredentials();
        }

        public void Challenge(IResponse response)
        {
            response.Headers["WWW-Authenticate"] = string.Format("{0} realm=\"{1}\"", SCHEME, _basicAuthenticator.Realm);
        }

        internal static BasicAuthRequestHeader ExtractBasicHeader(string value)
        {
            try
            {
                var basicBase64Credentials = value.Split(' ')[1];

                var basicCredentials = basicBase64Credentials.FromBase64String().Split(':');

                if (basicCredentials.Length != 2)
                    return null;

                return new BasicAuthRequestHeader(basicCredentials[0], basicCredentials[1]);
            }
            catch
            {
                return null;
            }

        }
    }
}