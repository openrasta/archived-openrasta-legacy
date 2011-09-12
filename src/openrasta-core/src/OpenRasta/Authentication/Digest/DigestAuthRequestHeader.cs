using System;
using System.Security.Cryptography;
using System.Text;
using OpenRasta;

namespace OpenRasta.Authentication.Digest
{
    public enum DigestAlgorithm
    {
        MD5
    }

    public class DigestAuthResponseChallenge
    {
        public string Realm { get; private set; } // realm=""

        public string Username { get; private set; } // qop=""
        public string Password { get; private set; } // qop=""
        public string Salt { get; private set; } // qop=""
        public string QualityOfProtection { get; private set; } // qop=""
        public string ServerNonce { get; private set; } // nonce=""
        public string ClientNonce { get; private set; } // cnonce=""
        public string Uri { get; private set; } // uri=""
        public string Response { get; private set; } // response=""
        public string Digest { get; private set; } // digest=""
        public string Opaque { get; private set; } // opaque=""
        public string RequestCounter { get; private set; } // nc=""
        public bool Stale { get; private set; } // stale=""
        public DigestAlgorithm Algorithm { get; private set; } // algorithm=""

        public DigestAuthResponseChallenge(string realm, string serverNonce, byte[] opaqueData, bool stale)
        {
            
        }

        public string GetCalculatedResponse(string httpMethod)
        {
            // A1 = unq(username-value) ":" unq(realm-value) ":" passwd
            string A1 = String.Format("{0}:{1}:{2}", Username, Realm, Password);

            // H(A1) = MD5(A1)
            string HA1 = GetMD5HashBinHex(A1);

            // A2 = Method ":" digest-uri-value
            string A2 = String.Format("{0}:{1}", httpMethod, Uri);

            // H(A2)
            string HA2 = GetMD5HashBinHex(A2);

            // KD(secret, data) = H(concat(secret, ":", data))
            // if qop == auth:
            // request-digest  = <"> < KD ( H(A1),     unq(nonce-value)
            // ":" nc-value
            // ":" unq(cnonce-value)
            // ":" unq(qop-value)
            // ":" H(A2)
            // ) <">
            // if qop is missing,
            // request-digest  = <"> < KD ( H(A1), unq(nonce-value) ":" H(A2) ) > <">
            string unhashedDigest;
            if (QualityOfProtection != null)
            {
                unhashedDigest = String.Format("{0}:{1}:{2}:{3}:{4}:{5}",
                                               HA1,
                                               ServerNonce,
                                               RequestCounter,
                                               ClientNonce,
                                               QualityOfProtection,
                                               HA2);
            }
            else
            {
                unhashedDigest = String.Format("{0}:{1}:{2}",
                                               HA1,
                                               RequestCounter,
                                               HA2);
            }

            return GetMD5HashBinHex(unhashedDigest);
        }

        static string GetMD5HashBinHex(string toBeHashed)
        {
            MD5 hash = MD5.Create();
            byte[] result = hash.ComputeHash(Encoding.ASCII.GetBytes(toBeHashed));

            var sb = new StringBuilder();
            foreach (byte b in result)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }

    public class DigestAuthRequestParameters
    {
        private const string SchemeName = "DIGEST";
        private const string SchemeNameWithSpace = SchemeName + " ";

        public string Username { get; private set; } // username=""
        public string Realm { get; private set; } // realm=""

        public string QualityOfProtection { get; private set; } // qop=""
        public string ServerNonce { get; private set; } // nonce=""
        public string ClientNonce { get; private set; } // cnonce=""
        public string Uri { get; private set; } // uri=""
        public string Response { get; private set; } // response=""
        public string Digest { get; private set; } // digest=""
        public string Opaque { get; private set; } // opaque=""
        public string RequestCounter { get; private set; } // nc=""

        public DigestAuthRequestParameters(string username)
        {
            Username = username;
        }

        public static DigestAuthRequestParameters Parse(string value)
        {
            if (value.IsNullOrWhiteSpace()) return null;

            if (!value.ToUpper().StartsWith(SchemeNameWithSpace)) return null;

            var basicBase64Credentials = value.Split(' ')[1];
            var basicCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(basicBase64Credentials)).Split(':');

            var username = basicCredentials[0];
            var password = basicCredentials[1];

            return new DigestAuthRequestParameters(username, password);
        }

        public static bool TryParse(string value, out DigestAuthRequestParameters credentials)
        {
            credentials = null;

            if (string.IsNullOrWhiteSpace(value)) return false;

            if (!value.ToUpper().StartsWith(SchemeNameWithSpace)) return false;

            var basicBase64Credentials = value.Split(' ')[1];

            credentials = ExtractBasicCredentials(basicBase64Credentials);

            return true;
        }

        private static DigestAuthRequestParameters ExtractDigestCredentials(string basicCredentialsAsBase64)
        {
            var basicCredentials = basicCredentialsAsBase64.FromBase64String().Split(':');

            var username = basicCredentials[0];
            var password = basicCredentials[1];

            return new DigestAuthRequestParameters(username, password);
        }
    }
}