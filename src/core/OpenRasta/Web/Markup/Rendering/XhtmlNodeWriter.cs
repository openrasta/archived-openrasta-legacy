#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion
using OpenRasta.Collections;
using OpenRasta.Web.Markup.Attributes;
using OpenRasta.Web.Markup.Elements;
using OpenRasta.Web.Markup.Modules;

namespace OpenRasta.Web.Markup.Rendering
{
    public class XhtmlNodeWriter
    {
        public  virtual void WriteStartTag(IXhtmlWriter writer, IElement element)
        {
            if (!string.IsNullOrEmpty(element.TagName))
            {
                writer.BeginWriteStartElement(element.TagName);
                element.Attributes.ForEach(a => WriteAttribute(writer, a));
                if (element.ContentModel.Count == 0) writer.EndWriteStartElementFinal();
                else writer.EndWriteStartElement();
            }
        }

        protected virtual void WriteAttribute(IXhtmlWriter writer, IAttribute attribute)
        {
            if (!attribute.IsDefault || attribute.RendersOnDefaultValue)
                writer.WriteAttributeString(attribute.Name.ToLowerInvariant(), attribute.SerializedValue);
        }

        public virtual void WriteChildren(IXhtmlWriter writer, IElement element)
        {
            if (element.ChildNodes.Count > 0) element.ChildNodes.ForEach(child => Write(writer, child));
        }

        public virtual void WriteEndTag(IXhtmlWriter writer, IElement element)
        {
            if (element.ContentModel.Count > 0) writer.WriteEndElement(element.TagName);
        }

        public void Write(IXhtmlWriter writer, INode element)
        {
            if (element is ITextNode) writer.WriteString(((ITextNode) element).Text);
            else if (element is IAttribute) WriteAttribute(writer, (IAttribute) element);
            else if (element is IElement)
            {
                var el = (IElement)element;
                el.Prepare();
                if (!el.IsVisible) return;
                WriteStartTag(writer,el);
                WriteChildren(writer, el);
                WriteEndTag(writer, el);
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
