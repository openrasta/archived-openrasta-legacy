#region License

/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */

#endregion

using System.Net;
using System.Text;
using NUnit.Framework;
using OpenRasta.Configuration;
using OpenRasta.Configuration.Fluent;
using OpenRasta.Testing;
using OpenRasta.Tests.Integration;
using OpenRasta.Web;

namespace TextPlain_Specification
{
    public class when_using_text_plain : server_context
    {
        public when_using_text_plain()
        {
            ConfigureServer(
                () => ResourceSpace.Has.ResourcesOfType<Customer>().AtUri("/{customerId}").HandledBy<CustomerHandler>()
            );
        }

        [Test]
        public void the_request_is_matched_to_the_parameter()
        {
            given_request_as_string("PATCH", "/3", "new customer name", "UTF-16");
            when_reading_response_as_a_string(Encoding.ASCII);

            TheResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            TheResponse.ContentType.ShouldContain("text/plain");

            TheResponseAsString.ShouldBe("new customer name");
        }

        [Test]
        public void the_response_is_formatted_in_text_plain()
        {
            given_request("GET", "/3");
            when_reading_response_as_a_string(Encoding.ASCII);

            TheResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            TheResponse.ContentType.ShouldContain("text/plain");

            TheResponseAsString.ShouldBe("Customer with id 3");
        }
    }
    public class CustomerHandler
    {
        public OperationResult Get(int customerId)
        {
            return new OperationResult.OK {ResponseResource = "Customer with id " + customerId};
        }

        public OperationResult Patch(int customerId, string customerName)
        {
            return new OperationResult.OK {ResponseResource = customerName};
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