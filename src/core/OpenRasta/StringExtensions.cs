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
using System.Text;

namespace OpenRasta
{
    public static class StringExtensions
    {
        public static bool EqualsOrdinalIgnoreCase(this string target, string compare)
        {
            return string.Compare(target, compare, StringComparison.OrdinalIgnoreCase) == 0;
        }
        public static bool IsEmpty(this string target)
        {
            return target == string.Empty;
        }

        public static bool IsNullOrEmpty(this string target)
        {
            return string.IsNullOrEmpty(target);
        }

        public static bool IsNullOrWhiteSpace(this string target)
        {
            return string.IsNullOrEmpty(target) || target.IsWhiteSpace() ;
        }

        public static bool IsWhiteSpace(this string target)
        {
            return target.Trim() == string.Empty;
        }

        public static string FromBase64String(this string value)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(value));
        }

        public static Uri ToUri(this string target)
        {
            return ToUri(target, UriKind.RelativeOrAbsolute);
        }

        public static Uri ToUri(this string target, UriKind uriKind)
        {
            if (target == null)
                return null;
            return new Uri(target, uriKind);
        }

        public static string With(this string target, params object[] parameters)
        {
            if (target == null) return null;
            if (parameters == null || parameters.Length == 0)
                return target;
            return string.Format(target, parameters);
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