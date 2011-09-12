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
using System.Net;
using System.Text;
using NUnit.Framework;
using OpenRasta.Configuration;
using OpenRasta.Testing;
using OpenRasta.Tests.Integration;
using OpenRasta.Web;

namespace HtmlForm_Specification
{
    [Serializable]
    public class when_receiving_an_entity_body : server_context
    {
        public when_receiving_an_entity_body()
        {
            ConfigureServer(() => ResourceSpace.Has.ResourcesOfType<Customer>()
                                      .AtUri("/flat/{id}").Named("Flat")
                                      .AndAt("/constructed/{id}").Named("Constructed")
                                      .HandledBy<CustomerHandler>());
        }

        [Test]
        public void the_key_value_pairs_are_matched_to_string_parameters()
        {
            given_request_as_url_form_encoded("PUT", "/flat/3", "firstname=John&lastname=Doe", "utf-8");

            when_reading_response_as_a_string(Encoding.GetEncoding("utf-8"));
            TheResponseAsString.ShouldBe("Flat3JohnDoe");
        }

        [Test]
        public void the_pairs_are_matched_to_complex_parameters()
        {
            given_request_as_url_form_encoded("PUT", "/constructed/3", "Customer.FirstName=John&Customer.LastName=Doe",
                                        "utf-8");

            when_reading_response_as_a_string(Encoding.GetEncoding("utf-8"));
            TheResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            TheResponseAsString.ShouldBe("Constructed3JohnDoe");
        }
    }

    public class CustomerHandler
    {
        [HttpOperation(HttpMethod.PUT, ForUriName = "Flat")]
        public OperationResult PutFlat(int id, string firstname, string lastname)
        {
            return new OperationResult.OK {
                                              ResponseResource = "Flat" + id + firstname + lastname
                                          };
        }

        [HttpOperation(HttpMethod.PUT, ForUriName = "Constructed")]
        public OperationResult PutConstructed(int id, Customer c)
        {
            return new OperationResult.OK {
                                              ResponseResource = "Constructed" + id + c.FirstName + c.LastName
                                          };
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