using System.Collections.Generic;
using System.IO;
using System.Net;
using OpenRasta;
using OpenRasta.Codecs;
using OpenRasta.Web;

namespace OpenBastard.Environments
{
    public class WebRequestResponse : IResponse, IHttpEntity
    {
        readonly HttpWebResponse _response;

        public WebRequestResponse(HttpWebResponse response)
        {
            _response = response;
            if (!string.IsNullOrEmpty(response.ContentType))
                ContentType = new MediaType(response.ContentType);

            ContentLength = response.ContentLength;
            Headers = new HttpHeaderDictionary();
            foreach (string headerName in response.Headers.AllKeys)
                Headers[headerName] = response.Headers[headerName];
        }

        public ICodec Codec
        {
            get { return null; }
            set { }
        }

        public long? ContentLength { get; set; }
        public MediaType ContentType { get; set; }

        public IHttpEntity Entity
        {
            get { return this; }
        }

        public IList<Error> Errors
        {
            get { return new Error[0]; }
        }

        public HttpHeaderDictionary Headers { get; private set; }

        public bool HeadersSent
        {
            get { return true; }
        }

        public object Instance
        {
            get { return null; }
            set { }
        }

        public int StatusCode
        {
            get { return (int)_response.StatusCode; }
            set { }
        }

        public Stream Stream
        {
            get { return _response.GetResponseStream(); }
        }

        public void WriteHeaders()
        {
        }
    }
}