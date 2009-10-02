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
using FormElement_Specification;
using NUnit.Framework;
using OpenRasta.Testing;
using OpenRasta.Web.Markup;
using OpenRasta.Web.Markup.Modules;

namespace SelectElement_Specification
{
    public class when_there_is_a_value : markup_element_context<ISelectElement>
    {
        public AttributeTargets PropertyReturningFalse { get{ return AttributeTargets.Interface;} }
        public void the_option_element_is_selected()
        {
            WhenCreatingElement(()=>((IXhtmlAnchor) null).Select(() => this.PropertyReturningFalse));

            ThenTheElement.ChildElements.OfType<IOptionElement>().Where(x => x.InnerText == "Interface").Single().Selected.ShouldBeTrue();
            ThenTheElement.ChildElements.OfType<IOptionElement>().Where(x => x.InnerText != "Interface").All(x => x.Selected.ShouldBeFalse());
            ThenTheElementAsString.Contains("<option value=\"Interface\" selected=\"selected\" />");
            ThenTheElementAsString.Contains("<option value=\"Method\" />");
        }
    }
    //public class when_setting_a_value_before_adding_option_elements : context
    //{
    //    [Test]
    //    public void the_value_is_stored()
    //    {
    //        var element = new SelectElement();
    //        element.Value = "25";
    //        element.Children.Add(new OptionElement {Content = "26"});
    //        element.Value.ShouldBe("25");
    //    }
    //    [Test]
    //    public void the_selected_property_is_set_on_the_option_element()
    //    {
    //        var element = new SelectElement();
    //        element.Value = "25";
    //        element.Children.Add(new OptionElement {Content = "25"});
    //        element.Children.First().Selected.ShouldBe(true);
    //    }
    //    [Test]
    //    public void the_selected_property_on_the_option_element_is_reset_if_the_assigned_value_is_different()
    //    {

    //        var element = new SelectElement();
    //        element.Value = "25";
    //        element.Children.Add(new OptionElement { Content = "26", Selected = true });
    //        element.Children.First().Selected.ShouldBe(false);
    //    }
    //    [Test]
    //    public void rendering_the_element_when_the_value_doesnt_match_an_option_results_in_an_error()
    //    {
    //        var element = new SelectElement() {Value = "24"};
    //        element.Children.Add(new OptionElement {Content = "25"});
    //        Executing(() => element.ToString())
    //            .ShouldThrow<InvalidOperationException>();
    //    }
    //}
    //public class when_setting_a_value_after_adding_option_elements : context
    //{
    //    [Test]
    //    public void the_value_is_stored()
    //    {
    //        var element = new SelectElement();
    //        element.Children.Add(new OptionElement { Content = "26" });
    //        element.Value = "25";
    //        element.Value.ShouldBe("25");
    //    }
    //    [Test]
    //    public void the_selected_property_is_set_on_the_option_element()
    //    {

    //        var element = new SelectElement();
    //        element.Children.Add(new OptionElement { Content = "25" });
    //        element.Value = "25";
    //        element.Children.First().Selected.ShouldBe(true);
    //    }
    //    [Test]
    //    public void the_value_is_stored_after_removing_an_option_element()
    //    {
    //        var element = new SelectElement();
    //        element.Children.Add(new OptionElement { Content = "25" });
    //        element.Value = "25";
            
    //        element.Children.RemoveAt(0);
    //        element.Value.ShouldBe("25");
    //    }
    //}
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
