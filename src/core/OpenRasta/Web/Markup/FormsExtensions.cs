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
using OpenRasta.DI;
using OpenRasta.Collections;
using OpenRasta.Web.Markup.Modules;
using OpenRasta.Web.UriDecorators;
using System.IO;

namespace OpenRasta.Web.Markup
{
    public static class FormsExtensions
    {
        private static bool IsUriMethodOverrideActive(IDependencyResolver resolver)
        {
                return resolver.HasDependencyImplementation(typeof (IUriDecorator), typeof (HttpMethodOverrideUriDecorator));
         
        }
        public static IFormElement Form(this IXhtmlAnchor anchor, object resourceInstance)
        {
            return new FormElement(IsUriMethodOverrideActive(anchor.Resolver)).Action(resourceInstance.CreateUri());
        }
        public static IFormElement Form<TResource>(this IXhtmlAnchor anchor)
        {
            return new FormElement(IsUriMethodOverrideActive(anchor.Resolver)).Action(anchor.Uris.CreateUriFor<TResource>());
        }
        public static IAElement Link<T>(this IXhtmlAnchor anchor)
        {
            return Document.CreateElement<IAElement>().Href(anchor.Uris.CreateUriFor<T>());
        }
        public static IAElement Link(this IXhtmlAnchor anchor, object instance)
        {
            return Document.CreateElement<IAElement>().Href(instance.CreateUri());
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
