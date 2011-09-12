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
using System.Text;
using System.Xml;

namespace OpenRasta.Web.Configuration.Wadl
{
    // no use for WADL for the moment, will review later if its worth including.


    //public class WadlRenderer : XmlCodec
    //{
    //    IDisposable _closer;
    //    private const string NS_WADL = "http://research.sun.com/wadl/2006/10";
    //    public override string ContentType { get { return "application/vnd.sun.wadl+xml"; } }
    //    public override void WriteToCore(IRastaContext context)
    //    {
    //        _closer = new ElementCloser(Writer);

    //        WadlApplication application = context.Request.Entity.EntityInstance as WadlApplication;

    //        using (Application())
    //        {
    //            using (Resources(application.Resources.BasePath))
    //            {
    //                foreach (var resource in application.Resources)
    //                {
    //                    using (Resource(resource.Path))
    //                    {
    //                        foreach (var param in resource.Parameters)
    //                            Param(param.Name, param.Style.ToString().ToLowerInvariant());

    //                        Method("GET");
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    protected IDisposable Application()
    //    {
    //        Writer.WriteStartElement("application", NS_WADL);
    //        return new ElementCloser(Writer);
    //    }
    //    protected IDisposable Resources(string @base)
    //    {
    //        Writer.WriteStartElement("resources", NS_WADL);
    //        return new ElementCloser(Writer);
            

    //    }
    //    protected IDisposable Resource(string path)
    //    {
    //        Writer.WriteStartElement("resource", NS_WADL);
    //        Writer.WriteAttributeString("base", path);
    //        return new ElementCloser(Writer);
    //    }
    //    protected void Param(string name, string style)
    //    {
    //        Writer.WriteStartElement("param", NS_WADL);
    //        Writer.WriteAttributeString("name", name);
    //        Writer.WriteAttributeString("style", style);
    //        Writer.WriteEndElement();
    //    }
    //    protected void Method(string operation)
    //    {
    //        Writer.WriteStartElement("method", NS_WADL);
    //        Writer.WriteAttributeString("name", operation);
    //        Writer.WriteEndElement();
    //    }
    //    protected void Request()
    //    {

    //    }
    //    protected void End()
    //    {
    //        Writer.WriteEndElement();
    //    }
    //    private class ElementCloser : IDisposable
    //    {
    //        private XmlWriter _writer;
    //        public ElementCloser(XmlWriter writer)
    //        {
    //            _writer = writer;
    //        }
    //        public void Dispose()
    //        {
    //            _writer.WriteEndElement();
    //        }
    //    }
    //}
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
