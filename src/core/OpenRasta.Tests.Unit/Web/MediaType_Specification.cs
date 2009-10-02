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
using NUnit.Framework;
using OpenRasta.Testing;
using OpenRasta.Web;

namespace MediaType_Specification
{
    [TestFixture]
    public class when_parsing_a_content_type
    {
        [Test]
        public void TheBaseTypeIsProcessedCorrectly()
        {
            MediaType content = new MediaType("application/xml");
            content.TopLevelMediaType.ShouldBe("application");
            content.Subtype.ShouldBe("xml");
            content.Quality.ShouldBe(1.0f);
        }
        [Test,SetCulture("fr-FR")]
        public void parsing_a_quality_value_when_using_another_culture_still_parse_the_dot_value()
        {
            var mediaType = new MediaType("application/xml;q=0.3");
            mediaType.Quality.ShouldBe(0.3f);
        }
    }

    [TestFixture]
    public class when_ordering_content_types
    {
        [Test]
        public void AWildcardForBaseTypeIsLessSpecificThanANonWildcard()
        {
            var ct = MediaType.Parse("*/xml,text/xml").ToList();
            ct[0].MediaType.ShouldBe("text/xml");
            ct[1].MediaType.ShouldBe("*/xml");
        }

        [Test]
        public void AWildcardHasLowerPriorityThanAny()
        {
            var ct = MediaType.Parse("application/*, */*, application/xhtml+xml").ToList();
            ct[0].MediaType.ShouldBe("application/xhtml+xml");
            ct[1].MediaType.ShouldBe("application/*");
            ct[2].MediaType.ShouldBe("*/*");
        }

        [Test]
        public void AWildcardIsConsideredAsHavingTheLowestPriority()
        {
            var ct = MediaType.Parse("*/*,text/plain;q=0.1").ToList();
            ct[0].MediaType.ShouldBe("text/plain");
            ct[1].MediaType.ShouldBe("*/*");
        }

        [Test]
        public void AWildcardSubTypeHasLowerPriorityThanASpecificSubType()
        {
            var ct = MediaType.Parse("application/*, application/xhtml+xml").ToList();
            ct[0].MediaType.ShouldBe("application/xhtml+xml");
        }

        [Test]
        public void TheApplicationXmlContentTypeIsOrderedAfterAnyOtherContentTypeOfSamePriority()
        {
            var ct = MediaType.Parse("application/xml,text/plain,image/jpeg;q=0.7").ToList();
            ct[0].MediaType.ShouldBe("text/plain");
            ct[1].MediaType.ShouldBe("application/xml");
            ct[2].MediaType.ShouldBe("image/jpeg");
        }

        [Test]
        public void TheApplicationXmlContentTypeIsOrderedAfterMoreSpecificTypes()
        {
            var ct = MediaType.Parse("application/xml, application/xhtml+xml").ToList();
            ct[0].MediaType.ShouldBe("application/xhtml+xml");
        }

        [Test]
        public void TheOrderingWorksAsPerRFC2616_14_1()
        {
            var contentTypes = MediaType.Parse("text/plain; q=0.5, text/html, text/x-dvi; q=0.8, text/x-c").ToList();

            contentTypes[0].MediaType.ShouldBe("text/x-c");
            contentTypes[1].MediaType.ShouldBe("text/html");
            contentTypes[2].MediaType.ShouldBe("text/x-dvi");
            contentTypes[3].MediaType.ShouldBe("text/plain");
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