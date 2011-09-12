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
using System.Reflection;
using System.Text;
using OpenRasta.DI;
using OpenRasta.IO;
using OpenRasta.Testing;
using OpenRasta.TypeSystem;
using OpenRasta.Web;

namespace OpenRasta.Codecs
{
    public abstract class media_type_reader_context<TCodec> : codec_context<TCodec>
        where TCodec : ICodec
    {
        object _theResult;

        protected IRequest Request
        {
            get { return Context.Request; }
        }

        protected void given_request_content_type(string mediaType)
        {
            Request.Entity.ContentType = new MediaType(mediaType);
        }

        protected void given_request_stream(string requestData, Encoding encoding)
        {
            given_request_stream(stream =>
            {
                using (var sw = new DeterministicStreamWriter(stream, encoding, StreamActionOnDispose.None))
                {
                    sw.Write(requestData);
                }
            });
        }

        protected void given_request_stream(Action<Stream> writer)
        {
            Request.Entity.Stream.Position = 0;

            writer(Request.Entity.Stream);
            Request.Entity.Stream.Position = 0;
        }

        protected void given_request_stream(string requestData)
        {
            given_request_stream(requestData, Encoding.UTF8);
        }


        protected T then_decoding_result<T>()
        {
            _theResult.ShouldNotBeNull();
            _theResult.ShouldNotBe(Missing.Value);
            _theResult.ShouldBeAssignableTo<T>();
            return (T)_theResult;
        }

        protected void when_decoding<T>()
        {
            when_decoding<T>("entity");
        }

        protected void when_decoding<T>(string paramName)
        {
            var codecInstance = CreateCodec(Context);
            var codec = codecInstance as IMediaTypeReader;
            if (codec != null)
                _theResult = codec.ReadFrom(Context.Request.Entity,TypeSystems.Default.FromClr(typeof(T)), paramName);
            else
            {
                throw new NullReferenceException();
            }
        }
    }
}

#region Full license
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion