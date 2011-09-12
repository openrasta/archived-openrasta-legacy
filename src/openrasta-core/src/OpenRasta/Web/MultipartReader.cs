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
using OpenRasta.Diagnostics;
using OpenRasta.IO;
using OpenRasta.IO.Diagnostics;

namespace OpenRasta.Web
{
    public class MultipartReader
    {
        readonly BoundaryStreamReader _reader;
        string _currentLine;

        public MultipartReader(string boundary, Stream inputStream)
        {
            _reader = new BoundaryStreamReader(boundary, inputStream, Encoding.ASCII);
        }

        public bool AtBeginBoundary
        {
            get { return _reader.AtBoundary; }
        }

        public bool AtBoundary
        {
            get { return AtBeginBoundary || AtEndBoundary; }
        }

        public bool AtEndBoundary
        {
            get { return _reader.AtEndBoundary; }
        }

        IMultipartHttpEntity CurrentEntity { get; set; }

        ILogger _log;
        public ILogger Log
        {
            get { return _log; }
            set { _log = _reader.Log = value; }
        }

        public IEnumerable<IMultipartHttpEntity> GetParts()
        {
            if (AtEndBoundary)
                throw new InvalidOperationException("Can only read through the enumerator once.");
            _reader.SeekToNextPart(); // seeks to the first part

            if (AtEndBoundary)
                yield break;

            while (ReadEntity())
            {
                yield return CurrentEntity;
            }
            yield break;
        }

        public void GoToNextBoundary()
        {
            _reader.SeekToNextPart();
        }

        public bool ReadNextLine()
        {
            _currentLine = _reader.ReadLine();
            return _currentLine != null;
        }

        bool ReadEntity()
        {
            if (AtEndBoundary)
                return false;
            var entity = new MultipartHttpEntity();

// TODO: Handle split headers
            while (ReadNextLine() && !string.IsNullOrEmpty(_currentLine) && !AtBoundary && !AtEndBoundary)
            {
                int columnIndex = _currentLine.IndexOf(":");
                if (columnIndex != -1)
                    entity.Headers[_currentLine.Substring(0, columnIndex).Trim()] =
                        _currentLine.Substring(columnIndex + 1).Trim();
            }
            if (_currentLine == null)
                return false;
            if (_currentLine.Length == 0)
                entity.Stream = _reader.GetNextPart();
            CurrentEntity = entity;
            return true;
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