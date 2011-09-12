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
using NUnit.Framework;
using OpenRasta.IO;
using OpenRasta.Testing;

namespace ByteArrayExtension_Specification
{
    public class when_matching_byte_sequences : context
    {
        readonly byte[] Base10 = new byte[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};

        [Test]
        public void a_full_match_returns_the_correct_index()
        {
            var match = Base10.Match(new byte[] {3, 4});
            match.State.ShouldBe(MatchState.Found);
            match.Index.ShouldBe(3);
        }

        [Test]
        public void a_match_at_the_beginning_returns_the_correct_index()
        {
            var match = Base10.Match(new byte[] {0, 1, 2, 3, 4});
            match.State.ShouldBe(MatchState.Found);
            match.Index.ShouldBe(0);
        }

        [Test]
        public void a_partial_match_returns_the_beginning_of_the_match()
        {
            var match = Base10.Match(new byte[] {8, 9, 10});
            match.Index.ShouldBe(8);
            match.State.ShouldBe(MatchState.Truncated);
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