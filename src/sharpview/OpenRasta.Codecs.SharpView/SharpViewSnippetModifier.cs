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
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using OpenRasta.CodeDom.Compiler;
using OpenRasta.Web.Markup;

namespace OpenRasta.Codecs.SharpView
{
    public class SharpViewSnippetModifier : ICodeSnippetTextModifier
    {
        const string REWRITE_WITHOUT_LAMBDA = "(global::System.Linq.Expressions.Expression<global::System.Func<global::OpenRasta.Web.Markup.IElement>>)({0})";
        const string REWRITE_WITH_LAMBDA = "(global::System.Linq.Expressions.Expression<global::System.Func<global::OpenRasta.Web.Markup.IElement>>)(()=>{0})";

        static readonly Regex SharpViewShortHand = new Regex(@"^\s*\(\)=>",
                                                             RegexOptions.Compiled | RegexOptions.Multiline);

        static readonly Regex SharpViewShortHandForCastToS = new Regex(@"^\s*\(s\)",
                                                                       RegexOptions.Compiled | RegexOptions.Multiline);

        public bool CanProcessObject(object source, object value)
        {
            return value is Expression<Func<IElement>>;
        }

        public string ProcessObject(object source, object value)
        {
            return new InlineSharpViewElement((Expression<Func<IElement>>)value).ToString();
        }

        public bool CanProcessString(string value)
        {
            return SharpViewShortHand.IsMatch(value) || SharpViewShortHandForCastToS.IsMatch(value);
        }

        /// <exception cref="InvalidOperationException">Something went really really wrong captain.</exception>
        public string ProcessString(string originalValue)
        {
            if (SharpViewShortHand.IsMatch(originalValue))
                return
                    string.Format(REWRITE_WITHOUT_LAMBDA, originalValue);
            if (SharpViewShortHandForCastToS.IsMatch(originalValue))
            {
                return
                    string.Format(REWRITE_WITH_LAMBDA, originalValue.Substring(originalValue.IndexOf("(s)") + 3));
            }
            throw new InvalidOperationException("Something went really really wrong captain.");
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