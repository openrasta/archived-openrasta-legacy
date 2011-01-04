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
using OpenRasta.Testing;
using OpenRasta.Web;
using OpenRasta.Web.Markup.Attributes;
using OpenRasta.Web.Markup.Elements;
using OpenRasta.Web.Markup;

namespace XhtmlDOM_Specification
{
    public class when_creating_list_of_attributes : context
    {
        [Test]
        public void any_attribute_with_simple_types_is_created_automatically()
        {
            var attribs = new XhtmlAttributeCollection();
            attribs.Count.ShouldBe(0);

            attribs.SetAttribute("name", "value");

            attribs["name"].ShouldBeOfType<IAttribute<string>>();
        }
        [Test]
        public void list_of_media_types_are_of_type_nmtoken()
        {
            var attribs = new XhtmlAttributeCollection();
            attribs.GetAttribute<IList<MediaType>>("types").Add(MediaType.Xml);

            attribs["types"].SerializedValue.ShouldBe("application/xml");
        }
        [Test]
        public void attributes_not_in_initial_list_are_added_as_generic_strings()
        {
            var attribs = new XhtmlAttributeCollection
            {
                AllowedAttributes = new Dictionary<string, Func<IAttribute>>
                {
                    {"name", () => new PrimaryTypeAttributeNode<string>("name")},
                    {"value", () => new PrimaryTypeAttributeNode<int?>("value")}
                }
            };

            attribs.SetAttribute("name", "a name");
            attribs.SetAttribute<int?>("value", 3);

            attribs["name"].SerializedValue.ShouldBe("a name");
            attribs["value"].SerializedValue.ShouldBe("3");

            attribs.SetAttribute("unknownValue", "25");

            attribs["unknownValue"].SerializedValue.ShouldBe("25");
        }

    }
    
    public class when_creating_elements : context
    {
        [Test]
        public void an_element_is_created_with_the_correct_attributes()
        {
            var element = Document.CreateElement<IScriptElement>("script").Type(MediaType.Javascript);
            element.Defer = true;

            element.OuterXml.ShouldBe("<script type=\"text/javascript\" defer=\"defer\"></script>");
        }
        [Test]
        public void elements_without_content_model_are_rendered_without_a_closing_tag()
        {
            var element = Document.CreateElement<IEmptyElement>("br") as IEmptyElement;
            element.OuterXml.ShouldBe("<br />");
        }
        [Test]
        public void elements_with_uri_attributes_are_generated_properly()
        {
            var el = Document.CreateElement<IImgElement>("img");
            el.Src("image.jpg").OuterXml.ShouldBe("<img src=\"image.jpg\" />");
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
