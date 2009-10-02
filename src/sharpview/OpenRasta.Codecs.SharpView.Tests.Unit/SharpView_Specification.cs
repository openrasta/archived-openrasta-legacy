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
using System.Linq.Expressions;
using System.Security.Principal;
using System.Threading;
using NUnit.Framework;
using OpenRasta.Codecs.SharpView;
using OpenRasta.Codecs.SharpView.Visitors;
using OpenRasta.Testing;
using OpenRasta.Web.Markup;
using OpenRasta.Web.Markup.Elements;
using OpenRasta.Web.Markup.Modules;

namespace SharpView_Specification
{
    public class when_using_foreach_with_inherited_sharpview_element : context
    {
        public class VideoElement : SharpViewElement
        {
            public VideoElement()
            {
                var values = new List<string> {"one", "two"};
                Root = () =>
                       div
                           [ul
                                [li.ForEach(values)
                                     [a.Href(values.Current())
                                          [values.Current()]
                                     ]
                                ]
                           ]
                    ;
            }
        }

        public string[] Property { get; set; }

        [Test]
        public void a_foreach_and_current_in_different_scopes_produces_the_expected_html_tree()
        {
            var list = new List<string> {"one", "two"};

            var element = new OneDivPerItemWithContent(list);
            element.OuterXml.ShouldBe("<div>one</div><div>two</div>");
        }

        [Test]
        public void a_foreach_and_current_in_same_scope_produces_the_expected_html_tree()
        {
            var list = new List<string> {"one", "two"};

            var element = new OneDivPerItemWithID(list);
            element.Prepare();
            element.OuterXml.ShouldBe("<div id=\"one\"></div><div id=\"two\"></div>");
        }

        [Test]
        public void a_foreach_and_multiple_current_items_in_custom_element_produces_the_correct_html_output()
        {
            var video = new VideoElement();
                var values = new [] {"one", "two"};
            video.OuterXml.ShouldBe("<div><ul><li><a href=\"one\">one</a></li><li><a href=\"two\">two</a></li></ul></div>");
        }


        [Test]
        public void a_null_collection_is_not_rendered()
        {
            new OneDivPerItemWithContent(null).OuterXml
                .ShouldBe(string.Empty);
        }

        [Test]
        public void an_empty_collection_is_not_rendered()
        {
            var list = new List<string>();
            new OneDivPerItemWithContent(list).OuterXml
                .ShouldBe(string.Empty);
        }
    }

    public class Customer
    {
        public List<Order> Orders { get; set; }
        public Order MainOrder { get; set; }
    }

    public class Order
    {
        public string ProductName { get; set; }
        public List<Line> Lines { get; set; }
    }
    public class Line
    {
        public string Description { get; set; }
    }
    public class when_using_foreach_with_inline_sharpview_element : context
    {
        [Test]
        public void a_foreach_and_multiple_current_items_producethe_correct_html_output()
        {
            var list = new List<string> {"one", "two"};
            var e = new InlineSharpViewElement(() => Document.CreateElement<IDivElement>().ForEach(list)
                                                         [
                                                         Document.CreateElement<IAElement>().Class(list.Current())[
                                                             list.Current()]
                                                         ]);
            e.OuterXml.ShouldBe("<div><a class=\"one\">one</a></div><div><a class=\"two\">two</a></div>");
        }

        [Test]
        public void a_foreach_applied_to_the_current_item_of_a_parent_foreach_are_processed()
        {
         Customer data = CreateCustomerOrderLines();
            var e = new InlineSharpViewElement(() => 
                Document.CreateElement<IDivElement>()
                    .Class(data.Orders.Current().ProductName)
                    .ForEach(data.Orders)
                    [Document.CreateElement<IDivElement>()
                             .Class(data.Orders.Current().Lines.Current().Description)
                             .ForEach(data.Orders.Current().Lines)
                ]);
            e.OuterXml.ShouldBe(
                "<div class=\"product1\">"+
                    "<div class=\"1\"></div>" +
                    "<div class=\"2\"></div>" +
                "</div><div class=\"product2\">"+
                    "<div class=\"1\"></div>" +
                    "<div class=\"2\"></div>" +
                "</div>"
                );
        }

        [Test]
        public void a_foreach_and_multiple_current_items_produce_the_correct_html_output1()
        {
            var anchor = new XhtmlAnchor(null, null, () => null);
            var data = new Customer
                       {
                           Orders = new List<Order>
                                    {
                                        new Order {ProductName = "product1"},
                                        new Order {ProductName = "product2"}
                                    }
                       };
            var e = new InlineSharpViewElement(() => anchor.TextBox(()=>data.Orders.Current().ProductName).ForEach(data.Orders));
            e.OuterXml.ShouldBe("<input type=\"text\" name=\"Customer.Orders:0.ProductName\" value=\"product1\" />"
                                + "<input type=\"text\" name=\"Customer.Orders:1.ProductName\" value=\"product2\" />");
        }

        [Test]
        public void nested_foreach_respect_the_index_values()
        {
            var anchor = new XhtmlAnchor(null, null, () => null);
            Customer data = CreateCustomerOrderLines();
            var e = new InlineSharpViewElement(() => Document.CreateElement<IDivElement>().ForEach(data.Orders)[
                    anchor.TextBox(()=>data.Orders.Current().Lines.Current().Description).ForEach(data.Orders.Current().Lines)
                ]);
            var elementString = e.OuterXml;
            elementString.ShouldBe(
                "<div>" +
                    "<input type=\"text\" name=\"Customer.Orders:0.Lines:0.Description\" value=\"1\" />" +
                    "<input type=\"text\" name=\"Customer.Orders:0.Lines:1.Description\" value=\"2\" />" +
                "</div><div>"+
                    "<input type=\"text\" name=\"Customer.Orders:1.Lines:0.Description\" value=\"1\" />" +
                    "<input type=\"text\" name=\"Customer.Orders:1.Lines:1.Description\" value=\"2\" />" +
                "</div>");
        }

        private Customer CreateCustomerOrderLines()
        {
            return new Customer
                       {
                           Orders = new List<Order>
                                        {
                                            new Order {
                                                          ProductName = "product1",
                                                          Lines = new List<Line>
                                                                      {
                                                                          new Line{Description="1"},
                                                                          new Line{Description="2"}
                                                                      }
                                                      },
                                            new Order {
                                                          ProductName = "product2",
                                                          Lines = new List<Line>
                                                                      {
                                                                          new Line{Description="1"},
                                                                          new Line{Description="2"}
                                                                      }
                                                      },
                                        }
                       };
        }
        [Test]
        public void a_root_node_is_rewritten()
        {
            var list = new List<string> {"one","two"};

            var inlineElement =
                new InlineSharpViewElement(() => Document.CreateElement<IDivElement>().ID(list.Current()).ForEach(list));
            inlineElement.Prepare();
            inlineElement.OuterXml.ShouldBe("<div id=\"one\"></div><div id=\"two\"></div>");
        }
        [Test]
        public void a_child_scope_is_rewritten()
        {
            var list = new List<string> { "one", "two" };

            var inlineElement =
                new InlineSharpViewElement(() =>
                    Document.CreateElement<IDivElement>()
                        [Document.CreateElement<IDivElement>()
                                 .ID(list.Current())
                                 .ForEach(list)
                        ]);
            inlineElement.Prepare();
            inlineElement.OuterXml.ShouldBe("<div><div id=\"one\"></div><div id=\"two\"></div></div>");
        }
        [Test]
        public void nested_foreach_clauses_are_rewritten()
        {
            var list = new List<string> { "one", "two" };
            var list2 = new List<string> { "three", "four" };

            var inlineElement = new InlineSharpViewElement(() =>
                Document.CreateElement<IDivElement>()
                        .Class(list.Current())
                        .ForEach(list)
                        [Document.CreateElement<IPElement>()
                                 .ID(list2.Current())
                                 .ForEach(list2)]
                );
            inlineElement.Prepare();
            inlineElement.OuterXml.ShouldBe("<div class=\"one\"><p id=\"three\"></p><p id=\"four\"></p></div><div class=\"two\"><p id=\"three\"></p><p id=\"four\"></p></div>");
        }

        [Test]
        public void using_the_foreach_operator_twice_results_in_an_exception()
        {
            var source1 = new[] {"one"};
            var source2 = new[] {"two"};

            Executing(
                () =>
                new InlineSharpViewElement(() => Document.CreateElement<IDivElement>()[Document.CreateElement<IDivElement>().ForEach(source1).ForEach(source2)])
                    .Prepare())
                .ShouldThrow<NotSupportedException>();
            Executing(
                () =>
                new InlineSharpViewElement(() => Document.CreateElement<IDivElement>().ForEach(source1).ForEach(source2))
                    .Prepare())
                .ShouldThrow<NotSupportedException>();
        }
    }

    public class when_using_if_with_inherited_sharpview_element : context
    {
        public class LoginElement : SharpViewElement
        {
            public LoginElement()
            {
                Root = () => div.If(User != null)[span.If(User.Identity)["Logged-in as " + User.Identity.Name]];
            }

            protected IPrincipal User { get; private set; }
        }

        [Test]
        public void if_scenarios_in_child_elements_are_rewritten_properly()
        {
            new LoginElement().OuterXml.ShouldBe(string.Empty);
        }
    }

    public class when_using_if_and_for_in_inline_element : context
    {
        [Test]
        public void the_foreach_is_not_executed_when_the_if_case_evaluates_to_false()
        {
            var strings = new[] {"one", "two"};
            var e =
                new InlineSharpViewElement(
                    () => Document.CreateElement<IDivElement>().ForEach(strings).Class("item").If(false));
            e.OuterXml.ShouldBe(string.Empty);
        }
    }

    public class when_using_if_with_inline_sharpview_element : context
    {
        public IPrincipal Principal { get; set; }

        private class nested
        {
            public subnested subnested;
        }

        private class subnested
        {
            public string property;
            public bool booleanValue;
        }

        [Test]
        public void a_boolean_sets_to_false_results_in_an_empty_element()
        {
            bool result = false;
            var inlineEelement = new InlineSharpViewElement(
                () => Document.CreateElement<IDivElement>().If(result));

            inlineEelement.OuterXml.ShouldBe(string.Empty);
        }

        [Test]
        public void a_comparison_on_a_member_returing_false_results_in_an_empty_string()
        {
            var inlineEelement = new InlineSharpViewElement(
                () => Document.CreateElement<IDivElement>().If(Principal.Identity.Name == "john"));

            inlineEelement.OuterXml.ShouldBe(string.Empty);
        }

        [Test]
        public void a_comparison_on_a_static_returning_false_results_in_an_empty_element()
        {
            var inlineEelement = new InlineSharpViewElement(
                () => Document.CreateElement<IDivElement>().If(Thread.CurrentPrincipal.Identity.Name == "john"));

            inlineEelement.OuterXml.ShouldBe(string.Empty);
        }

        [Test]
        public void access_to_a_null_field_that_is_an_interface_results_in_a_null_element()
        {
            IConvertible convertible = null;
            var e = new InlineSharpViewElement(() => Document.CreateElement<IDivElement>().If(convertible));
            e.OuterXml.ShouldBe(string.Empty);
        }

        [Test]
        public void an_if_evaluating_to_false_stops_the_rendering_of_indexers_before_the_next_method_call()
        {
            var inlineEelement = new InlineSharpViewElement(
                () => Document.CreateElement<IDivElement>().If(false)["test"]);

            inlineEelement.OuterXml.ShouldBe(string.Empty);
        }

        [Test]
        public void an_if_evaluating_to_false_stops_the_rendering_of_methods_before_the_next_method_call()
        {
            var inlineEelement = new InlineSharpViewElement(
                () => Document.CreateElement<IDivElement>().If(false).ID("test"));

            inlineEelement.OuterXml.ShouldBe(string.Empty);
        }

        [Test]
        public void an_if_pointing_to_an_empty_string_generates_an_empty_element()
        {
            string value = string.Empty;
            var e = new InlineSharpViewElement(() => Document.CreateElement<IDivElement>().If(value)["content"]);
            e.OuterXml.ShouldBe(string.Empty);
        }

        [Test]
        public void an_intermediate_null_value_results_in_a_null_element()
        {
            var nested = new nested();
            var inlineEelement = new InlineSharpViewElement(
                () => Document.CreateElement<IDivElement>().If(nested.subnested.property));
            inlineEelement.Prepare();
            inlineEelement.ChildNodes.Count.ShouldBe(0);
        }
        [Test]
        public void both_sides_of_conditionals_inside_ifs_are_propagating_null_values()
        {
            var nested = new nested();
            string value = string.Empty;
            var inlineEelement = new InlineSharpViewElement(
                () => Document.CreateElement<IDivElement>().If(nested.subnested.booleanValue || false));
            inlineEelement.Prepare();
            inlineEelement.ChildNodes.Count.ShouldBe(0);
        }
        [Test]
        public void multiple_methods_are_rewritten()
        {
            var Xhtml = new XhtmlAnchor(null, null, () => Thread.CurrentPrincipal);
            var e =
                new InlineSharpViewElement(
                    () => Document.CreateElement<IAElement>().If(false).ID("hi").Class("hello")["content"]);

            e.OuterXml.ShouldBe(string.Empty);
        }
    }
    public class when_building_property_paths_for_enumerators : context
    {
        [Test]
        public void the_prefix_and_suffix_starts_at_the_current_extension_method()
        {
            var customer = new Customer();
            Expression<Func<object>> expr = () => customer.Orders.Current().Lines;
            var property = new PropertyPathForIteratorVisitor().BuildPropertyPath(expr);
            property.TypePrefix.ShouldBe("Order");
            property.TypeSuffix.ShouldBe("Lines");
        }

        [Test]
        public void the_prefix_is_always_the_root_type_and_the_suffx_the_path_to_a_property()
        {
            var customer = new Customer();
            Expression<Func<object>> expr = () => customer.MainOrder.Lines;
            var property = new PropertyPathForIteratorVisitor().BuildPropertyPath(expr);
            property.TypePrefix.ShouldBe("Customer");
            property.TypeSuffix.ShouldBe("MainOrder.Lines");
        }
        [Test]
        public void indexers_are_serialzied_after_a_semi_column()
        {
            var customer = new Customer();
            Expression<Func<object>> expr = () => customer.Orders[0].Lines;
            var property = new PropertyPathForIteratorVisitor().BuildPropertyPath(expr);
            property.TypePrefix.ShouldBe("Customer");
            property.TypeSuffix.ShouldBe("Orders:0.Lines");
        }
    }
    public class OneDivPerItemWithID : SharpViewElement
    {
        private readonly IEnumerable<string> _strings;

        public OneDivPerItemWithID(IEnumerable<string> strings)
        {
            _strings = strings;
            Root = () => div.ForEach(_strings).ID(_strings.Current());
        }
    }

    public class OneDivPerItemWithContent : SharpViewElement
    {
        private readonly IEnumerable<string> _strings;

        public OneDivPerItemWithContent(IEnumerable<string> strings)
        {
            _strings = strings;
            Root = () => div.ForEach(_strings)[_strings.Current()];
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