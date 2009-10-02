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
using OpenRasta.Tests;
using OpenRasta.Tests.Unit.Fakes;
using OpenRasta.Web;
using OpenRasta.Pipeline;

namespace HandlerMethodsInvoker_Specification
{
    //public class when_the_method_returns_an_opration_context : openrasta_context
    //{
    //    [Test]
    //    public void the_operation_context_is_assigned_and_executed()
    //    {
    //        var handler = new ResourceHandler();
    //        given_pipeline_contributor<HandlerMethodInvoker>();
    //        GivenAHandler(handler);
    //        GivenAFinalMethodInvocation<OperationResult>(handler.GetOp);

    //        when_sending_notification<RequestEntityReader>()
    //            .ShouldBe(PipelineContinuation.Continue);

    //        Context.OperationResult.ShouldBeOfType<OperationResult.OK>();

    //        Context.Response.Entity.Instance.ShouldBeOfType<Resource>();
    //    }
    //}

    //public class when_the_method_returns_another_object : openrasta_context
    //{
    //    [Test]
    //    public void the_result_is_wraped_in_a_no_content_when_returns_null()
    //    {
    //        var handler = new ResourceHandler();
    //        given_pipeline_contributor<HandlerMethodInvoker>();
    //        GivenAHandler(handler);
    //        GivenAFinalMethodInvocation<object>(handler.GetNull);

    //        when_sending_notification<RequestEntityReader>()
    //            .ShouldBe(PipelineContinuation.Continue);

    //        Context.OperationResult.ShouldBeOfType<OperationResult.NoContent>();

    //        Context.Response.Entity.Instance.ShouldBeNull();
    //    }

    //    [Test]
    //    public void the_result_is_wraped_in_an_OK_when_there_is_an_entity()
    //    {
    //        var handler = new ResourceHandler();
    //        given_pipeline_contributor<HandlerMethodInvoker>();
    //        GivenAHandler(handler);
    //        GivenAFinalMethodInvocation<object>(handler.GetObj);

    //        when_sending_notification<RequestEntityReader>()
    //            .ShouldBe(PipelineContinuation.Continue);

    //        Context.OperationResult.ShouldBeOfType<OperationResult.OK>();

    //        Context.Response.Entity.Instance.ShouldBeOfType<Resource>();
    //    }
    //}

    //public class when_the_method_returns_void : openrasta_context
    //{
    //    [Test]
    //    public void the_result_is_wraped_in_an_OK_when_there_is_an_entity()
    //    {
    //        var handler = new ResourceHandler();
    //        given_pipeline_contributor<HandlerMethodInvoker>();
    //        GivenAHandler(handler);
    //        GivenAFinalMethodInvocation(handler.GetVoid);

    //        when_sending_notification<RequestEntityReader>()
    //            .ShouldBe(PipelineContinuation.Continue);

    //        Context.OperationResult.ShouldBeOfType<OperationResult.NoContent>();

    //        Context.Response.Entity.Instance.ShouldBeNull();
    //    }
    //}

    //public class when_the_method_doesnt_have_all_its_parameters_fullfilled : openrasta_context
    //{
    //    [Test]
    //    public void a_bad_request_error_is_returned()
    //    {
    //        var handler = new ResourceHandler();
    //        given_pipeline_contributor<HandlerMethodInvoker>();
    //        GivenAHandler(handler);
    //        GivenAFinalMethodInvocation<Customer, OperationResult>(handler.Post);

    //        when_sending_notification<RequestEntityReader>()
    //            .ShouldBe(PipelineContinuation.RenderNow);

    //        Context.OperationResult.ShouldBeOfType<OperationResult.BadRequest>();
    //    }
    //}

    //public class when_the_method_fails : openrasta_context
    //{
    //    [Test]
    //    public void an_error_is_logged_and_the_pipeline_aborts()
    //    {
    //        var handler = new ResourceHandler();
    //        given_pipeline_contributor<HandlerMethodInvoker>();
    //        GivenAHandler(handler);
    //        GivenAFinalMethodInvocation(handler.GetError);

    //        when_sending_notification<RequestEntityReader>()
    //            .ShouldBe(PipelineContinuation.Abort);

    //        Context.ServerErrors[0]
    //            .ShouldBeOfType<ErrorFrom<HandlerMethodInvoker>>()
    //            .Message
    //            .ShouldContain("InvalidOperationException");
    //    }
    //}

    public class Resource
    {
    }

    public class ResourceHandler
    {
        public OperationResult GetOp() { return new OperationResult.OK(new Resource()); }
        public OperationResult Post(Customer customer) { return null; }
        public void GetVoid() { }
        public object GetObj() { return new Resource(); }
        public object GetNull() { return null; }
        public void GetError() { throw new InvalidOperationException(); }
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