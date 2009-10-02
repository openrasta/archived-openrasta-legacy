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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using OpenRasta.Configuration;
using OpenRasta.DI;
using OpenRasta.Hosting;
using OpenRasta.Hosting.HttpListener;
using OpenRasta.Testing;
using OpenRasta.Web;

namespace OpenRasta.Tests.Integration
{
    public abstract class server_context : context
    {
        protected override void SetUp()
        {
            base.SetUp();
            TheResponseAsString = null;
            TheResponse = null;
        }

        AppDomainHost<ConfigurationExecutorHost> _http;
        public HttpWebResponse TheResponse;
        public string TheResponseAsString;
        public int _port;
        string _username;
        string _password;
        internal WebRequest Request { get; set; }

        public server_context()
        {
            SelectPort();

            _http = new AppDomainHost<ConfigurationExecutorHost>(new[] { "http://+:" + _port + "/" }, "/",null);
            _http.Initialize();
        }
        public server_context(int port)
        {
            _port = port;
        }

        void SelectPort()
        {
            _port = 6687;
        }
        [TestFixtureSetUp]
        public virtual void setup_context()
        {
            if (_http != null)
                _http.StartListening();
        }
        [TestFixtureTearDown]
        public void tear()
        {
            try
            {

                if (_http != null)
                {
                    _http.StopListening();
                    _http = null;
                }
            }
            catch(AppDomainUnloadedException){}
        }

        public void given_request(string verb, string uri)
        {
            given_request(verb, uri, null, null);
        }

        public void given_request_as_string(string verb, string uri, string content, string textEncoding)
        {
            given_request_as_string(verb, uri, content, textEncoding, "text/plain");
        }

        public void given_request_as_string(string verb, string uri, string content, string textEncoding, string contentType)
        {
            given_request(verb, uri, Encoding.GetEncoding(textEncoding).GetBytes(content),
                          new MediaType(contentType) {CharSet = textEncoding});
        }

        public void given_request_as_url_form_encoded(string verb, string uri, string content, string textEncoding)
        {
            given_request(verb, uri, Encoding.GetEncoding(textEncoding).GetBytes(content),
                          new MediaType("application/x-www-form-urlencoded") {CharSet = textEncoding});
        }
        public void given_client_credentials(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public void given_request(string verb, string uri, byte[] content, MediaType contentType)
        {
            var destinationUri = new Uri("http://127.0.0.1:" + _port + uri);

            Request = WebRequest.Create(destinationUri);
            Request.Timeout = int.MaxValue;
            Request.Method = verb;
            Request.ContentLength = content != null ? content.Length : 0;
            if (_username != null)
            {
                Request.Credentials = new NetworkCredential(_username, _password);
            }
            if (Request.ContentLength > 0)
            {
                Request.ContentType = contentType.ToString();
                using (Stream requestStream = Request.GetRequestStream())
                    requestStream.Write(content, 0, content.Length);
            }
        }
        public void when_reading_response()
        {
            try
            {
                TheResponse = Request.GetResponse() as HttpWebResponse;
            }
            catch (WebException exception)
            {
                TheResponse = exception.Response as HttpWebResponse;
                Console.WriteLine(exception.ToString());
            }
            
        }
        public void when_reading_response_as_a_string(Encoding encoding)
        {
            when_reading_response();
            var data = new byte[TheResponse.ContentLength];
            var reader = new BinaryReader(TheResponse.GetResponseStream());

            int payload = TheResponse.GetResponseStream().Read(data, 0, data.Length);

            TheResponseAsString = encoding.GetString(data, 0, payload);
        }

        public void ConfigureServer(Action t)
        {
            _http.Listener.Configure(t);
        }

        public class ConfigurationExecutorHost : HttpListenerHost
        {
            Action t;
            public void Configure(Action t)
            {
                this.t = t;
            }
            public override bool ConfigureLeafDependencies(IDependencyResolver resolver)
            {
                using (OpenRastaConfiguration.Manual)
                    t();
                return true;
            }
        }
    }
}

#region Full license

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#endregion