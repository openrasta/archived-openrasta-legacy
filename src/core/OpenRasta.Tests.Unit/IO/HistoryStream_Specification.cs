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
using NUnit.Framework;
using OpenRasta.IO;
using OpenRasta.Testing;

namespace HistoryStream_Specification
{
    public class when_wrapping_a_stream_in_a_HistoryStream : stream_context
    {
        void GivenAMemoryStream() { Stream = new MemoryStream(); }

        HistoryStream HistoryStream;

        void SeekBy(int pos) { SeekBy(pos, SeekOrigin.Current); }

        void SeekBy(int pos, SeekOrigin origin) { HistoryStream.Seek(pos, origin); }

        void GivenAHistoryStream() { HistoryStream = new HistoryStream(Stream); }

        void GivenAHistoryStreamWithBufferSize(int size) { HistoryStream = new HistoryStream(Stream, size); }

        byte[] ReadBytes(int byteCount)
        {
            byte[] bytes = new byte[byteCount];
            var result = HistoryStream.Read(bytes, 0, bytes.Length);
            byte[] returnValue = new byte[result];
            Buffer.BlockCopy(bytes, 0, returnValue, 0, result);
            return returnValue;
        }

        [Test]
        public void a_read_followed_by_a_backward_seek_will_rehydrate_data_correctly_on_the_next_read()
        {
            GivenAMemoryStreamContaining(TextInUTF16(new string('x', 2048)), TextInASCII("\r\n--boundary"));
            GivenAHistoryStreamWithBufferSize(4096);
            // if buffer is 2048 we need another 2048 to trigger a response

            ReadBytes(5000).Length.ShouldBe(4096);
            Stream.Position.ShouldBe(4096);
            // now the buffer should be full, we seek back by half

            Executing(() => SeekBy(-2048))
                .ShouldCompleteSuccessfully();

            Stream.Position.ShouldBe(4096);
            // issueing a new read request should trigger a new read on the stream for the leftover text

            ReadBytes(5000).Length.ShouldBe(4096 - 2048 + 12);
            Stream.Position.ShouldBe(Stream.Length);
        }

        [Test]
        public void reading_cached_data_doesnt_change_the_underlying_stream_poisition()
        {
            GivenAMemoryStreamContaining(new byte[4096]);
            GivenAHistoryStreamWithBufferSize(2048);

            ReadBytes(2048).Length
                .ShouldBe(2048);
            Stream.Position
                .ShouldBe(2048);

            Executing(() => SeekBy(-1024))
                .ShouldCompleteSuccessfully(); // b is at 1024
            Stream.Position
                .ShouldBe(2048);

            ReadBytes(1000).Length
                .ShouldBe(1000);
            Stream.Position
                .ShouldBe(2048); // no change

            ReadBytes(2048).Length
                .ShouldBe(2048); // b is at 2024+2048 = 4072
            Stream.Position
                .ShouldBe(4072);
        }

        [Test]
        public void seeking_back_further_than_the_buffer_current_size_will_throw()
        {
            GivenAMemoryStreamContaining(new byte[4096]);
            GivenAHistoryStreamWithBufferSize(1024);

            ReadBytes(512);

            Executing(() => SeekBy(-513))
                .ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void seeking_back_further_than_the_buffer_maximum_size_will_throw()
        {
            GivenAMemoryStreamContaining(new byte[4096]);
            GivenAHistoryStreamWithBufferSize(4096);

            ReadBytes(4096);

            Executing(() => SeekBy(-4097))
                .ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void seeking_from_another_origin_than_current_will_throw()
        {
            GivenAMemoryStreamContaining(new byte[4096]);
            GivenAHistoryStreamWithBufferSize(1024);

            ReadBytes(30);

            Executing(() => SeekBy(1, SeekOrigin.Begin))
                .ShouldThrow<NotSupportedException>();
            Executing(() => SeekBy(1, SeekOrigin.End))
                .ShouldThrow<NotSupportedException>();
        }

        [Test]
        public void the_default_stream_has_a_buffer_of_4096_bytes()
        {
            GivenAMemoryStream();
            GivenAHistoryStream();

            HistoryStream.BufferSize.
                ShouldBe(4096);
        }

        [Test]
        public void the_memory_stream_is_read_only_and_seekable()
        {
            GivenAMemoryStream();
            GivenAHistoryStream();

            HistoryStream.CanRead.
                ShouldBeTrue();

            HistoryStream.CanWrite.
                ShouldBeFalse();

            HistoryStream.CanSeek.
                ShouldBeTrue();

            Executing(() => HistoryStream.Position = 0).ShouldThrow<NotSupportedException>();
            Executing(() => { var result = HistoryStream.Position; }).ShouldThrow<NotSupportedException>();
            Executing(() => { var result = HistoryStream.Length; }).ShouldThrow<NotSupportedException>();
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