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
using System.Text;
using OpenRasta.TypeSystem;
using OpenRasta.Web;

namespace OpenRasta.Codecs
{
    // see rfc2616 for text/plain definition.
    // We completely ignore the Accept-Charset for now.
    [MediaType("text/plain;q=0.5")]
    [SupportedType(typeof(string))]
    public class TextPlainCodec : IMediaTypeWriter, IMediaTypeReader
    {
        const string ENCODING_ISO_8859_1 = "ISO-8859-1";
        readonly Dictionary<IHttpEntity, string> _values = new Dictionary<IHttpEntity, string>();
        public object Configuration { get; set; }

        public object ReadFrom(IHttpEntity request, IType destinationType, string destinationParameterName)
        {
            if (request.ContentLength == 0)
                return string.Empty;
            if (!_values.ContainsKey(request))
            {
                var encoding = DetectTextEncoding(request);

                string result = new StreamReader(request.Stream, encoding).ReadToEnd();
                _values.Add(request, result);
            }
            return _values[request];
        }

        public void WriteTo(object entity, IHttpEntity response, string[] parameters)
        {
            var entityString = entity.ToString();
            
            var encodedText = Encoding.GetEncoding(ENCODING_ISO_8859_1).GetBytes(entityString);
            response.ContentType = new MediaType("text/plain;charset=ISO-8859-1");
            response.ContentLength = encodedText.Length;
            response.Stream.Write(encodedText, 0, encodedText.Length);
        }

        Encoding DetectTextEncoding(IHttpEntity request)
        {
            Encoding encoding;
            try
            {
                encoding = Encoding.GetEncoding(request.ContentType.CharSet);
            }
            catch
            {
                return Encoding.UTF8;
                // we always default to UTF8 and try to decode.
                // Reason is that the text codec is used by multipart, and browsers send UTF-8 by default.
                // TODO: Log an error
            }
            return encoding;
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