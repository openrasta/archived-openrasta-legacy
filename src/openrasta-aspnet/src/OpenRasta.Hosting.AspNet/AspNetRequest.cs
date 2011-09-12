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
using System.Globalization;
using System.Web;
using OpenRasta.Web;

namespace OpenRasta.Hosting.AspNet
{
    public class AspNetRequest : IRequest
    {
        string _httpMethod;

        public AspNetRequest(HttpContext context)
        {
            NativeContext = context;
            Uri = NativeContext.Request.Url;
            Headers = new HttpHeaderDictionary(NativeContext.Request.Headers);

            // TODO: Finish off the new input stream that goes to the
            // WorkerRequest and bypasses asp.net completely.
            Entity = new HttpEntity(Headers, NativeContext.Request.InputStream);

            if (!string.IsNullOrEmpty(NativeContext.Request.ContentType))
                Entity.ContentType = new MediaType(NativeContext.Request.ContentType);
            CodecParameters = new List<string>();
        }

        public IList<string> CodecParameters { get; private set; }

        public IHttpEntity Entity { get; private set; }

        public HttpHeaderDictionary Headers { get; private set; }

        public string HttpMethod
        {
            get { return _httpMethod ?? NativeContext.Request.HttpMethod; }
            set { _httpMethod = value; }
        }

        public CultureInfo NegotiatedCulture { get; set; }

        public Uri Uri { get; set; }
        public string UriName { get; set; }
        HttpContext NativeContext { get; set; }
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