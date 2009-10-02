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
using System.IO;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Web.UI;
using OpenRasta.Collections.Specialized;
using OpenRasta.DI;
using OpenRasta.Hosting.InMemory;
using OpenRasta.Pipeline;
using OpenRasta.Pipeline.InMemory;
using OpenRasta.Web;
using OpenRasta.Web.Markup;

namespace OpenRasta.Codecs.WebForms
{
    public static class XHtmlProducer
    {
        public static void RenderResource(this IXhtmlAnchor anchor, object resource)
        {
            throw new NotImplementedException();
        }

        // TODO: Need to be moved to the core framework.
        public static void RenderResource(this IXhtmlAnchor anchor, Uri resource)
        {
            var context = new InMemoryCommunicationContext
                {
                    Request = new InMemoryRequest
                        {
                            HttpMethod = "GET", 
                            Uri = resource, 
                            Entity = new HttpEntity
                                {
                                    ContentLength = 0
                                }
                        }
                };
            context.Request.Headers["Accept"] = MediaType.XhtmlFragment.ToString();
            var textWriterProvider = anchor.AmbientWriter as ISupportsTextWriter;

            StringBuilder inMemoryRendering = null;

            if (textWriterProvider != null && textWriterProvider.TextWriter != null)
                context.Response = new InMemoryResponse
                    {
                        Entity = new TextWriterEnabledEntity(textWriterProvider.TextWriter)
                    };
            else
            {
                inMemoryRendering = new StringBuilder();
                var writer = new StringWriter(inMemoryRendering);
                context.Response = new InMemoryResponse { Entity = new TextWriterEnabledEntity(writer) };
            }

            anchor.Resolver.Resolve<IPipeline>().Run(context);

            if (context.Response.Entity.Stream.Length > 0)
            {
                context.Response.Entity.Stream.Position = 0;
                var
                    destinationEncoding =
                        Encoding.GetEncoding(context.Response.Entity.ContentType.CharSet ?? "UTF8");

                var reader = new StreamReader(context.Response.Entity.Stream, destinationEncoding);
                anchor.AmbientWriter.WriteUnencodedString(reader.ReadToEnd());
            }
            else if (inMemoryRendering != null && inMemoryRendering.Length > 0)
            {
                anchor.AmbientWriter.WriteUnencodedString(inMemoryRendering.ToString());
            }
        }

        public static UnencodedOutput RenderUserControl(this IXhtmlAnchor anchor, string vpath)
        {
            return RenderUserControl(anchor, vpath, null);
        }

        public static UnencodedOutput RenderUserControl(this IXhtmlAnchor anchor, string vpath, object parameters)
        {
            var control = DependencyManager.GetService(BuildManager.GetCompiledType(vpath)) as Control;

            if (parameters != null)
            {
                var keyValues = parameters.ToProperties();

                AssignPropertiesToControl(control, keyValues);
            }
            var builder = new StringBuilder();
            var writer = new StringWriter(builder);

            var dummyPage = new Page();
            dummyPage.Controls.Add(control);

            HttpContext.Current.Server.Execute(dummyPage, writer, true);

            return (UnencodedOutput)builder.ToString();
        }

        static void AssignPropertiesToControl(Control control, IDictionary<string, object> keyValues)
        {
            var controlType = control.GetType();
            foreach (var kv in keyValues)
            {
                var pi = controlType.GetProperty(kv.Key);
                if (pi != null)
                    pi.SetValue(control, kv.Value, null);
                else
                    throw new InvalidOperationException("Some properties don't match the control's properties.");
            }
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