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
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using OpenRasta.Codecs.WebForms;
using OpenRasta.Collections.Specialized;
using OpenRasta.Testing;

namespace WebFormsCodec_Specification
{
    public class when_there_are_no_codec_parameters_or_uri_names
    {
        [Test]
        public void the_first_page_with_a_name_of_index_is_selected()
        {
            WebFormsCodec.GetViewVPath(
                new { notanindex = "page.aspx", index = "anotherpage.aspx" }.ToCaseInvariantDictionary(), null, null)
                .ShouldBe("anotherpage.aspx");
        }
        [Test]
        public void IfThereIsNoRequestParametersAndNoIndexThePageWillNotBeRendered()
        {
            WebFormsCodec.GetViewVPath(
                new { notanindex = "page.aspx", notanindexeither = "anotherpage.aspx" }.ToCaseInvariantDictionary(), null, null)
                .ShouldBeNull();
        }

        [Test]
        public void the_page_is_not_found_when_there_is_no_renderer_parameter()
        {
            WebFormsCodec.GetViewVPath(new Dictionary<string, string>(), null, null)
                .ShouldBeNull();
        }
    }

    public class when_there_are_codec_parameters_but_no_uri_name
    {
        [Test]
        public void an_unrecognized_view_name_is_not_selecting_the_default()
        {
            WebFormsCodec.GetViewVPath(
                new { index = "page.aspx" }.ToCaseInvariantDictionary(),
                new[] { "view1" }, null)
                .ShouldBeNull();

        }

        [Test]
        public void the_matching_on_codec_parameter_is_case_insensitive()
        {
            WebFormsCodec.GetViewVPath(new { View1 = "page.aspx" }.ToCaseInvariantDictionary(), new[] { "view1" }, null)
                .ShouldBe("page.aspx");
        }

        [Test]
        public void the_last_codec_parameter_is_used()
        {
            WebFormsCodec.GetViewVPath(
                new { view1 = "page.aspx", view2 = "anotherpage.aspx" }.ToCaseInvariantDictionary(),
                new[] { "view1", "view2" }, null)
                .ShouldBe("anotherpage.aspx");
        }
    }

    internal class when_there_is_no_codec_parameter_but_uri_is_named
    {
        [Test]
        public void the_uri_name_is_used_to_select_the_view()
        {

            WebFormsCodec.GetViewVPath(
                new { view1 = "page.aspx", view2 = "anotherpage.aspx" }.ToCaseInvariantDictionary(),
                null, "view1")
                .ShouldBe("page.aspx");
        }
        [Test]
        public void the_named_uri_parameter_not_matching_a_registered_view_is_ignored_and_the_default_is_used()
        {
            WebFormsCodec.GetViewVPath(
                new { index = "page.aspx", view2 = "anotherpage.aspx" }.ToCaseInvariantDictionary(),
                null, "view1")
                .ShouldBe("page.aspx");
        }


    }
    public class when_there_are_codec_parameters_and_the_uri_has_a_name : context
    {

        [Test]
        public void the_default_view_is_used_for_unrecognized_view_names()
        {
            WebFormsCodec.GetViewVPath(
                new { index = "page.aspx" }.ToCaseInvariantDictionary(),
                new[] { "NewsTopics" },
                "NewsTopics")
                .ShouldBe("page.aspx");
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