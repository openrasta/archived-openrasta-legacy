using System;
using System.Net;
using System.ServiceModel.Syndication;
using System.Threading;
using NUnit.Framework;
using OpenRasta.Configuration;
using OpenRasta.DI;
using OpenRasta.Hosting.HttpListener;
using OpenRasta.Pipeline;
using OpenRasta.Testing;

namespace OpenRasta.Tests.Integration.Regressions
{
    public class when_pipeline_contributor_raises_exception_after_operation_executed : server_context
    {
        private static readonly int PORT = 6687;

        public when_pipeline_contributor_raises_exception_after_operation_executed()
        {

            ConfigureServer(() =>
            {
                ResourceSpace.Uses.PipelineContributor<RecursiveContributor>();
                ResourceSpace.Has.ResourcesOfType<string>()
                    .AtUri("/news")
                    .HandledBy<DefaultHandler>()
                    .AsXmlDataContract();
            });
        }
        [Test]
        public void the_pipeline_doesnt_recurse_and_run_quickly()
        {
            the_pipeline_doesnt_recurse();
        }
        public void the_pipeline_doesnt_recurse()
        {
            given_request("GET", "/news");

            when_reading_response();

            this.TheResponse.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);

        }
    }

    public class RecursiveContributor : IPipelineContributor
    {
        public void Initialize(IPipeline pipelineRunner)
        {
            pipelineRunner.Notify(x => { throw new InvalidOperationException(); }).After<KnownStages.IOperationResultInvocation>();
        }
    }

    public class DefaultHandler
    {
        public string Get() {
            return "hello";
        }
    }

}