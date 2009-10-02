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
using OpenRasta.Web.Markup.Attributes;

namespace OpenRasta.Web.Markup
{
    public static class CommonAttributeExtensions
    {
        public static T Accept<T>(this T attrib, string accept) where T:IAcceptAttribute
        {
            attrib.Accept.Add(new MediaType(accept));
            return attrib;
        }
        public static T Accept<T>(this T attrib, MediaType accept) where T : IAcceptAttribute
        {
            attrib.Accept.Add(accept);
            return attrib;
        }
        public static T AccessKey<T>(this T attrib, char key) where T : IAccessKeyAttribute
        {
            attrib.AccessKey = key;
            return attrib;
        }
        public static T Alt<T>(this T attrib, string alternativeText) where T : IAltAttribute
        {
            attrib.Alt = alternativeText;
            return attrib;
        }
        public static T TabIndex<T>(this T attrib, int key) where T : ITabIndexAttribute
        {
            attrib.TabIndex = key;
            return attrib;
        }
        public static T Disabled<T>(this T attrib) where T : IDisabledAttribute
        {
            attrib.Disabled = true;
            return attrib;
        }
        public static T ReadOnly<T>(this T attrib) where T : IReadOnlyAttribute
        {
            attrib.ReadOnly = true;
            return attrib;
        }
        public static T Src<T>(this T attrib, string src) where T : ISrcAttribute
        {
            attrib.Src = new Uri(src,UriKind.RelativeOrAbsolute);
            return attrib;
        }
        public static T Src<T>(this T attrib, Uri src) where T : ISrcAttribute
        {
            attrib.Src = src;
            return attrib;
        }
        public static T Name<T>(this T attrib, string name) where T : INameAttribute
        {
            attrib.Name = name;
            return attrib;
        }

        public static T Size<T>(this T attrib, int size) where T : ISizeAttribute
        {
            attrib.Size = size;
            return attrib;
        }

        public static T Value<T>(this T attrib, string value) where T : IValueAttribute
        {
            attrib.Value = value;
            return attrib;
        }
        public static T Label<T>(this T attrib, string label) where T : ILabelAttribute
        {
            attrib.Label = label;
            return attrib;
        }
        public static T ID<T>(this T attrib, string id) where T : IIDAttribute
        {
            attrib.ID = id;
            return attrib;
        }
        public static T Type<T>(this T attrib, MediaType type) where T : ITypeAttribute
        {
            attrib.Type = type;
            return attrib;
        }
        public static T Type<T>(this T attrib, string mediaType) where T : ITypeAttribute
        {
            attrib.Type = new MediaType(mediaType);
            return attrib;
        }
        public static T CharSet<T>(this T attrib, string charSet) where T : ICharSetAttribute
        {
            attrib.CharSet = charSet;
            return attrib;
        }
        public static T Title<T>(this T attrib, string title) where T : ITitleAttribute
        {
            attrib.Title = title;
            return attrib;
        }
        public static T Rel<T>(this T attrib, string rel) where T : ILinkRelationshipAttribute
        {
            attrib.Rel.Add(rel);
            return attrib;
        }
        public static T Rev<T>(this T attrib, string rev) where T : ILinkRelationshipAttribute
        {
            attrib.Rev.Add(rev);
            return attrib;
        }
        public static T Href<T>(this T attrib, string href) where T : IHrefAttribute
        {
            attrib.Href = new Uri(href,UriKind.RelativeOrAbsolute);
            return attrib;
        }
        public static T Href<T>(this T attrib, Uri href) where T : IHrefAttribute
        {
            attrib.Href = href;
            return attrib;
        }
        public static T HrefLang<T>(this T attrib, string hrefLang) where T : IHrefAttribute
        {
            attrib.HrefLang = hrefLang;
            return attrib;
        }
        public static T Media<T>(this T attrib, string media) where T : IMediaAttribute
        {
            attrib.Media.Add(media);
            return attrib;
        }
        public static T LongDesc<T>(this T attrib, Uri longDesc) where T : ILongDescAttribute
        {
            attrib.LongDesc = longDesc;
            return attrib;
        }
        public static T LongDesc<T>(this T attrib, string longDescUri) where T : ILongDescAttribute
        {
            attrib.LongDesc = new Uri(longDescUri, UriKind.RelativeOrAbsolute);
            return attrib;
        }

        public static T Width<T>(this T attrib, string width) where T : IWidthAttribute
        {
            attrib.Width = width;
            return attrib;
        }
        public static T Height<T>(this T attrib, string height) where T : IWidthHeightAttribute
        {
            attrib.Height = height;
            return attrib;
        }
        public static T Width<T>(this T attrib, int width) where T : IWidthAttribute
        {
            attrib.Width = width.ToString();
            return attrib;
        }
        public static T Height<T>(this T attrib, int height) where T : IWidthHeightAttribute
        {
            attrib.Height = height.ToString();
            return attrib;
        }
        public static T Cite<T>(this T attrib, Uri cite) where T : ICiteAttribute
        {
            attrib.Cite = cite;
            return attrib;
        }
        public static T Cite<T>(this T attrib, string cite) where T : ICiteAttribute
        {
            attrib.Cite = new Uri(cite,UriKind.RelativeOrAbsolute);
            return attrib;
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
