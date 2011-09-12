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
using System.IO;
using System.Text;
using NUnit.Framework;
using OpenRasta.Testing;
using OpenRasta.Web;

namespace MultipartReader_Specification
{
    public class when_parsing_a_multipart_related_content_type : context
    {
        void CheckEntity(IHttpEntity entity, MediaType httpContentType, string expectedContent)
        {
            entity.ContentType
                .Matches(httpContentType)
                .ShouldBeTrue();
            string actualContent = new StreamReader(entity.Stream).ReadToEnd();
            actualContent.ShouldBe(expectedContent);
        }

        string TheTextIn(Stream stream) { return new StreamReader(stream).ReadToEnd(); }
        string TEXT_MULTIPART_NOHEAD = @"
ignore this text
--boundary42

text
--boundary42--";
        string TEXT_RFC1521_MULTIPART_MANY =
            @"
--boundary42
Content-Type: text/plain; charset=us-ascii 

text

--boundary42
Content-Type: text/richtext 

text
--boundary42
Content-Type: text/x-whatever 

text
...
--boundary42--

";
        [Test]
        public void a_part_without_a_boundary_will_throw()
        {
            
        }

        [Test]
        public void a_part_with_no_headers_has_its_content_parsed_correctly_and_no_header()
        {
            MultipartReader reader = new MultipartReader("boundary42",
                                                         new MemoryStream(Encoding.ASCII.GetBytes(TEXT_MULTIPART_NOHEAD)));

            var enumerator = reader.GetParts().GetEnumerator();
            enumerator.MoveNext()
                .ShouldBeTrue();

            enumerator.Current.Headers.Count.ShouldBe(0);
            TheTextIn(enumerator.Current.Stream).ShouldBe("text");
        }

        [Test]
        public void
            parsing_a_message_with_three_parts_and_skipping_the_first_one_gets_the_content_of_the_second_properly()
        {
            MultipartReader reader = new MultipartReader("boundary42",
                                                         new MemoryStream(
                                                             Encoding.ASCII.GetBytes(TEXT_RFC1521_MULTIPART_MANY)));

            var enumerator = reader.GetParts().GetEnumerator();
            enumerator.MoveNext();
            enumerator.MoveNext();
            CheckEntity(enumerator.Current, new MediaType("text/richtext"), "text");
            enumerator.Dispose();
        }

        [Test]
        public void parsing_an_entity_with_three_messages_returns_three_objects()
        {
            MultipartReader reader = new MultipartReader("boundary42",
                                                         new MemoryStream(
                                                             Encoding.ASCII.GetBytes(TEXT_RFC1521_MULTIPART_MANY)));

            var enumerator = reader.GetParts().GetEnumerator();
            enumerator.MoveNext();

            CheckEntity(enumerator.Current, new MediaType("text/plain") {CharSet = "us-ascii"}, "text\r\n");

            enumerator.MoveNext();
            CheckEntity(enumerator.Current, new MediaType("text/richtext"), "text");

            enumerator.MoveNext();
            CheckEntity(enumerator.Current, new MediaType("text/x-whatever"), "text\r\n...");

            enumerator.MoveNext().ShouldBeFalse();
            enumerator.Dispose();
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