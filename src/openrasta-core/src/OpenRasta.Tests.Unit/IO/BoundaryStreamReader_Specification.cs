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
using OpenRasta.IO;
using OpenRasta.Testing;

namespace BoundaryStreamReader_Specification
{
    public class when_reading_a_boundary_type_from_a_stream : stream_context
    {
        BoundaryStreamReader Reader;
        public void GivenABoundaryStreamReader(string boundary) { Reader = new BoundaryStreamReader(boundary, Stream); }
        public void GivenABoundaryStreamReader(string boundary, int bufferLength) { Reader = new BoundaryStreamReader(boundary, Stream, Encoding.ASCII, bufferLength); }

        void ThenTheNextPartShouldBeEmpty() { Reader.ReadNextPart().ShouldBe(new byte[0]); }

        void ThenTheNextPartShouldBe(byte[] p) { Reader.ReadNextPart().ShouldBe(p); }

        [Test]
        public void a_boundary_starting_at_the_beginning_of_the_stream_with_a_crlf_is_processed_correctly()
        {
            GivenAMemoryStreamContaining(TextInASCII("\r\n--boundary\r\ntext\r\n--boundary--"));
            GivenABoundaryStreamReader("boundary");

            ThenTheNextPartShouldBeEmpty();

            Encoding.ASCII.GetString(Reader.ReadNextPart()).ShouldBe("text");
        }

        [Test]
        public void a_boundary_starting_at_the_beginning_of_the_stream_without_a_crlf_is_processed_correctly()
        {
            GivenAMemoryStreamContaining(TextInASCII("--boundary\r\ntext\r\n--boundary--"));
            GivenABoundaryStreamReader("boundary");

            ThenTheNextPartShouldBeEmpty();

            Encoding.ASCII.GetString(Reader.ReadNextPart()).ShouldBe("text");
        }

        [Test]
        public void a_boundary_stream_defaults_to_ASCII_encoding()
        {
            GivenAMemoryStreamContaining(TextInASCII(""));
            GivenABoundaryStreamReader("bla");

            Reader.Encoding.ShouldBe(Encoding.ASCII);
        }

        [Test]
        public void a_boundary_with_whitepaces_at_the_end_is_supported()
        {
            GivenAMemoryStreamContaining(TextInASCII("a\r\n--boundary \r\ntext\r\n"));
            GivenABoundaryStreamReader("boundary");

            ThenTheNextPartShouldBe(TextInASCII("a"));
            Reader.ReadLine().ShouldBe("text");
        }

        [Test]
        public void a_line_is_read_and_the_stream_position_is_readjusted()
        {
            GivenAMemoryStreamContaining(TextInASCII("sentence\r\nsentence2"));

            GivenABoundaryStreamReader("b");

            Reader.ReadLine().ShouldBe("sentence");
            Stream.Position.ShouldBe(10);
        }

        [Test]
        public void a_stream_without_boundaries_and_with_crlf_is_returned_in_full()
        {
            GivenAMemoryStreamContaining(TextInASCII("once upon a time,\r\na BoundaryStreamReader...\r\n"));
            GivenABoundaryStreamReader("boundary");

            Reader.ReadNextPart().ShouldBe(TextInASCII("once upon a time,\r\na BoundaryStreamReader...\r\n"));
        }

        [Test]
        public void a_stream_without_boundaries_is_returned_in_full()
        {
            GivenAMemoryStreamContaining(TextInASCII("once upon a time, a BoundaryStreamReader..."));
            GivenABoundaryStreamReader("boundary");

            Reader.ReadNextPart().ShouldBe(TextInASCII("once upon a time, a BoundaryStreamReader..."));
        }

        [Test]
        public void a_truncated_read_will_continue_reading_and_parse_the_content()
        {
            GivenAMemoryStreamContaining(TextInASCII("abc\r\n--bb\r\n"));
            GivenABoundaryStreamReader("bb", 8);

            ThenTheNextPartShouldBe(TextInASCII("abc"));

            Reader.ReadLine().ShouldBeNull();
        }

        [Test]
        public void an_unrecognized_boundary_is_correctly_ignored()
        {
            GivenAMemoryStreamContaining(TextInASCII("a\r\n--boundary2\r\nb\r\n--boundary\r\n"));
            GivenABoundaryStreamReader("boundary");

            ThenTheNextPartShouldBe(TextInASCII("a\r\n--boundary2\r\nb"));
        }

        [Test]
        public void building_a_boundary_stream_reader_necessitates_a_non_null_stream()
        {
            GivenANullStream();
            Executing(() => GivenABoundaryStreamReader("b"))
                .ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void building_a_boundary_stream_reader_necessitates_a_seekable_stream()
        {
            GivenANonSeekableStream();

            Executing(() => GivenABoundaryStreamReader("b"))
                .ShouldThrow<ArgumentException>();
        }

        [Test]
        public void
            read_line_followed_by_full_read_followed_by_read_line_reads_a_line_the_rest_of_the_part_and_the_line_of_the_next_part
            ()
        {
            GivenAMemoryStreamContaining(
                TextInASCII("--boundary\r\nline1\r\nline2\r\n--boundary\r\nline3\r\nline4\r\n--boundary--"));
            GivenABoundaryStreamReader("boundary");

            Reader.SeekToNextPart(); //ensures we get to our first part

            Reader.ReadLine().ShouldBe("line1");

            Reader.GetNextPart().ReadToEnd().ShouldBe(TextInASCII("line2"));

            Reader.ReadLine().ShouldBe("line3");

            Reader.GetNextPart().ReadToEnd().ShouldBe(TextInASCII("line4"));
        }

        [Test]
        public void reading_a_part_completely_and_seeking_skips_the_second_part()
        {
            GivenAMemoryStreamContaining(TextInASCII("--boundary\r\ncontent\r\n--boundary\r\ncontent2\r\n--boundary--"));
            GivenABoundaryStreamReader("boundary");

            Reader.GetNextPart().ReadToEnd().ShouldBe(TextInASCII("content"));
            Reader.SeekToNextPart();
            Reader.GetNextPart().ShouldBeNull();
        }

        [Test]
        public void reading_a_second_part_after_a_partially_read_first_part_results_in_the_correct_text()
        {
            GivenAMemoryStreamContaining(TextInASCII("--boundary\r\ncontent\r\n--boundary\r\ncontent2\r\n--boundary--"));
            GivenABoundaryStreamReader("boundary");

            Reader.GetNextPart().ReadByte().ShouldBe('c');
            Reader.ReadNextPart().ShouldBe(TextInASCII("content2"));
        }

        [Test]
        public void skipping_to_the_next_part_seeks_the_stream_to_the_beginning_of_the_first_part()
        {
            GivenAMemoryStreamContaining(TextInASCII("--boundary\r\ncontent\r\n--boundary\r\ncontent2\r\n--boundary--"));
            GivenABoundaryStreamReader("boundary");

            Reader.SeekToNextPart();
            Reader.GetNextPart().ReadToEnd().ShouldBe(TextInASCII("content"));
        }

        [Test]
        public void skipping_twice_reads_the_thrid_entity()
        {
            GivenAMemoryStreamContaining(
                TextInASCII("--boundary\r\ncontent\r\n--boundary\r\ncontent2\r\n--boundary\r\ncontent3\r\n--boundary--"));
            GivenABoundaryStreamReader("boundary");

            Reader.SeekToNextPart(); // at content
            Reader.SeekToNextPart();

            Reader.SeekToNextPart();
            Reader.GetNextPart().ReadToEnd().ShouldBe(TextInASCII("content3"));
        }
        [Test]
        public void reading_over_truncated_values_doesnt_corrupt_the_stream()
        {
            byte[] content = new byte[9000];
            for (int i = 0; i < content.Length; i++)
            {
                content[i] = (byte)(i % 255);
            }
            content[4095] = 13;
            GivenAMemoryStreamContaining(TextInASCII("--boundary\r\n"), content, TextInASCII("\r\n--boundary--"));

            GivenABoundaryStreamReader("boundary");

            Reader.SeekToNextPart();

            Reader.GetNextPart().ReadToEnd().ShouldBe(content);
        }
        [Test]
        public void the_buffer_of_the_reader_must_be_big_enough_to_seek_for_a_boundary()
        {
            GivenAMemoryStreamContaining(new byte[0]);
            Executing(() => GivenABoundaryStreamReader("four", 9))
                .ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Test]
        public void the_content_is_read_until_the_boundary_is_reached()
        {
            var unicodeText = TextInUTF16(
                @"????
?
??????
??");
            GivenAMemoryStreamContaining(TextInASCII("Header: value\r\n\r\n"), unicodeText,
                                         TextInASCII("\r\n--boundary\r\n"));

            GivenABoundaryStreamReader("boundary");

            Reader.ReadLine().ShouldBe("Header: value");
            Reader.ReadLine().ShouldBe("");
            Reader.ReadNextPart().ShouldHaveSameElementsAs(unicodeText);
            Reader.ReadLine().ShouldBeNull();
        }

        [Test]
        public void when_a_substream_is_open_reading_a_line_closes_the_previous_stream_and_moves_to_the_next_part()
        {
            GivenAMemoryStreamContaining(
                TextInASCII("--boundary\r\nline1\r\nline2\r\n--boundary\r\nline3\r\nline4\r\n--boundary--"));
            GivenABoundaryStreamReader("boundary");

            Reader.SeekToNextPart(); //ensures we get to our first part

            Reader.ReadLine().ShouldBe("line1");

            Reader.GetNextPart().ReadByte().ShouldBe('l');

            Reader.ReadLine().ShouldBe("line3");

            Reader.GetNextPart().ReadToEnd().ShouldBe(TextInASCII("line4"));
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