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
using OpenRasta.Collections;
using OpenRasta.Web.Markup.Elements;
using OpenRasta.Web.Markup.Modules;
using OpenRasta.Web.UriDecorators;

namespace OpenRasta.Web.Markup
{
    public class FormElement : GenericElement
    {
        Uri _originalAction;
        public FormElement(bool allowMethodOverrideSyntax)
            : base("form")
        {
            ContentModel.Clear();
            ContentModel.AddRange(Document.GetContentModelFor<IFormElement>());
            Attributes.AllowedAttributes = Document.GetAllowedAttributesFor<IFormElement>();
            IsMethodOverrideActive = allowMethodOverrideSyntax;
        }

        bool IsMethodOverrideActive { get; set; }
        string _originalMethod;
        public override string Method
        {
            get{ return _originalMethod ?? Attributes.GetAttribute("method");}
            set
            {
                
                if (!IsMethodOverrideActive && !IsMethodHtmlFriendly(value))
                    throw new InvalidOperationException("Cannot use any other method than POST and GET unless you register the {0} uri decorator".With(typeof(HttpMethodOverrideUriDecorator).Name));
                if (IsMethodOverrideActive && !IsMethodHtmlFriendly(value))
                {
                    Attributes.SetAttribute("method", "POST");
                    _originalMethod = value;
                    Action = Action;
                }
                else
                {
                    Attributes.SetAttribute("method",value);
                    _originalMethod = null;
                }
            }
        }
        public override Uri Action
        { 
            get
            {
                return _originalAction;
            }
            set
            {
                if (value == null || IsMethodHtmlFriendly(Method))
                {
                    _originalAction = value;
                    Attributes.SetAttribute("action",value);
                }
                else
                {
                    _originalAction = value;
                    Attributes.SetAttribute("action",AddHttpOverrider(value,Method));
                }
            }
        }
        static Uri AddHttpOverrider(Uri uri, string httpMethod)
        {
            var builder = new UriBuilder(uri);
            builder.Path += "!" + httpMethod;
            return builder.Uri;
        }

        static bool IsMethodHtmlFriendly(string method) { return method.EqualsOrdinalIgnoreCase("POST") || method.EqualsOrdinalIgnoreCase("GET"); }
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