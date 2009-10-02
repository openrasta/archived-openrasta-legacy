#region License

/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace OpenRasta.Security
{
    public class DigestHeader
    {
        readonly Dictionary<string, string> _values = new Dictionary<string, string>();

        public DigestHeader()
        {
        }

        public DigestHeader(DigestHeader copy)
        {
            foreach (var kv in copy._values)
                _values.Add(kv.Key, kv.Value);
        }

        public string ClientNonce
        {
            get { return _values.ContainsKey("cnonce") ? _values["cnonce"] : null; }
            set { _values["cnonce"] = value; }
        }

        public string ClientRequestHeader
        {
            get
            {
                var builder = new StringBuilder();
                builder.AppendFormat("Digest username=\"{0}\", realm=\"{1}\",nonce=\"{2}\"",
                                     Username, Realm, Nonce);
                builder.AppendFormat(
                    @",uri=""{0}"",
                 qop={1},
                 nc={2},
                 cnonce=""{3}"",
                 response=""{4}"",
                 opaque=""{5}""",
                    Uri, QualityOfProtection, NonceCount, ClientNonce, Response, Opaque);
                return builder.ToString();
            }
        }

        public string Nonce
        {
            get { return _values.ContainsKey("nonce") ? _values["nonce"] : null; }
            set { _values["nonce"] = value; }
        }

        public string NonceCount
        {
            get { return _values.ContainsKey("nc") ? _values["nc"] : null; }
            set { _values["nc"] = value; }
        }

        public string Opaque
        {
            get { return _values.ContainsKey("opaque") ? _values["opaque"] : null; }
            set { _values["opaque"] = value; }
        }

        public string Password { get; set; }

        public string QualityOfProtection
        {
            get { return _values.ContainsKey("qop") ? _values["qop"] : null; }
            set { _values["qop"] = value; }
        }

        public string Realm
        {
            get { return _values.ContainsKey("realm") ? _values["realm"] : null; }
            set { _values["realm"] = value; }
        }

        public string Response
        {
            get { return _values.ContainsKey("response") ? _values["response"] : null; }
            set { _values["response"] = value; }
        }

        public string ServerResponseHeader
        {
            get
            {
                var builder = new StringBuilder();
                builder.AppendFormat("Digest realm=\"{0}\",nonce=\"{1}\",opaque=\"{2}\"",
                                     Realm, Nonce, Opaque);
                builder.AppendFormat(",stale={0}", Stale);
                builder.Append(",algorithm=MD5, qop=\"auth\"");
                return builder.ToString();
            }
        }

        public bool Stale
        {
            get
            {
                string stale;
                return _values.TryGetValue("stale", out stale) ? bool.Parse(stale) : false;
            }
            set { _values["stale"] = value ? "TRUE" : "FALSE"; }
        }

        public string Uri
        {
            get { return _values.ContainsKey("uri") ? _values["uri"] : null; }
            set { _values["uri"] = value; }
        }

        public string Username
        {
            get { return _values.ContainsKey("username") ? _values["username"] : null; }
            set { _values["username"] = value; }
        }

        public static DigestHeader Parse(string header)
        {
            if (!header.ToUpper().StartsWith("DIGEST"))
                return null;
            var credentials = new DigestHeader();
            string arguments = header.Substring(6);

            string[] keyValues = arguments.Split(',');
            foreach (string kv in keyValues)
            {
                string[] parts = kv.Split(new[] {'='}, 2);
                string key = parts[0].Trim(' ', '\t', '\r', '\n', '\"');
                string value = parts[1].Trim(' ', '\t', '\r', '\n', '\"');
                credentials._values.Add(key, value);
            }
            return credentials;
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
                                               Nonce,
                                               NonceCount,
                                               ClientNonce,
                                               QualityOfProtection,
                                               HA2);
            }
            else
            {
                unhashedDigest = String.Format("{0}:{1}:{2}",
                                               HA1,
                                               NonceCount,
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
}

#region Full license

// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion