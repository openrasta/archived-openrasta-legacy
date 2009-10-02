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
using OpenRasta.Testing;
using OpenRasta.Text;

namespace Rfc2047Encoding_Specification
{
    public class when_decoding_strings_in_text_tokens : context
    {
        const string US_ASCII = "=?US-ASCII?Q?Keith_Moore?= <moore@cs.utk.edu>";
        const string KLINGON = "=?tlh?Q?Klingon Text";
        const string UNKNOWN_ENCODING = "=?tlh?W?Klingon Text";
        const string ISO = "=?ISO-8859-1?Q?Keld_J=F8rn_Simonsen?=";
        const string ISO_SUBJECT =
            "=?ISO-8859-1?B?SWYgeW91IGNhbiByZWFkIHRoaXMgeW8=?==?ISO-8859-2?B?dSB1bmRlcnN0YW5kIHRoZSBleGFtcGxlLg==?=";

        [Test]
        public void encoded_characters_are_decoded() { Rfc2047Encoding.DecodeTextToken(ISO).ShouldBe("Keld Jørn Simonsen"); }

        [Test]
        public void mutliple_encodings_are_supported() { Rfc2047Encoding.DecodeTextToken(ISO_SUBJECT).ShouldBe("If you can read this you understand the example."); }

        [Test]
        public void the_decoding_is_done_including_spaces()
        {
            Rfc2047Encoding.DecodeTextToken(US_ASCII)
                .ShouldBe("Keith Moore <moore@cs.utk.edu>");
        }

        [Test]
        public void the_text_is_not_decoded_if_the_charset_is_unknown() { Rfc2047Encoding.DecodeTextToken(KLINGON).ShouldBe(KLINGON); }

        [Test]
        public void the_text_is_not_decoded_if_the_encoding_is_unknown() { Rfc2047Encoding.DecodeTextToken(UNKNOWN_ENCODING).ShouldBe(UNKNOWN_ENCODING); }
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