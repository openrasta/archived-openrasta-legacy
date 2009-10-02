using System;
using System.IO;
using System.Net;
using System.Text;
using NUnit.Framework;
using OpenRasta.Hosting.AspNet.AspNetHttpListener;
using OpenRasta.Testing;
using OpenRasta.Web;

namespace OpenRasta.Hosting.AspNet.Tests.Integration
{
    public class aspnet_server_context : context
    {
        protected override void SetUp()
        {
            base.SetUp();
            TheResponseAsString = null;
            TheResponse = null;
        }

        readonly HttpListenerController _http;
        public HttpWebResponse TheResponse;
        public string TheResponseAsString;
        public int _port;

        public aspnet_server_context()
        {
            SelectPort();

            _http = new HttpListenerController(new[] {"http://+:" + _port + "/"}, "/", FileCopySetup.TempFolder.FullName);
            _http.Start();
        }

        void SelectPort()
        {
            _port = 6687;
        }

        [TestFixtureTearDown]
        public void tear()
        {
            if (_http != null)
                _http.Stop();
        }

        public void GivenARequest(string verb, string uri)
        {
            GivenARequest(verb, uri, null, null);
        }

        public void GivenATextRequest(string verb, string uri, string content, string textEncoding)
        {
            GivenATextRequest(verb, uri, content, textEncoding, "text/plain");
        }

        public void GivenATextRequest(string verb, string uri, string content, string textEncoding, string contentType)
        {
            GivenARequest(verb, uri, Encoding.GetEncoding(textEncoding).GetBytes(content),
                          new MediaType(contentType) {CharSet = textEncoding});
        }

        public void GivenAUrlFormEncodedRequest(string verb, string uri, string content, string textEncoding)
        {
            GivenARequest(verb, uri, Encoding.GetEncoding(textEncoding).GetBytes(content),
                          new MediaType("application/x-www-form-urlencoded") {CharSet = textEncoding});
        }

        public void GivenARequest(string verb, string uri, byte[] content, MediaType contentType)
        {
            var destinationUri = new Uri("http://127.0.0.1:" + _port + uri);

            WebRequest request = WebRequest.Create(destinationUri);
            request.Timeout = int.MaxValue;
            request.Method = verb;
            request.ContentLength = content != null ? content.Length : 0;
            if (request.ContentLength > 0)
            {
                request.ContentType = contentType.ToString();
                using (Stream requestStream = request.GetRequestStream())
                    requestStream.Write(content, 0, content.Length);
            }
            try
            {
                TheResponse = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException exception)
            {
                TheResponse = exception.Response as HttpWebResponse;
            }
        }

        public void GivenTheResponseIsInEncoding(Encoding encoding)
        {
            var data = new byte[TheResponse.ContentLength];

            int payload = TheResponse.GetResponseStream().Read(data, 0, data.Length);

            TheResponseAsString = encoding.GetString(data, 0, payload);
        }

        public void ConfigureServer(Action t)
        {
            _http.Host.ExecuteConfig(t);
        }
    }
}