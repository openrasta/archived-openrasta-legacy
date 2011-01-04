#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion

using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using OpenRasta.Testing;
using OpenRasta.Web;

namespace MultipartWriter_Specification
{
    public class when_writing_a_multipart : context
    {
        readonly List<IHttpEntity> Entities = new List<IHttpEntity>();

        void GivenAFormEntity(string key, string value)
        {
            var entity = new MultipartHttpEntity();
            entity.Headers["Content-Disposition"] = "form-data; name=\"" + key + "\"";
            entity.Stream = new MemoryStream();
            var swriter = new StreamWriter(entity.Stream, Encoding.ASCII);
            swriter.Write(value);
            swriter.Flush();
            entity.Stream.Position = 0;
            Entities.Add(entity);
        }

        string ThenTheResult;

        void WhenWritingTheMultipartMessage()
        {
            foreach (var entity in Entities)
                Writer.Write(entity);
            Writer.Close();
            WriterStream.Position = 0;
            ThenTheResult = new StreamReader(WriterStream).ReadToEnd();
        }

        MultipartWriter Writer;
        Stream WriterStream;

        void GivenAMultipartWriter(string boundary, Encoding encoding)
        {
            WriterStream = new MemoryStream();
            Writer = new MultipartWriter(boundary, WriterStream, encoding);
        }

        [Test]
        public void writing_an_entity_generates_two_parts()
        {
            GivenAMultipartWriter("boundary", Encoding.ASCII);

            GivenAFormEntity("user", "username");

            WhenWritingTheMultipartMessage();

            ThenTheResult.ShouldBe(
                @"
--boundary
Content-Disposition: form-data; name=""user""

username
--boundary--
");
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