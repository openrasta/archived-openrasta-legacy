#region License

/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion

using System.IO;
using System.Text;

namespace OpenRasta.IO
{
    /// <summary>
    /// Implements a StreamWriter that does not close or dispose the stream when it doesn't own it.
    /// </summary>
    public class DeterministicStreamWriter : StreamWriter
    {
        readonly StreamActionOnDispose _closeAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeterministicStreamWriter"/> class.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="encoding">The encoding used when writing to the stream.</param>
        /// <param name="action">The action to take for the stream when the writer is closed.</param>
        public DeterministicStreamWriter(Stream stream, Encoding encoding, StreamActionOnDispose action)
            : base(stream, encoding)
        {
            _closeAction = action;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (BaseStream != null && disposing)
                    Flush();
            }
            finally
            {
                if (_closeAction == StreamActionOnDispose.Close && BaseStream != null && disposing)
                {
                    BaseStream.Close();
                }
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