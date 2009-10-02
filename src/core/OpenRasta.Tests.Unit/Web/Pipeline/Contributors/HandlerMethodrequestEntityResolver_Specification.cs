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
using System.Text;
using OpenRasta.Web.Pipeline.Contributors;
using OpenRasta.Testing;
using ResourceTypeResolver_Specification;
using NUnit.Framework;
using OpenRasta.Web;
using OpenRasta.Web.Pipeline;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Specialized;
using OpenRasta.Codecs;
using OpenRasta.Tests;
using OpenRasta.Tests.Unit.Fakes;

namespace HandlerMethodRequestEntityResolver_Specification
{
    public class when_the_codec_assigns_all_parameters_successfully : openrasta_context
    {
        [Test]
        public void the_method_invocation_is_ready_to_be_invoked()
        {
            GivenAContributor<HandlerMethodRequestEntityResolver>();
            GivenAFinalMethodInvocation<Customer>(c => { });
            GivenTheRequestCodec<CustomerCodec,Strictly<Customer>>("application/vnd.rasta");
            GivenARequestContentTypeOf("application/vnd.rasta");

            Context.PipelineData.SelectedMethod.IsReadyForInvocation.ShouldBeFalse();

            WhenSendingNotificationFor<RequestEntityCodecResolver>()
                .ShouldBe(PipelineContinuation.Continue);

            Context.PipelineData.SelectedMethod.IsReadyForInvocation.ShouldBeTrue();
            Context.PipelineData.SelectedMethod.GetParameterByName("c")
                .ShouldNotBeNull()
                .Value
                    .ShouldBeOfType<Customer>();
        }
    }
    public class when_the_codec_fails_to_read_a_parameter_successfully : openrasta_context
    {
        [Test]
        public void an__error_is_added_and_the_pipeline_aborts()
        {
            
        }
    }
    public class when_there_is_no_codec : openrasta_context
    {
        [Test]
        public void the_contributor_is_ignored()
        {
            GivenAContributor<HandlerMethodRequestEntityResolver>();
            GivenAFinalMethodInvocation<Customer>(c => { });
            GivenARequestContentTypeOf("application/vnd.rasta");

            WhenSendingNotificationFor<RequestEntityCodecResolver>()
                .ShouldBe(PipelineContinuation.Continue);
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
