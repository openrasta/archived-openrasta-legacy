using System;
using OpenBastard.Infrastructure;
using OpenRasta.Hosting.InMemory;
using OpenRasta.Security;
using OpenRasta.Web;

namespace OpenBastard.Environments
{
    public class InMemoryEnvironment : IEnvironment
    {
        InMemoryHost _host;

        public string Name
        {
            get { return "In memory environment"; }
        }

        public void Dispose()
        {
            _host.Close();
            _host = null;
        }

        public IClientRequest CreateRequest(string uri)
        {
            return new InMemoryClientRequest
            {
                Uri = new Uri(new Uri("http://localhost", UriKind.Absolute), new Uri(uri, UriKind.Relative))
            };
        }

        public IResponse ExecuteRequest(IClientRequest request)
        {
            if (request.Entity.Stream.Length > 0)
                request.Entity.Stream.Position = 0;
            var response = _host.ProcessRequest(request);
            if (response.StatusCode == 303)
                response = RedirectOn303SeeOther(response);
            else if (response.StatusCode == 401)
                response = RetryWithHttpAuthenticationCredentials(request, response);

            if (response.Entity.ContentLength > 0 && response.Entity.Stream.CanSeek)
                response.Entity.Stream.Position = 0;
            return response;
        }

        IResponse RetryWithHttpAuthenticationCredentials(IClientRequest request, IResponse response)
        {
            if (response.Headers["WWW-Authenticate"] != null && response.Headers["WWW-Authenticate"].Contains("Digest"))
            {
                var responseDigest = DigestHeader.Parse(response.Headers["WWW-Authenticate"]);

                var header = new OpenRasta.Security.DigestHeader(responseDigest)
                {
                    Username = request.Credentials.Username,
                    Password = request.Credentials.Password,
                    Nonce = responseDigest.Nonce,
                    ClientNonce = "none",
                    Uri = request.Uri.GetLeftPart(UriPartial.Path)
                };
                header.Response =  header.GetCalculatedResponse(request.HttpMethod);

                request.Headers["Authorization"] = header.ClientRequestHeader;
                return _host.ProcessRequest(request);
            }
            return response;
        }

        IResponse RedirectOn303SeeOther(IResponse response)
        {
            string acceptHeader = response.Headers["Accept"];
            var redirectedRequest = new InMemoryRequest
            {
                Uri = new Uri(response.Headers["Location"], UriKind.Absolute), 
                HttpMethod = "GET"
            }.Accept(acceptHeader);
            response = _host.ProcessRequest(redirectedRequest);
            return response;
        }

        public void Initialize()
        {
            _host = new InMemoryHost(new Configurator());
        }
    }

    public class InMemoryClientRequest : InMemoryRequest, IClientRequest
    {
        public Credentials Credentials { get; set; }
    }
}