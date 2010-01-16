using System;
using System.Net;
using OpenRasta.Hosting.InMemory;
using OpenRasta.IO;
using OpenRasta.Web;

namespace OpenBastard.Environments
{
    public abstract class HttpWebRequestEnvironment : IEnvironment
    {
        public HttpWebRequestEnvironment(int port)
        {
            Port = port;
        }

        public abstract string Name { get; }
        public int Port { get; set; }

        public IClientRequest CreateRequest(string uri)
        {
            return new InMemoryClientRequest
                {
                    Uri = new Uri(new Uri("http://localhost:" + Port, UriKind.Absolute), new Uri(uri, UriKind.Relative))
                };
        }

        public abstract void Dispose();

        public IResponse ExecuteRequest(IClientRequest request)
        {
            var webRequest = ConvertRequestToWebClient(request);
            HttpWebResponse response;
            try
            {
                response = webRequest.GetResponse() as HttpWebResponse;
            }
            catch (WebException exception)
            {
                response = exception.Response as HttpWebResponse;
                if (response == null)
                    throw;
            }

            return new WebRequestResponse(response);
        }

        public abstract void Initialize();

        WebRequest ConvertRequestToWebClient(IClientRequest request)
        {
            var webRequest = WebRequest.Create(request.Uri);
            webRequest.Method = request.HttpMethod;
            if (request.Entity.ContentType != null)
                webRequest.ContentType = request.Entity.ContentType.ToString();
            if (request.Credentials != null)
                webRequest.Credentials = new NetworkCredential(request.Credentials.Username, request.Credentials.Password);
            foreach (var header in request.Headers)
            {
                try
                {
                    webRequest.Headers[header.Key] = header.Value;
                }
                catch (ArgumentException)
                {
                }
            }
            if (request.Entity.ContentLength != null && request.Entity.ContentLength > 0)
            {
                webRequest.ContentLength = request.Entity.Stream.Length;
                var requestStream = webRequest.GetRequestStream();
                request.Entity.Stream.Position = 0;
                request.Entity.Stream.CopyTo(requestStream);
            }

            return webRequest;
        }
    }
}