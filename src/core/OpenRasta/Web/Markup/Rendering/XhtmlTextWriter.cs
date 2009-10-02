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
using System.IO;
using System.Text;

namespace OpenRasta.Web.Markup
{
    public class XhtmlTextWriter : IXhtmlWriter, ISupportsTextWriter
    {
        const string TAG_ATTR = " {0}=\"{1}\"";
        const string TAG_END = "</{0}>";
        const string TAG_START_BEGIN = "<{0}";
        const string TAG_START_END = ">";
        const string TAG_START_END_FINAL = " />";

        public XhtmlTextWriter(TextWriter source)
        {
            if (source == null)
                throw new ArgumentNullException("source", "source is null.");
            TextWriter = source;
        }

        public TextWriter TextWriter { get; private set; }

        public void BeginWriteStartElement(string TagName) { TextWriter.Write(TAG_START_BEGIN.With(TagName)); }

        public void EndWriteStartElement() { TextWriter.Write(TAG_START_END); }
        public void EndWriteStartElementFinal() { TextWriter.Write(TAG_START_END_FINAL); }

        public void WriteEndElement(string tagName) { TextWriter.Write(TAG_END.With(tagName)); }

        public void WriteAttributeString(string key, string value) { TextWriter.Write(TAG_ATTR.With(key, HtmlEncode(value))); }
        public void WriteString(string content) { TextWriter.Write(HtmlEncode(content)); }

        public void WriteUnencodedString(string content) { TextWriter.Write(content); }

        public static string HtmlEncode(string source)
        {
            if (source == null)
                return null;
            StringBuilder builder = new StringBuilder();
            foreach (var c in source)
            {
                switch (c)
                {
                    case '"':
                    case '\'':
                    case '&':
                    case '<':
                    case '>':
                        builder.Append("&#").Append((int) c).Append(';');
                        break;
                    default:
                        builder.Append(c);
                        break;
                }
            }
            return builder.ToString();
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