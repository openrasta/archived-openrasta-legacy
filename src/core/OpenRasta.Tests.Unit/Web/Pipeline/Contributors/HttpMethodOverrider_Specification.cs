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
using OpenRasta.Pipeline.Contributors;
using OpenRasta.Testing;
using OpenRasta.Tests;
using OpenRasta.Pipeline;

namespace HttpMethodOverrider_Specification
{
    public class when_the_http_method_is_overridden : openrasta_context
    {
        [Test]
        public void an_original_method_of_get_result_in_an_error()
        {
            Context.Request.HttpMethod = "GET";
            Context.Request.Headers["X-HTTP-Method-Override"] = "PUT";

            given_pipeline_contributor<HttpMethodOverriderContributor>();
            var result = when_sending_notification<KnownStages.IHandlerSelection>();

            result.ShouldBe(PipelineContinuation.Abort);
            Context.ServerErrors[0].ShouldBeOfType<HttpMethodOverriderContributor.MethodIsNotPostError>();
        }

        [Test]
        public void the_http_method_in_the_context_is_updated_for_post()
        {
            Context.Request.HttpMethod = "POST";
            Context.Request.Headers["X-HTTP-Method-Override"] = "PUT";

            given_pipeline_contributor<HttpMethodOverriderContributor>();
            when_sending_notification<KnownStages.IHandlerSelection>();

            Context.Request.HttpMethod.ShouldBe("PUT");
        }
    }

    public class when_there_is_no_override_header : openrasta_context
    {
        [Test]
        public void the_contributor_doesnt_do_anything()
        {
            Context.Request.HttpMethod = "POST";

            given_pipeline_contributor<HttpMethodOverriderContributor>();
            when_sending_notification<KnownStages.IHandlerSelection>();

            Context.Request.HttpMethod.ShouldBe("POST");
            Context.ServerErrors.Count.ShouldBe(0);
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