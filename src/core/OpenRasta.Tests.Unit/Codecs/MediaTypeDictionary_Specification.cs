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
using OpenRasta.Testing;
using NUnit.Framework;
using OpenRasta.Codecs;
using OpenRasta.Web;

namespace MediaTypeDictionary_Specification
{
    public class when_adding_media_types : media_type_context
    {
        [Test]
        public void adding_a_null_media_type_raises_an_error()
        {
            Executing(() => GivenMediaType(null, null))
                .ShouldThrow<ArgumentNullException>();
        }
    }
    public class when_matching_media_types : media_type_context
    {
        [Test]
        public void registering_a_specific_mediatype_and_matching_on_that_mediatype_returns_one_result()
        {
            GivenMediaType("application/xml", "xml");
            GivenMediaType("text/plain", "text");

            WhenMatching("application/xml");

            ThenTheResult
                .ShouldContain("xml");
            ThenTheResult
                .Count.ShouldBe(1);
        }
        [Test]
        public void registering_a_specific_media_type_and_matching_on_sub_type_wildcard_returns_two_results()
        {
            GivenMediaType("application/xml", "xml");
            GivenMediaType("application/xhtml+xml", "xhtml");
            GivenMediaType("text/plain", "text");

            WhenMatching("application/*");

            ThenTheResult.Count.ShouldBe(2);
            ThenTheResult.ShouldContain("xhtml");
            ThenTheResult.ShouldContain("xml");
        }
        [Test]
        public void matching_on_wildcard_returns_all_results()
        {
            GivenMediaType("application/xml", "xml");
            GivenMediaType("application/xhtml+xml", "xhtml");

            WhenMatching("*/*");

            ThenTheResult.Count.ShouldBe(2);
            ThenTheResult.ShouldContain("xhtml");
            ThenTheResult.ShouldContain("xml");
        }
        [Test]
        public void registering_two_media_types_with_different_values_is_supported()
        {
            GivenMediaType("text/plain", "text1");
            GivenMediaType("text/plain", "text2");

            WhenMatching("text/plain");

            ThenTheResult.Count.ShouldBe(2);
            ThenTheResult.ShouldContain("text1");
            ThenTheResult.ShouldContain("text2");
        }
        [Test]
        public void registering_the_same_media_type_and_associated_value_adds_it_only_once()
        {
            GivenMediaType("text/plain", "text1");
            GivenMediaType("text/plain", "text1");

            WhenMatching("text/plain");

            ThenTheResult.Count.ShouldBe(1);
            ThenTheResult.ShouldContain("text1");

        }
        [Test]
        public void an_item_with_a_wildcard_media_type_matches_any_media_type()
        {
            GivenMediaType("*/*","wildcard");

            WhenMatching("text/plain");

            ThenTheResult.ShouldContain("wildcard")
                .Count().ShouldBe(1);
        }
    }
    public class media_type_context : context
    {
        protected MediaTypeDictionary<string> _repository;

        protected override void SetUp()
        {
            base.SetUp();
            _repository = new MediaTypeDictionary<string>();
        }
        protected void GivenMediaType(string mediatype, string name)
        {
            _repository.Add(new MediaType(mediatype), name);
        }
        protected List<string> ThenTheResult;
        protected void WhenMatching(string mediaType)
        {
            ThenTheResult = new List<string>(_repository.Matching(new MediaType(mediaType)));
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
