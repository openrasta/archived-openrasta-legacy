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
using System.IO.Compression;
using NUnit.Framework;
using OpenRasta.IO;
using OpenRasta.Testing;

namespace LengthTrackingStream_Specification
{
    public class when_writing_to_a_non_seekable_stream : context
    {
        [Test]
        public void the_correct_number_of_bytes_is_recorded()
        {
            var stream = new DeflateStream(new MemoryStream(), CompressionMode.Compress);

            var tracker = new LengthTrackingStream(stream);

            tracker.Write(new byte[50]);

            tracker.Length
                .ShouldBe(50);
        }
    }

    public class when_writing_to_a_seekable_stream : context
    {
        [Test]
        public void the_correct_number_of_bytes_is_recorded()
        {
            var stream = new MemoryStream();
            var tracker = new LengthTrackingStream(stream);

            stream.Write(new byte[2 << 4]);

            tracker.Length
                .ShouldBe(stream.Length)
                .ShouldBe(32);
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