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
using OpenRasta.Web;
using OpenRasta.Pipeline;

namespace OpenRasta.Pipeline.Contributors
{
    /// <summary>
    /// Supports the use of the X-HTTP-Method-Override header to override the verb used
    /// by OpenRasta for processing.
    /// </summary>
    /// <remarks>Clients that can add http headers may not support other verbs than POST (Flash and Silverlight for example). With the X-HTTP-Method-Override header, OpenRasta will process the request as if it was made using a genuine http verb.</remarks>
    public class HttpMethodOverriderContributor : IPipelineContributor
    {
        const string HTTP_METHOD_OVERRIDE = "X-HTTP-Method-Override";
        public void Initialize(IPipeline pipelineRunner)
        {
            pipelineRunner.Notify(OverrideHttpVerb).Before<KnownStages.IHandlerSelection>();
        }

        public PipelineContinuation OverrideHttpVerb(ICommunicationContext context)
        {
            if (context.Request.Headers[HTTP_METHOD_OVERRIDE] != null)
            {
                if (context.Request.HttpMethod != "POST")
                {
                    context.ServerErrors.Add(new MethodIsNotPostError(context.Request.HttpMethod));
                    return PipelineContinuation.Abort;
                }
                context.Request.HttpMethod = context.Request.Headers[HTTP_METHOD_OVERRIDE];
            }
            return PipelineContinuation.Continue;
        }

        public class MethodIsNotPostError : ErrorFrom<HttpMethodOverriderContributor>
        {
            public MethodIsNotPostError(string requestedMethod)
            {
                Title = "Overriding the http method is not supported on method " + requestedMethod;
                Message =
                    "The X-HTTP-Method-Override http header can only be added to requests that are sent as a POST.\r\n"
                    + "Http methods are case-sensitive, make sure the method is in all upper-case.";
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