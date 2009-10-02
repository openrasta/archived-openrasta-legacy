using System;
using OpenBastard.Infrastructure;
using OpenBastard.Scenarios;
using OpenRasta.Hosting.InMemory;
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

        public IRequest CreateRequest(string uri)
        {
            return new InMemoryRequest
                {
                    Uri = new Uri(new Uri("http://localhost", UriKind.Absolute), new Uri(uri, UriKind.Relative))
                };
        }

        public void Dispose()
        {
            _host.Close();
            _host = null;
        }

        public IResponse ExecuteRequest(IRequest request)
        {
            if (request.Entity.Stream.Length > 0)
                request.Entity.Stream.Position = 0;
            var response = _host.ProcessRequest(request);
            if (response.StatusCode == 303)
            {
                string acceptHeader = response.Headers["Accept"];
                var redirectedRequest = new InMemoryRequest
                    {
                        Uri = new Uri(response.Headers["Location"], UriKind.Absolute), 
                        HttpMethod = "GET"
                    }.Accept(acceptHeader);
                response = _host.ProcessRequest(redirectedRequest);
            }
            if (response.Entity.ContentLength > 0 && response.Entity.Stream.CanSeek)
                response.Entity.Stream.Position = 0;
            return response;
        }

        public void Initialize()
        {
            _host = new InMemoryHost(new Configurator());
        }
    }
}