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
using System.Web;
using OpenRasta.DI;
using OpenRasta.Diagnostics;
using OpenRasta.Web;

namespace OpenRasta.Hosting.AspNet
{
    public class AspNetResponse : IResponse
    {
        public AspNetResponse(HttpContext context)
        {
            NativeContext = context;
            Headers = new HttpHeaderDictionary();
            Entity = new HttpEntity(Headers, NativeContext.Response.OutputStream);
        }

        public long? ContentLength
        {
            get { return Headers.ContentLength; }
            set { Headers.ContentLength = value; }
        }

        public string ContentType
        {
            get { return Headers.ContentType.ToString(); }
            set { Headers.ContentType = new MediaType(value); }
        }

        public IHttpEntity Entity { get; set; }
        public HttpHeaderDictionary Headers { get; private set; }
        public bool HeadersSent { get; private set; }
        public ILogger Log { get; set; }

        public int StatusCode
        {
            get { return NativeContext.Response.StatusCode; }
            set { NativeContext.Response.StatusCode = value; }
        }

        HttpContext NativeContext { get; set; }

        public void WriteHeaders()
        {
            if (HeadersSent)
                throw new InvalidOperationException("The headers have already been sent.");
            foreach (var header in Headers)
            {
                try
                {
                    Log.WriteDebug("Writing http header {0}:{1}", header.Key, header.Value);
                    NativeContext.Response.AppendHeader(header.Key, header.Value);
                }
                catch (Exception ex)
                {
                    var commcontext = DependencyManager.GetService<ICommunicationContext>();
                    if (commcontext != null)
                        commcontext.ServerErrors.Add(new Error { Message = ex.ToString() });
                }
            }
            HeadersSent = true;
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