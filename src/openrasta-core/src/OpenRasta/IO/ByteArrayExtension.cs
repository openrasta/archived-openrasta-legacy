#region License

/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion

namespace OpenRasta.IO
{
    public static class ByteArrayExtension
    {
        public static MatchResult Match(this byte[] source, byte[] marker)
        {
            return Match(source, 0L, marker, 0L, source.LongLength);
        }

        public static MatchResult Match(this byte[] source, long sourceIndex, byte[] marker, long count)
        {
            return Match(source, sourceIndex, marker, 0L, count);
        }

        public static MatchResult Match(this byte[] source, long sourceIndex, byte[] marker, long markerIndex, long count)
        {
            long endOfArray = sourceIndex + count > source.Length ? source.Length : sourceIndex + count;
            for (long sourceCurrentIndex = sourceIndex; sourceCurrentIndex < endOfArray; sourceCurrentIndex++)
            {
                long markerCurrentIndex = markerIndex;
                for (; markerCurrentIndex < marker.Length; markerCurrentIndex++)
                {
                    if (sourceCurrentIndex + markerCurrentIndex >= endOfArray || source[sourceCurrentIndex + markerCurrentIndex] != marker[markerCurrentIndex]) // match
                        break;
                }
                if (markerCurrentIndex == marker.Length)
                    return new MatchResult {State = MatchState.Found, Index = sourceCurrentIndex};
                else if (sourceCurrentIndex + markerCurrentIndex == endOfArray)
                    return new MatchResult {State = MatchState.Truncated, Index = sourceCurrentIndex};
            }
            return new MatchResult {State = MatchState.NotFound, Index = -1};
        }
    }

    public struct MatchResult
    {
        public long Index;
        public MatchState State;
    }

    public enum MatchState
    {
        Found,
        NotFound,
        Truncated
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