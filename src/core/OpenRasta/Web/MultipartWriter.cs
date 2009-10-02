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
using System.Text;
using System.IO;
using OpenRasta.IO;

namespace OpenRasta.Web
{
    public class MultipartWriter : IDisposable
    {
        string _boundary;

        Stream _underlyingStream;
        byte[] _beginBoundary;
        byte[] _endBoundary;
        Encoding _encoding;
        public MultipartWriter(string boundary, Stream underlyingStream, Encoding encoding)
        {
            _boundary = boundary;
            _underlyingStream = underlyingStream;
            _encoding = encoding;
            _beginBoundary = encoding.GetBytes("--" + boundary +"\r\n" );
            _endBoundary = encoding.GetBytes("\r\n--" + boundary + "--\r\n");

        }


        public void Write(IHttpEntity formDataField)
        {
            WriteLine();
            WriteBoundary();
            foreach (var header in formDataField.Headers)
                WriteHeader(header);
            WriteContentLength(formDataField);
            WriteLine();
            WriteBody(formDataField);
        }

        private void WriteBoundary()
        {

            _underlyingStream.Write(_beginBoundary, 0, _beginBoundary.Length);
        }

        private void WriteContentLength(IHttpEntity formDataField)
        {
            if (formDataField.ContentLength != null)
                if (formDataField.Stream != null && formDataField.Stream.CanSeek)
                    WriteHeader(new KeyValuePair<string, string>("Content-Length", formDataField.Stream.Length.ToString()));
                else if (formDataField.Stream == null)
                    WriteHeader(new KeyValuePair<string, string>("Content-Length", "0"));
        }

        private void WriteBody(IHttpEntity formDataField)
        {
            formDataField.Stream.CopyTo(_underlyingStream);
        }

        private void WriteLine()
        {
            _underlyingStream.Write(new byte[] { 13, 10 }, 0, 2);
        }

        private void WriteHeader(KeyValuePair<string, string> header)
        {
            // TODO: handle split header scenarios and non-ascii chars
            string serializedHeader = header.Key + ": " + header.Value + "\r\n";
            byte[] resultHeader = _encoding.GetBytes(serializedHeader);
            _underlyingStream.Write(resultHeader);
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        public void Close()
        {
            _underlyingStream.Write(_endBoundary);
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
