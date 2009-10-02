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
using OpenRasta.Codecs;
using OpenRasta.Diagnostics;

namespace OpenRasta.Web
{
    public interface IMultipartHttpEntity : IHttpEntity
    {
        void SwapStream(string filepath);
        void SwapStream(Stream stream);
    }

    public class MultipartHttpEntity : IMultipartHttpEntity, IDisposable
    {
        ILogger Log { get; set; }

        public MultipartHttpEntity()
        {
            Headers = new HttpHeaderDictionary();
        }

        public MediaType ContentType
        {
            get { return Headers.ContentType; }
            set { Headers.ContentType = value; }
        }

        public long? ContentLength
        {
            get { return Headers.ContentLength; }
            set { Headers.ContentLength = value; }
        }

        private Stream _stream;
        public Stream Stream
        {
            get
            {
                if (_stream == null && File.Exists(_filePath))
                    _stream = File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return _stream;
            }
            set
            {
                _stream = value;
                _filePath = null;
            }
        }
        public IList<Error> Errors{ get; private set; }

        public HttpHeaderDictionary Headers { get; private set; }
        public ICodec Codec { get; set; }
        public object Instance { get; set; }

        private string _filePath = null;
        public void SwapStream(Stream stream)
        {
            Stream = stream;
        }
        public void SwapStream(string filepath)
        {
            _filePath = filepath;
            _stream = null;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_stream != null)
                    {
                        try
                        {
                            _stream.Dispose();
                        }
                        catch (ObjectDisposedException) { }
                        finally
                        {
                            _stream = null;
                        }
                    }
                    if (_filePath != null && File.Exists(_filePath))
                    {
                        try
                        {
                            File.Delete(_filePath);
                        }
                        catch (Exception e)
                        {
                            Log.Safe().WriteError("Could not delete file {0} after use. See exception for details.", _filePath);
                            Log.Safe().WriteException(e);
                        }
                        finally
                        {
                            _filePath = null;
                        }
                    }
                }

                _disposed = true;
            }
        }
        ~MultipartHttpEntity()
        {
            Dispose(false);
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
