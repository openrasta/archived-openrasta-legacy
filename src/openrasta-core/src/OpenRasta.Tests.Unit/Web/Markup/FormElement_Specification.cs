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
using NUnit.Framework;
using OpenRasta;
using OpenRasta.DI;
using OpenRasta.Testing;
using OpenRasta.Web.Markup;
using OpenRasta.Web.Markup.Modules;
using OpenRasta.Web.UriDecorators;

namespace FormElement_Specification
{
    public class when_generating_the_form_tag : markup_element_context<IFormElement>
    {
        [Test]
        public void non_html_methods_are_not_allowed_without_the_url_modifier_in_place()
        {
            Executing(() =>
                      WhenCreatingElement(
                          () => new FormElement(false).Method("PUT").Action("http://localhost/test"))
                ).ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void non_html_methods_are_rendered_as_url_modifiers_when_the_uri_decorator_is_active()
        {
            DependencyManager.GetService<IDependencyResolver>().AddDependency<IUriDecorator,HttpMethodOverrideUriDecorator>();

            WhenCreatingElement(() =>new FormElement(true).Action("http://localhost/test").Method("PUT"));

            ThenTheElementAsString.ShouldContain("method=\"POST\"");
            ThenTheElementAsString.ShouldContain("action=\"http://localhost/test!PUT");
        }

        [Test]
        public void the_default_method_is_get()
        {
            WhenCreatingElement(() => new FormElement(false).Action("htp://localhost/"));

            ThenTheElement.Method
                .ShouldBe("GET");
        }
        [Test]
        public void multiple_media_types_in_accept_results_in_a_comma_separated_list()
        {
            WhenCreatingElement(() => new FormElement(false).Accept("text/html").Accept("application/xhtml+xml"));

            ThenTheElementAsString.ShouldContain("accept=\"text/html,application/xhtml+xml\"");
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