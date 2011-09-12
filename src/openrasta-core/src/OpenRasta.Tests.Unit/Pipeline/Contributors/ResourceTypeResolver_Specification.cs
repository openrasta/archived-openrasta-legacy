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
using OpenRasta.Hosting.InMemory;
using OpenRasta.Pipeline.Contributors;
using OpenRasta.Testing;
using OpenRasta.Tests;
using OpenRasta.Web;
using OpenRasta.Pipeline;

namespace ResourceTypeResolver_Specification
{
    public class when_there_is_already_a_match : openrasta_context
    {
        [Test]
        public void the_match_is_not_modified()
        {
            given_pipeline_contributor<ResourceTypeResolverContributor>();

            given_request_uri("http://localhost/fake");

            var theOriginalMatch = Context.PipelineData.SelectedResource = new UriRegistration("/", "int");

            when_sending_notification<BootstrapperContributor>()
                .ShouldBe(PipelineContinuation.Continue);

            Context.PipelineData.SelectedResource.ShouldBeTheSameInstanceAs(theOriginalMatch);
        }
    }

    public class when_there_is_no_existing_match : openrasta_context
    {
        [Test]
        public void a_failing_match_results_in_an_operation_not_found()
        {
            given_pipeline_contributor<ResourceTypeResolverContributor>();

            given_request_uri("http://localhost/fake");
            given_registration_urimapping<Fake>("/somewhere");

            when_sending_notification<BootstrapperContributor>()
                .ShouldBe(PipelineContinuation.RenderNow);

            Context.PipelineData.SelectedResource.ShouldBeNull();
            Context.OperationResult.ShouldBeOfType<OperationResult.NotFound>();
        }

        [Test]
        public void a_new_matching_is_done_and_the_resource_type_is_assigned()
        {
            given_pipeline_contributor<ResourceTypeResolverContributor>();

            given_request_uri("http://localhost/fake");
            given_registration_urimapping<Fake>("/fake");

            when_sending_notification<BootstrapperContributor>();

            Context.PipelineData.SelectedResource.ResourceKey.ShouldBe(typeof (Fake).AssemblyQualifiedName);

        }[Test]
        public void the_match_is_relative_to_the_app_base_uri()
        {
            given_app_base_uri("http://localhost/root");
            given_pipeline_contributor<ResourceTypeResolverContributor>();
            given_request_uri("http://localhost/root/fake");
            given_registration_urimapping<Fake>("/fake");

            when_sending_notification<BootstrapperContributor>();

            Context.PipelineData.SelectedResource
                .ShouldNotBeNull()
                .ResourceKey.ShouldBe(typeof(Fake).AssemblyQualifiedName);
        }

        void given_app_base_uri(string appBaseUri)
        {
            ((InMemoryCommunicationContext)base.Context).ApplicationBaseUri = new Uri(appBaseUri);

        }
    }

    public class Fake
    {
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