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

namespace OpenRasta.Web
{
    public static class UriExtensions
    {
        public static string[] GetSegments(this Uri uri)
        {
#if SILVERLIGHT
            return uri.AbsolutePath.Split('/').Where(s => s != string.Empty).ToArray();
#else
            return uri.Segments;
#endif
        }
        public static Uri IgnoreAuthority(this Uri uri)
        {
            UriBuilder builder = new UriBuilder(uri);
            builder.Host = "uritemplate";
            return builder.Uri;
        }
        public static Uri IgnorePortAndAuthority(this Uri uri)
        {
            UriBuilder builder = new UriBuilder(uri);
            builder.Host = "uritemplate";
            builder.Port = 80;
            return builder.Uri;
        }

        public static Uri IgnoreSchemePortAndAuthority(this Uri uri)
        {
            UriBuilder builder = new UriBuilder(uri);
            builder.Scheme = "http:";
            builder.Host = "uritemplate";
            builder.Port = 80;
            return builder.Uri;
        }
        public static Uri ReplaceAuthority(this Uri uri, Uri baseUri)
        {
            if (baseUri == null)
                return uri;
            UriBuilder builder = new UriBuilder(uri);
            builder.Host = baseUri.Host;
            builder.Port = baseUri.Port;
            return builder.Uri;
        }
        public static Uri EnsureHasTrailingSlash(this Uri uri)
        {
            if (uri.Segments.Length > 1 && !uri.Segments[uri.Segments.Length - 1].EndsWith("/"))
            {
                var builder = new UriBuilder(uri);
                builder.Path += "/";
                uri = builder.Uri;
            }
            return uri;
        }
        public static Uri MakeAbsolute(this Uri uri, string baseUri)
        {
            return uri.MakeAbsolute(new Uri(baseUri, UriKind.Absolute));
        }
        public static Uri MakeAbsolute(this Uri uri, Uri baseUri)
        {
            return new Uri(baseUri,uri);
        }
        public static Uri ForView(this Uri uri, string viewName)
        {
            UriBuilder builder = new UriBuilder(uri);
            builder.Path += ";" + viewName;
            return builder.Uri;
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
