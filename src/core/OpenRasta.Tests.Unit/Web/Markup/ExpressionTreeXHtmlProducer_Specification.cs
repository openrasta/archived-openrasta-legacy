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
using System.Security.Principal;
using NUnit.Framework;
using OpenRasta.Collections;
using OpenRasta.Testing;
using OpenRasta.Tests.Unit.Fakes;
using OpenRasta.Web.Markup;
using OpenRasta.Web.Markup.Modules;

namespace ExpressionTreeXHtmlProducer_Specification
{
    public class when_building_forms : context
    {
        [Test]
        public void the_form_tag_is_written_for_the_correct_resource_uri() { }
    }

    public class when_building_textboxes_based_on_types : xhtml_context
    {
        [Test]
        public void a_reference_type_property_is_identified_correctly()
        {
            XHtml.TextBox<Customer>(c => c.FirstName)
                .Name
                .ShouldBe("Customer.FirstName");
        }

        [Test]
        public void a_reference_type_property_is_identified_correctly_on_password()
        {
            XHtml.Password<Customer>(c => c.FirstName)
                .Name
                .ShouldBe("Customer.FirstName");
        }

        [Test]
        public void a_value_type_property_is_identified_correctly()
        {
            XHtml.TextBox<Customer>(c => c.DateOfBirth.Day)
                .Name
                .ShouldBe("Customer.DateOfBirth.Day");
        }
    }

    public class when_building_textboxes : xhtml_context
    {
        [Test]
        public void null_values_are_propagated()
        {
            var customer = new Customer();
            var textbox = XHtml.TextBox(() => customer.FirstName);

            textbox.Value.ShouldBeNull();
        }

        [Test]
        public void the_correct_html_fragment_is_generated()
        {
            var customer = new Customer {FirstName = "John"};
            var textbox = XHtml.TextBox(() => customer.FirstName);

            textbox.OuterXml
                .ShouldContain("<input")
                .ShouldContain("type=\"text\"")
                .ShouldContain("value=\"John\"")
                .ShouldContain("name=\"Customer.FirstName\"")
                .ShouldContain("/>");
        }

        [Test]
        public void the_property_name_is_written_correctly()
        {
            var customer = new Customer {FirstName = "John"};
            var textbox = XHtml.TextBox(() => customer.FirstName);

            textbox.Name.ShouldBe("Customer.FirstName");
        }

        [Test]
        public void the_property_type_is_set_to_text()
        {
            var customer = (new Customer {FirstName = "John"});
            var textbox = XHtml.TextBox(() => customer.FirstName);
            textbox.Type.ShouldBe(InputType.Text);
        }

        [Test]
        public void the_property_value_is_written_correctly()
        {
            var customer = new Customer {FirstName = "John"};
            var textbox = XHtml.TextBox(() => customer.FirstName);

            textbox.Value.ShouldBe("John");
        }
    }

    public class when_building_select : xhtml_context
    {
        [Test]
        public void a_dictionary_is_rendered_as_options()
        {
            var t = new Test();
            var select = XHtml.Select(() => t.Enum,
                                            new Dictionary<string, string> { { "key1", "value1" }, { "key2", "value2" } });
            var first = select.ChildNodes[0].ShouldBeOfType<IOptionElement>();
            first.InnerText.ShouldBe("value1");
            first.Value.ShouldBe("key1");

            var second = select.ChildNodes[1].ShouldBeOfType<IOptionElement>();
            second.InnerText.ShouldBe("value2");
            second.Value.ShouldBe("key2");
        }

        [Test]
        public void an_enum_property_gets_enumeration_values_set()
        {
            var target = new Test { Enum = AttributeTargets.All };
            var select = XHtml.Select(() => target.Enum);
            select.Name.ShouldBe("Test.Enum");
            select.ChildNodes.Cast<IOptionElement>().SingleOrDefault(x => x.InnerText == "All")
                .ShouldNotBeNull()
                .Selected.ShouldBeTrue();
            select.ChildNodes.Count.ShouldBe(Enum.GetNames(typeof (AttributeTargets)).Length);
        }
        [Test]
        public void a_nullable_enum_property_sets_to_null_selects_an_empty_option_element_in_first_position()
        {
            var target = new Test();
            var select = XHtml.Select(() => target.NullableEnum);
            select.Name.ShouldBe("Test.NullableEnum");
            select.ChildNodes.Cast<IOptionElement>().SingleOrDefault(x => x.InnerText == "")
                .ShouldNotBeNull()
                .Selected.ShouldBeTrue();
            select.ChildNodes.Count.ShouldBe(Enum.GetNames(typeof(AttributeTargets)).Length+1);
        }
    }
    public class when_building_checkboxes:xhtml_context
    {
        [Test]public void a_checkbox_has_the_value_specified_in_the_property()
        {
            var target = new Test {IsSelected = true};
            var checkbox = XHtml.CheckBox(() => target.IsSelected);

            checkbox.Type.ShouldBe(InputType.CheckBox);
            checkbox.Checked.ShouldBeTrue();
            checkbox.Name.ShouldBe("Test.IsSelected");
            checkbox.ToString().ShouldContain("<input type=\"checkbox\"");
        }
    }

    public class Test
    {
        public AttributeTargets Enum { get; set; }
        public AttributeTargets? NullableEnum { get; set; }
        public bool IsSelected { get; set; }
    }
    

    public class xhtml_context : context
    {
        public XhtmlAnchor XHtml = new XhtmlAnchor(null,null,
                                                   () => new GenericPrincipal(new GenericIdentity("bla"), new string[0]));
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