using System;
using System.Collections.Generic;
using System.Globalization;
using OpenRasta.IO;
using OpenRasta.Web;

namespace OpenRasta.Hosting.HttpListener
{
    public class HttpListenerRequest : IRequest
    {
        readonly HttpListenerCommunicationContext _context;
        readonly System.Net.HttpListenerRequest _nativeRequest;
        string _httpMethod;

        public HttpListenerRequest(HttpListenerCommunicationContext context, System.Net.HttpListenerRequest request)
        {
            _context = context;
            _nativeRequest = request;
            Uri = _nativeRequest.Url;
            CodecParameters = new List<string>();

            Headers = new HttpHeaderDictionary(_nativeRequest.Headers);

            Entity = new HttpEntity(Headers, new HistoryStream(_nativeRequest.InputStream));

            if (!string.IsNullOrEmpty(_nativeRequest.ContentType))
                Entity.ContentType = new MediaType(_nativeRequest.ContentType);
        }

        public IList<string> CodecParameters { get; private set; }

        public long? ContentLength
        {
            get { return Entity.ContentLength; }
            set { Entity.ContentLength = value; }
        }

        public IHttpEntity Entity { get; private set; }
        public HttpHeaderDictionary Headers { get; private set; }

        public string HttpMethod
        {
            get { return _httpMethod ?? _nativeRequest.HttpMethod; }
            set { _httpMethod = value; }
        }

        public CultureInfo NegotiatedCulture { get; set; }
        public Uri Uri { get; set; }

        public string UriName { get; set; }
    }
}