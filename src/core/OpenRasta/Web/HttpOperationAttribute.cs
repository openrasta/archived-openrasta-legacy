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
using System.Reflection;

namespace OpenRasta.Web
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpOperationAttribute : Attribute
    {
        public HttpOperationAttribute()
        {
            ContentType = new MediaType("*/*");
        }

        public HttpOperationAttribute(HttpMethod method) : this()
        {
            Method = method.ToString();
        }

        public HttpOperationAttribute(string method) : this()
        {
            Method = method;
        }

        public string ForUriName { get; set; }
        public string Method { get; set; }
        public MediaType ContentType { get; set; }

        /// <summary>
        /// Tries to find an HttpOperation attribute on a method. 
        /// </summary>
        /// <param name="mi"></param>
        /// <returns>The instance of the HttpOperation attribute, or null if none were defined.</returns>
        public static HttpOperationAttribute Find(MethodInfo mi)
        {
            try
            {
                return GetCustomAttribute(mi, typeof (HttpOperationAttribute)) as HttpOperationAttribute;
            }
            catch
            {
                return null;
            }
        }

        public bool MatchesUriName(string uriName)
        {
            return string.Compare(ForUriName, uriName, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public bool MatchesHttpMethod(string httpMethod)
        {
            return string.CompareOrdinal(Method, httpMethod) == 0;
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