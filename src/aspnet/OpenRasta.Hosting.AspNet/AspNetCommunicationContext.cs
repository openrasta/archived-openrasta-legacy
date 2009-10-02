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
using System.Security.Principal;
using System.Web;
using OpenRasta.Diagnostics;
using OpenRasta.Pipeline;
using OpenRasta.Web;

namespace OpenRasta.Hosting.AspNet
{
    public class AspNetCommunicationContext : ICommunicationContext
    {
        public AspNetCommunicationContext(ILogger logger, HttpContext context, AspNetRequest request, AspNetResponse response)
        {
            Log = logger;
            NativeContext = context;
            ServerErrors = new ServerErrorList { Log = logger };
            PipelineData = new PipelineData();
            Request = request;
            Response = response;
        }

        public Uri ApplicationBaseUri
        {
            get
            {
                if (NativeContext == null)
                    return null;

                string baseUri = "{0}://{1}/".With(NativeContext.Request.Url.Scheme, NativeContext.Request.ServerVariables["HTTP_HOST"]);

                var appBaseUri = new Uri(new Uri(baseUri), new Uri(NativeContext.Request.ApplicationPath, UriKind.Relative));
                return appBaseUri;
            }
        }

        public ILogger Log { get; set; }


        public OperationResult OperationResult { get; set; }
        public PipelineData PipelineData { get; set; }
        public IRequest Request { get; private set; }
        public IResponse Response { get; private set; }

        public IList<Error> ServerErrors { get; private set; }

        public IPrincipal User
        {
            get { return NativeContext.User; }
            set { NativeContext.User = value; }
        }

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