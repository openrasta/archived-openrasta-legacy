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
using OpenRasta.Web;

namespace HttpHeaders_Specification
{
    public class whettn_parsing_content_disposition : context
    {
        [Test]
        public void a_parameter_value_without_quotes_results_in_an_error()
        {
            Executing(() => new ContentDispositionHeader("form-data; name=n"))
                .ShouldThrow<FormatException>();
        }

        [Test]
        public void an_empty_header_results_in_an_error()
        {
            Executing(() => new ContentDispositionHeader(""))
                .ShouldThrow<FormatException>();
        }

        [Test, Ignore("need to define the use cases better first")]
        public void lack_of_quotes_in_parameters_can_be_recovered() { }

        [Test]
        public void the_filename_parameter_is_parsed()
        {
            var header = new ContentDispositionHeader("form-data;filename=\"test\"");
            header.FileName.
                ShouldBe("test");
        }

        [Test]
        public void the_first_value_is_the_disposition()
        {
            var header = new ContentDispositionHeader("form-data");
            header.Disposition.
                ShouldBe("form-data");
        }

        [Test]
        public void the_name_parameter_is_parsed()
        {
            var header = new ContentDispositionHeader("form-data;name=\"hi\"");
            header.Name.
                ShouldBe("hi");
        }

        [Test]
        public void the_toString_method_normalize_the_header()
        {
            var header = new ContentDispositionHeader("form-data ; name= \"hi\";");
            header.ToString().
                ShouldBe("form-data; name=\"hi\"");
        }

        [Test]
        public void whitespace_in_parameters_is_ignored()
        {
            var header = new ContentDispositionHeader("form-data ; name = \"hi \";");
            header.Name
                .ShouldBe("hi ");
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