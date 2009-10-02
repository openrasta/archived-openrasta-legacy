using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using OpenRasta.Pipeline;
using OpenRasta.Web;

namespace OpenRasta.Hosting.HttpListener
{
    public class HttpListenerCommunicationContext : ICommunicationContext
    {
        readonly IHost _host;
        readonly HttpListenerContext _nativeContext;

        public HttpListenerCommunicationContext(IHost host, HttpListenerContext nativeContext)
        {
            ServerErrors = new List<Error>();
            PipelineData = new PipelineData();
            _host = host;
            _nativeContext = nativeContext;
            User = nativeContext.User;
            Request = new HttpListenerRequest(this, nativeContext.Request);
            Response = new HttpListenerResponse(this, nativeContext.Response);
        }

        public Uri ApplicationBaseUri
        {
            get
            {
                var request = _nativeContext.Request;

                string baseUri = "{0}://{1}{2}/".With(request.Url.Scheme, 
                                                      request.Url.Host, 
                                                      request.Url.IsDefaultPort ? string.Empty : ":" + request.Url.Port);

                var appBaseUri = new Uri(new Uri(baseUri, UriKind.Absolute), 
                                         new Uri(_host.ApplicationVirtualPath, UriKind.Relative));
                return appBaseUri;
            }
        }

        public OperationResult OperationResult { get; set; }
        public PipelineData PipelineData { get; set; }

        public IRequest Request { get; private set; }

        public IResponse Response { get; private set; }

        public IList<Error> ServerErrors { get; private set; }

        public IPrincipal User { get; set; }
    }
}