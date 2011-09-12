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
using System.IO;
using System.Linq;
using System.Reflection;
using OpenRasta.IO;
using OpenRasta.TypeSystem;
using OpenRasta.Web;

namespace OpenRasta.Codecs
{
    [MediaType("application/octet-stream;q=0.5")]
    [MediaType("*/*;q=0.1")]
    [SupportedType(typeof(IFile))]
    [SupportedType(typeof(Stream))]
    [SupportedType(typeof(byte[]))]
    public class ApplicationOctetStreamCodec : Codec, IMediaTypeReader, IMediaTypeWriter
    {
        public object ReadFrom(IHttpEntity request, IType destinationType, string destinationName)
        {
            if (destinationType.IsAssignableTo<IFile>())
                return new HttpEntityFile(request);
            if (destinationType.IsAssignableTo<Stream>())
                return request.Stream;
            if (destinationType.IsAssignableTo<byte[]>())
                return request.Stream.ReadToEnd();
            return Missing.Value;
        }

        public void WriteTo(object entity, IHttpEntity response, string[] codecParameters)
        {
            if (!GetWriters(entity, response).Any(x => x))
                throw new InvalidOperationException();
        }


        static bool TryProcessAs<T>(object target, Action<T> action) where T : class
        {
            var typedTarget = target as T;
            if (typedTarget != null)
            {
                action(typedTarget);
                return true;
            }
            return false;
        }

        static void WriteFileWithFilename(IFile file, string disposition, IHttpEntity response)
        {
            var contentDispositionHeader = response.Headers.ContentDisposition ?? new ContentDispositionHeader(disposition);

            if (!string.IsNullOrEmpty(file.FileName))
                contentDispositionHeader.FileName = file.FileName;
            if (!string.IsNullOrEmpty(contentDispositionHeader.FileName) || contentDispositionHeader.Disposition != "inline")
                response.Headers.ContentDisposition = contentDispositionHeader;
            if (file.ContentType != null && file.ContentType != MediaType.ApplicationOctetStream
                || (file.ContentType == MediaType.ApplicationOctetStream && response.ContentType == null))
                response.ContentType = file.ContentType;
            
            using (var stream = file.OpenStream())
                stream.CopyTo(response.Stream);
        }

        IEnumerable<bool> GetWriters(object entity, IHttpEntity response)
        {
            yield return TryProcessAs<IDownloadableFile>(entity, file => WriteFileWithFilename(file, "attachment", response));
            yield return TryProcessAs<IFile>(entity, file => WriteFileWithFilename(file, "inline", response));
            yield return TryProcessAs<Stream>(entity, stream => stream.CopyTo(response.Stream));
            yield return TryProcessAs<byte[]>(entity, bytes => response.Stream.Write(bytes));
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