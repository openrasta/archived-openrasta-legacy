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
using System.Collections.ObjectModel;
using NUnit.Framework;
using OpenRasta;
using OpenRasta.Testing;

namespace UriTemplateTable_Specification
{
    [TestFixture]
    public class when_matching_a_template_table
    {
        [Test]
        public void out_of_two_templates_with_one_query_parameter_only_the_correct_one_is_used()
        {
            var table = new OpenRasta.UriTemplateTable(new Uri("http://localhost"), new List<KeyValuePair<OpenRasta.UriTemplate, object>>
            {
                new KeyValuePair<OpenRasta.UriTemplate, object>(new OpenRasta.UriTemplate("resource1?query={queryText}"), null),
                new KeyValuePair<OpenRasta.UriTemplate, object>(new OpenRasta.UriTemplate("resource1?query2={queryText}"), null)
            });
            Collection<OpenRasta.UriTemplateMatch> match = table.Match(new Uri("http://localhost/resource1?query=testing a query"));
            match.Count.ShouldBe(1);
            match[0].QueryParameters["queryText"].ShouldBe("testing a query");
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
