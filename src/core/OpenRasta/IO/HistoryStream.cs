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

namespace OpenRasta.IO
{
    /// <summary>
    /// Provides a stream over non-seekable streams that buffers all read calls
    /// and provide a seekable recent history of the stream.
    /// </summary>
    public class HistoryStream : Stream
    {
        readonly byte[] _buffer;
        readonly byte[] _tempBuffer;
        int _bufferLength;
        int _bufferPosition;

        public HistoryStream(Stream baseStream)
            : this(baseStream, 4096)
        {
        }

        public HistoryStream(Stream baseStream, int bufferSize)
        {
            _buffer = new byte[bufferSize];
            _tempBuffer = new byte[bufferSize];
            UnderlyingStream = baseStream;
        }

        public int BufferSize
        {
            get { return _buffer.Length; }
        }

        public override bool CanRead
        {
            get { return UnderlyingStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public Stream UnderlyingStream { get; private set; }

        public override void Flush()
        {
            // do nothing
        }

        public override int Read(byte[] targetBuffer, int offset, int count)
        {
            if (count > _buffer.Length)
                count = _buffer.Length;

            int existingBytesInBuffer = _bufferLength - _bufferPosition;

            int unusedBytesInBuffer = _buffer.Length - _bufferLength;

            int extraBytesNeededForRead = count - existingBytesInBuffer;

            if (extraBytesNeededForRead > 0)
            {
                if (extraBytesNeededForRead <= unusedBytesInBuffer)
                {
                    int readBytes = UnderlyingStream.Read(_buffer, _bufferLength, extraBytesNeededForRead);
                    _bufferLength += readBytes;
                    var numberOfBytesToSend = Math.Min(count, _bufferLength - _bufferPosition);
                    Buffer.BlockCopy(_buffer, _bufferPosition, targetBuffer, offset, numberOfBytesToSend);
                    _bufferPosition += numberOfBytesToSend;
                    return numberOfBytesToSend;
                }
                if (extraBytesNeededForRead > unusedBytesInBuffer)
                {
                    int newlyReadBytesFromStream = UnderlyingStream.Read(_tempBuffer, 0, extraBytesNeededForRead);

                    //  now check we're not in a case where the read is actually smaller than the avail space
                    if (newlyReadBytesFromStream < unusedBytesInBuffer)
                    {
                        // copy back in our buffer
                        Buffer.BlockCopy(_tempBuffer, 0, _buffer, _bufferLength, newlyReadBytesFromStream);
                        _bufferLength += newlyReadBytesFromStream;
                        var numberOfBytesToSend = Math.Min(count, _bufferLength - _bufferPosition);
                        Buffer.BlockCopy(_buffer, _bufferPosition, targetBuffer, offset, numberOfBytesToSend);
                        _bufferPosition += numberOfBytesToSend;
                        return numberOfBytesToSend;
                    }
                    // not enough space for storing in available space, trim the beginning

                    int additionalBufferSizeRequired = newlyReadBytesFromStream - unusedBytesInBuffer;
                    int remainingBufferLength = _bufferLength - additionalBufferSizeRequired;
                    Buffer.BlockCopy(_buffer, additionalBufferSizeRequired, _buffer, 0, remainingBufferLength);
                    Buffer.BlockCopy(_tempBuffer, 0, _buffer, remainingBufferLength, newlyReadBytesFromStream);
                    _bufferPosition -= _bufferLength - remainingBufferLength;
                    _bufferLength = remainingBufferLength + newlyReadBytesFromStream;

                    // finally copy from our new buffer
                    int bytesSentBack = _bufferLength - _bufferPosition;
                    bytesSentBack = count < bytesSentBack ? count : bytesSentBack;
                    Buffer.BlockCopy(_buffer, _bufferPosition, targetBuffer, offset, bytesSentBack);
                    _bufferPosition += bytesSentBack;
                    return bytesSentBack;
                }
            }
            Buffer.BlockCopy(_buffer, _bufferPosition, targetBuffer, offset, count);
            _bufferPosition += count;
            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin != SeekOrigin.Current)
                throw new NotSupportedException("You cannot seek in this way");
            int requestedPosition = _bufferPosition + (int)offset;

            // TODO: A seek beyond the buffer length could trigger a read
            if (requestedPosition < 0 || requestedPosition > _bufferLength)
                throw new InvalidOperationException("You cannot seek further than the amount of data available in the buffer");
            _bufferPosition = requestedPosition;
            return -1;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("The history stream only works in read-only mode");
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