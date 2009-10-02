#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion

using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using NUnit.Framework;
using OpenRasta.Codecs;
using OpenRasta.Configuration;
using OpenRasta.Testing;
using OpenRasta.Tests.Integration;
using OpenRasta.Web;

namespace Json_Specification
{
    public class when_sending_json_content : server_context
    {
        public when_sending_json_content()
        {
            ConfigureServer(() => ResourceSpace.Has.ResourcesOfType<Customer>()
                                      .AtUri("/customer/{id}")
                                      .HandledBy<CustomerHandler>()
                                      .AndTranscodedBy<JsonDataContractCodec>(null));
        }

        [Test]
        public void the_content_is_parsed_as_the_correct_object_type()
        {
            string customerJson = @"
{
    ""FirstName"": ""John"",
    ""LastName"": ""Doe""
}";
            given_request_as_string("PUT", "/customer/3", customerJson, "utf-8", "application/json");

            when_reading_response_as_a_string(Encoding.UTF8);

            TheResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            var serializer = new DataContractJsonSerializer(typeof(Customer));
            var customer =
                serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(TheResponseAsString))) as Customer;

            customer.FirstName.ShouldBe("John");
            customer.LastName.ShouldBe("Doe");
        }

        public class CustomerHandler
        {
            public OperationResult Put(int id, Customer customer)
            {
                return new OperationResult.Modified { ResponseResource = customer };
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