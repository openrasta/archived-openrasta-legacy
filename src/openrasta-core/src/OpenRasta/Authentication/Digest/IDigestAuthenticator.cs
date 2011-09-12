namespace OpenRasta.Authentication.Digest
{
    public interface IDigestAuthenticator
    {
        string Realm { get; }
        AuthenticationResult Authenticate(DigestAuthRequestParameters header);
    }
}