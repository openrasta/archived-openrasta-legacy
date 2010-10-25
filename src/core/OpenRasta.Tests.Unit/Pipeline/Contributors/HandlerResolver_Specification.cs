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
using System.Linq;
using NUnit.Framework;
using OpenRasta.Pipeline.Contributors;
using OpenRasta.Testing;
using OpenRasta.Tests;
using OpenRasta.Web;
using OpenRasta.TypeSystem;
using OpenRasta.Pipeline;
using ResourceTypeResolver_Specification;

namespace HandlerResolver_Specification
{
    public class when_no_handler_is_found : openrasta_context
    {
        [Test]
        public void the_resource_handler_is_not_defined_and_the_pipeline_continues()
        {
            given_pipeline_contributor<HandlerResolverContributor>();
            given_pipeline_resourceKey<Fake>();
            when_sending_notification<KnownStages.IUriMatching>();

            Context.PipelineData.SelectedHandlers.ShouldBeNull();
        }
    }

    public class when_a_handler_is_found : openrasta_context
    {
        class FakeHandler
        {
        }

        [Test]
        public void the_handler_types_are_assigned_to_the_correct_collection()
        {
            given_pipeline_contributor<HandlerResolverContributor>();
            given_pipeline_resourceKey<Fake>();
            given_registration_handler<Fake, FakeHandler>();

            when_sending_notification<KnownStages.IUriMatching>()
                .ShouldBe(PipelineContinuation.Continue);

            Context.PipelineData.SelectedHandlers.Count().ShouldBe(1);
            Context.PipelineData.SelectedHandlers.First().Equals<Fake>();
            
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