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
using System.Linq;
using System.Net;
using NUnit.Framework;
using OpenRasta.Configuration;
using OpenRasta.Configuration.Fluent;
using OpenRasta.Testing;
using OpenRasta.Tests.Integration;
using OpenRasta.Web;

namespace MultipartFormData_Specification
{
    // integration tests trying the whole system.
    public class when_the_request_contains_a_multipart_form_data_entity : server_context
    {
        const string TWO_FIELDS = @"
--boundary42
Content-Disposition: form-data; name=""username""

johndoe
--boundary42
Content-Disposition: form-data; name=""dateofbirth""

12/10/2001
--boundary42--
";

        public when_the_request_contains_a_multipart_form_data_entity()
        {
            ConfigureServer(() => ResourceSpace.Has.ResourcesOfType<Customer>()
                                      .AtUri("/")
                                      .And
                                      .AtUri("/multipart").Named("PostMultipart")
                                      .HandledBy<CustomerHandler>());
        }
        [Test]
        public void the_multipart_entity_is_received()
        {
            given_request_as_multipart("POST","/multipart", "boundary42",TWO_FIELDS);
            when_reading_response();

            TheResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        }

        void given_request_as_multipart(string httpMethod, string uri, string boundary, string multipartContent)
        {
            given_request_as_string(httpMethod,uri,multipartContent,"utf-8","multipart/form-data;boundary=" +boundary);

        }
    }

    public class CustomerHandler
    {
        public object Get() { return "hi"; }
        public virtual OperationResult Put(string firstname, string lastname) { return null; }
        [HttpOperation(ForUriName="PostMultipart")]
        public OperationResult PostTwoMultiparts(IEnumerable<IMultipartHttpEntity> multiparts)
        {
            // force a read
            multiparts = multiparts.ToList();
            if (multiparts.Count() != 2)
                throw new InvalidOperationException();
            var first = multiparts.First();
            var second = multiparts.Skip(1).First();
            if (first.Headers.ContentDisposition.Name == "username"
                && second.Headers.ContentDisposition.Name == "dateofbirth")
                return new OperationResult.OK();
            throw new InvalidOperationException();
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