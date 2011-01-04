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
using OpenRasta.Testing;
using OpenRasta.Web.Markup;
using OpenRasta.Web.Markup.Elements;

namespace ElementExtensions_Specification
{
    public class when_setting_the_id_property : context
    {
        [Test]
        public void the_attribute_is_not_generated_when_id_is_null()
        {
            var element = new GenericElement("fake").ID(null);

            element.ID.ShouldBe(null);

            element.ToString()
                .ShouldNotContain("id=\"");
        }

        [Test]
        public void the_attribute_is_set_to_the_correct_value()
        {
            var element = new GenericElement("fake").ID("fakeid");

            element.ID.ShouldBe("fakeid");
        }
    }

    public class when_setting_classes : context
    {
        [Test]
        public void the_class_attribute_is_serialized_for_a_single_value()
        {
            var element = Document.CreateElement<IDivElement>().Class("fakeclass");

            element.OuterXml
                .ShouldContain("class=\"fakeclass\"");
        }

        [Test]
        public void the_class_attribute_is_serialized_for_multiple_chained_values()
        {
            var element = Document.CreateElement<IDivElement>().Class("fakeclass").Class("fakeclass2");

            element.OuterXml
                .ShouldContain("class=\"fakeclass fakeclass2\"");
        }

        [Test]
        public void the_class_property_contains_independent_values_when_added_as_one_string()
        {
            var element = Document.CreateElement<IDivElement>().Class("fakeclass fakeclass2");
            element.Class
                .ShouldContain("fakeclass")
                .ShouldContain("fakeclass2");
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