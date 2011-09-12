using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using OpenRasta.OperationModel;
using OpenRasta.Pipeline.Contributors;
using OpenRasta.Testing;
using OpenRasta.Web;
using OpenRasta.Pipeline;
using OperationCreationContributor_Specification;

namespace OpenRasta.Tests.Unit.Web.Pipeline.Contributors
{
    namespace CodecSelector
    {
        public class when_no_operation_is_returned : operation_processors_context<OperationCodecSelectorContributor>
        {
            [Test]
            public void the_result_is_a_415_error()
            {
                given_processor();
                given_operations(0);

                when_executing_processor();

                Result.ShouldBe(PipelineContinuation.RenderNow);
                Context.OperationResult.ShouldBeOfType<OperationResult.RequestMediaTypeUnsupported>();
            }

            protected override OperationCodecSelectorContributor create_processor()
            {
                return new OperationCodecSelectorContributor(Resolver);
            }

            void when_executing_processor()
            {
                when_sending_notification<KnownStages.IOperationFiltering>();
            }
        }
    }

    namespace Filter
    {
    }

    namespace Hydrator
    {
    }

    public abstract class operation_processors_context<TStage> : contributor_context<TStage>
        where TStage : class, IPipelineContributor
    {
        protected TStage Processor { get; set; }

        public void given_processor()
        {
            given_pipeline_contributor(() => create_processor());
        }

        protected abstract TStage create_processor();

        protected void given_operations(int count)
        {
            var mock = new Mock<IOperationCreator>();
            Context.PipelineData.Operations = count >= 0 ? Enumerable.Range(0, count).Select(i => CreateMockOperation()).ToList() : null;
        }

        IOperation CreateMockOperation()
        {
            var operation = new Mock<IOperation>();
            operation.Expect(x => x.ToString()).Returns("Fake method");
            operation.ExpectGet(x => x.Name).Returns("OperationName");
            return operation.Object;
        }
    }
}