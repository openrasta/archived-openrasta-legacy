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
using System.Text;
using OpenRasta.Diagnostics;
using OpenRasta.IO.Diagnostics;

namespace OpenRasta.IO
{
    public class BoundaryStreamReader
    {
        readonly byte[] _beginBoundary;
        readonly string _beginBoundaryAsString;
        readonly byte[] _localBuffer;
        readonly byte[] _newLine = new byte[] { 13, 10 };
        int _localBufferLength;
        BoundarySubStream _previousStream;

        public BoundaryStreamReader(string boundary, Stream baseStream)
            : this(boundary, baseStream, Encoding.ASCII)
        {
        }

        public BoundaryStreamReader(string boundary, Stream baseStream, Encoding streamEncoding)
            : this(boundary, baseStream, streamEncoding, 4096)
        {
        }

        public BoundaryStreamReader(string boundary, Stream baseStream, Encoding streamEncoding, int bufferLength)
        {
            if (baseStream == null)
                throw new ArgumentNullException("baseStream");
            if (!baseStream.CanSeek || !baseStream.CanRead)
                throw new ArgumentException("baseStream must be a seekable readable stream.");
            if (bufferLength < boundary.Length + 6)
                throw new ArgumentOutOfRangeException("bufferLength", "The buffer needs to be big enough to contain the boundary and control characters (6 bytes)");

            Log = NullLogger<IOLogSource>.Instance;
            BaseStream = baseStream;

            // by default if unspecified an encoding should be ascii
            // some people are of the opinion that utf-8 should be parsed by default
            // or that it should depend on the source page.
            // Need to test what browsers do in the wild.
            Encoding = streamEncoding;
            Encoding.GetBytes("--" + boundary);
            _beginBoundary = Encoding.GetBytes("\r\n--" + boundary);
            _localBuffer = new byte[bufferLength];
            _beginBoundaryAsString = "--" + boundary;
            AtPreamble = true;
        }

        public bool AtBoundary { get; private set; }
        public bool AtEndBoundary { get; private set; }
        public bool AtPreamble { get; private set; }
        public Stream BaseStream { get; private set; }
        public Encoding Encoding { get; private set; }
        public ILogger Log { get; set; }

        public Stream GetNextPart()
        {
            if (AtEndBoundary)
                return null;
            SkipPreamble();
            if (AtEndBoundary)
                return null;
            TerminateExistingStream();
            return _previousStream = new BoundarySubStream(this);
        }

        /// <summary>
        /// Used only to parse boundaries and headers. ASCII always.
        /// </summary>
        /// <returns></returns>
        public string ReadLine()
        {
            bool temp;
            return ReadLine(out temp);
        }

        public string ReadLine(out bool crlfFound)
        {
            TerminateExistingStream();
            if (AtEndBoundary)
            {
                crlfFound = true;
                return null;
            }

            var toConvert = ReadUntil(_newLine, true, out crlfFound);
            if (toConvert == null)
            {
                AtEndBoundary = true; // reached the end of the steam
                return null;
            }

            string convertedLine = toConvert.Length == 0 ? string.Empty : Encoding.GetString(toConvert).TrimEnd();
            if (string.Compare(convertedLine, _beginBoundaryAsString, StringComparison.OrdinalIgnoreCase) == 0)
            {
                AtPreamble = false;
                AtBoundary = true;
                return convertedLine;
            }

            if (string.Compare(convertedLine, _beginBoundaryAsString + "--", StringComparison.OrdinalIgnoreCase) == 0)
            {
                AtPreamble = false;
                AtBoundary = true;
                AtEndBoundary = true;
                return convertedLine;
            }

            return convertedLine;
        }

        public byte[] ReadNextPart()
        {
            if (AtEndBoundary)
                return new byte[0];
            var part = new MemoryStream();
            long count = ReadNextPart(part, true);
            var result = new byte[count];
            Buffer.BlockCopy(part.GetBuffer(), 0, result, 0, (int)count);
            return result;
        }

        public long ReadNextPart(Stream destinationStream, bool continueToNextBoundaryOnEmptyRead)
        {
            if (AtEndBoundary)
                return 0;
            TerminateExistingStream();
            if (AtEndBoundary)
                return 0;
            long bytesRead = 0;
            long lastRead;
            if (TryReadPreamble(destinationStream, ref bytesRead))
                return bytesRead;

            bool markerFound;
            while ((lastRead = ReadUntil(destinationStream, _beginBoundary, false, out markerFound)) >= 0)
            {
                bytesRead += lastRead;
                if (markerFound)
                {
                    BaseStream.Read(new byte[2], 0, 2);

                    string line = ReadLine();

                    if (AtBoundary || AtEndBoundary)
                    {
                        if (bytesRead == 0 && continueToNextBoundaryOnEmptyRead) // no data between boundaries
                            continue;
                        break;
                    }

                    destinationStream.Write(_newLine, 0, 2);
                    var encodedLine = Encoding.GetBytes(line);
                    destinationStream.Write(encodedLine, 0, encodedLine.Length);
                    destinationStream.Write(_newLine, 0, 2);
                    bytesRead += encodedLine.Length + 4;
                }
                else
                {
                    break;
                }
            }

            return bytesRead;
        }

        public long SeekToNextPart()
        {
            Log.WriteDebug("Seeking to next available part");
            if (AtEndBoundary)
                return 0;
            if (AtPreamble)
            {
                SkipPreamble();
                return 0;
            }

            return ReadNextPart(Stream.Null, false);
        }

        byte[] ReadUntil(byte[] marker, bool swallowMarker, out bool markerFound)
        {
            var dataToSendBack = new MemoryStream();

            long count = ReadUntil(dataToSendBack, marker, swallowMarker, out markerFound);
            if (count == 0 && markerFound)
                return new byte[0];
            if (count == 0 && !markerFound)
                return null;
            return dataToSendBack.ToArray();
        }

        int ReadUntil(byte[] buffer, int offset, int count, byte[] marker, out MatchState lastMatch)
        {
            lastMatch = MatchState.NotFound;
            int maxReadLength = count > _localBuffer.Length - _localBufferLength ? _localBuffer.Length - _localBufferLength : count;

            int totalRead = 0;
            int lastReadCount = BaseStream.Read(_localBuffer, _localBufferLength, maxReadLength);
            if (lastReadCount > 0)
            {
                var searchResult = _localBuffer.Match(0L, marker, 0, lastReadCount + _localBufferLength);
                lastMatch = searchResult.State;
                if (searchResult.State == MatchState.Found)
                {
                    if (searchResult.Index > 0)
                        Buffer.BlockCopy(_localBuffer, 0, buffer, offset, (int)searchResult.Index);

                    long leftOver = (_localBufferLength + lastReadCount) - searchResult.Index;
                    BaseStream.Seek(leftOver * -1, SeekOrigin.Current);
                    _localBufferLength = 0;
                    totalRead = (int)searchResult.Index;
                }
                else if (searchResult.State == MatchState.NotFound)
                {
                    totalRead = lastReadCount + _localBufferLength;
                    Buffer.BlockCopy(_localBuffer, 0, buffer, offset, totalRead);

                    _localBufferLength = 0;
                }
                else if (searchResult.State == MatchState.Truncated)
                {
                    totalRead = (int)searchResult.Index;
                    Buffer.BlockCopy(_localBuffer, 0, buffer, offset, totalRead);

                    int datalength = lastReadCount + _localBufferLength;
                    int leftover = datalength - (int)searchResult.Index;
                    Buffer.BlockCopy(_localBuffer, (int)searchResult.Index, _localBuffer, 0, leftover);
                    _localBufferLength = leftover;
                }
            }
            else
            {
                Buffer.BlockCopy(_localBuffer, 0, buffer, offset, _localBufferLength);
                totalRead = _localBufferLength;
                _localBufferLength = 0;
            }

            return totalRead;
        }

        long ReadUntil(Stream destinationStream, byte[] marker, bool swallowMarker, out bool markerFound)
        {
            var buffer = new byte[4096];

            markerFound = false;
            AtBoundary = false;
            AtEndBoundary = false;
            int lastReadCount;
            long totalCount = 0;
            MatchState lastState;

            while ((lastReadCount = ReadUntil(buffer, 0, buffer.Length, marker, out lastState)) > 0)
            {
                destinationStream.Write(buffer, 0, lastReadCount);
                totalCount += lastReadCount;
            }

            if (lastState == MatchState.Found)
            {
                markerFound = true;
                if (swallowMarker)
                    BaseStream.Seek(marker.Length, SeekOrigin.Current);
            }

            return totalCount;
        }

        void SkipPreamble()
        {
            Log.WriteDebug("Skip the preamble. AtPreamble was {0}.", AtPreamble);
            long preambleSize = 0;
            bool preambleRead = false;
            if (AtPreamble)
                preambleRead = TryReadPreamble(Stream.Null, ref preambleSize);
            Log.WriteDebug("Preamble found: {0} of size {1}", preambleRead, preambleSize);
        }

        void TerminateExistingStream()
        {
            Log.WriteDebug("TerminateExistingStream(), previous stream was " + _previousStream == null ? "null" : "not null");
            if (_previousStream != null)
            {
                _previousStream.AtEnd = true;
                _previousStream = null;
                SeekToNextPart();
            }
        }

        bool TryReadPreamble(Stream destinationStream, ref long bytesRead)
        {
            bool wasAtPreamble = AtPreamble;

            bool preambleRead = false;
            bool lastPreambleCrLfPending = false;
            while (AtPreamble)
            {
                bool crlfFound;
                string currentLine = ReadLine(out crlfFound);
                if (currentLine == null)
                    break;
                lastPreambleCrLfPending = crlfFound;
                if (!AtBoundary)
                {
                    if (currentLine != string.Empty)
                    {
                        if (preambleRead)
                        {
                            destinationStream.Write(_newLine);
                            bytesRead += 2;
                        }

                        var encodedLine = Encoding.GetBytes(currentLine);
                        destinationStream.Write(encodedLine);
                        bytesRead += encodedLine.Length;
                    }

                    preambleRead = true;
                }
                else
                    lastPreambleCrLfPending = false;
            }

            if (wasAtPreamble && lastPreambleCrLfPending && bytesRead > 0)
            {
                destinationStream.Write(_newLine);
                bytesRead += 2;
            }

            return wasAtPreamble;
        }

        class BoundarySubStream : Stream
        {
            readonly BoundaryStreamReader _reader;

            public BoundarySubStream(BoundaryStreamReader reader)
            {
                _reader = reader;
            }

            public bool AtEnd { get; set; }

            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanSeek
            {
                get { return true; }
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            /// <exception cref="NotSupportedException"><c>NotSupportedException</c>.</exception>
            public override long Length
            {
                get { throw new NotSupportedException(); }
            }

            /// <exception cref="NotSupportedException"><c>NotSupportedException</c>.</exception>
            public override long Position
            {
                get { throw new NotSupportedException(); }
                set { throw new NotSupportedException(); }
            }

            public override void Flush()
            {
                throw new NotImplementedException();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (AtEnd)
                    return 0;
                MatchState state;
                int resultCount = _reader.ReadUntil(buffer, offset, count, _reader._beginBoundary, out state);
                if (state == MatchState.Found) // reached a boundary
                    AtEnd = true;

                return resultCount;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
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