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
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenRasta.Codecs.WebForms;
using OpenRasta.Testing;

namespace OpenRasta.Tests.Unit.Codecs
{
    public class when_rewriting_directives_with_the_page_parser : context
    {
        [Test]
        public void names_in_brackets_are_resolved()
        {
            var typeName = "OpenRasta.Codecs.WebForms.ResourceView<System.String>";

            OpenRastaPageParserFilter.GetTypeFromCSharpType(typeName,null)
                .ShouldBe<ResourceView<string>>();

        }
        [Test]
        public void nested_types_are_resolved()
        {
            var typeName = "OpenRasta.Codecs.WebForms.ResourceView<System.Collections.Generic.KeyValuePair<System.String,System.String>>";

            OpenRastaPageParserFilter.GetTypeFromCSharpType(typeName,null)
                .ShouldBe<ResourceView<KeyValuePair<string,string>>>();

        }
        public void types_are_resolved_against_imported_namespaces()
        {
            var typename = "String";
            OpenRastaPageParserFilter.GetTypeFromCSharpType(typename, new[] {"System"})
                .ShouldBe<string>();
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
