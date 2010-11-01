using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using Moq;
using NUnit.Framework;
using OpenRasta.Codecs;
using OpenRasta.Collections;
using OpenRasta.DI;
using OpenRasta.OperationModel;
using OpenRasta.Pipeline.Contributors;
using OpenRasta.Testing;
using OpenRasta.Tests;
using OpenRasta.TypeSystem;
using OpenRasta.Web;
using OpenRasta.Pipeline;
using OpenRasta.Tests.Unit.Fakes;

namespace OperationCreationContributor_Specification
{
    public class when_in_the_pipeline : operation_creation_context
    {
        [Test]
        public void it_executes_after_handler_selection()
        {
            given_operation_creator_returns_null();
            given_contributor();

            when_sending_notification<KnownStages.IHandlerSelection>();

            then_contributor_is_executed();
        }
    }

    public class when_there_is_no_handler : operation_creation_context
    {
        [Test]
        public void operations_are_not_created_and_the_processing_continues()
        {
            given_operation_creator_returns(1);
            given_contributor();
            when_sending_notification();
            then_contributor_returns(PipelineContinuation.Continue);
            Context.PipelineData.Operations.ShouldBeNull();
        }
    }

    public class when_no_operation_is_created : operation_creation_context
    {
        [Test]
        public void the_operation_result_is_set_to_method_not_allowed()
        {
            given_pipeline_selectedHandler<CustomerHandler>();
            given_operation_creator_returns(0);
            given_contributor();

            when_sending_notification();

            then_contributor_returns(PipelineContinuation.RenderNow);
            Context.OperationResult.ShouldBeOfType<OperationResult.MethodNotAllowed>();
        }
    }

    public class when_operations_are_created : operation_creation_context
    {
        [Test]
        public void an_operation_is_set_and_the_processing_continues()
        {
            given_pipeline_selectedHandler<CustomerHandler>();
            given_operation_creator_returns(1);
            given_contributor();

            when_sending_notification();

            then_contributor_returns(PipelineContinuation.Continue);
            Context.PipelineData.Operations.ShouldHaveCountOf(1)
                .ShouldHaveSameElementsAs(Operations);
        }
    }

    public abstract class contributor_context<T> : openrasta_context
        where T : class, IPipelineContributor
    {
        public void given_contributor()
        {
            given_pipeline_contributor<T>();
        }

        protected void then_contributor_returns(PipelineContinuation continuation)
        {
            Result.ShouldBe(continuation);
        }
    }

    public abstract class operation_creation_context : contributor_context<OperationCreatorContributor>
    {
        public List<IOperation> Operations { get; set; }


        protected void given_operation_creator_returns_null()
        {
            given_operation_creator_returns(-1);
        }

        protected void given_operation_creator_returns(int count)
        {
            var mock = new Mock<IOperationCreator>();
            Operations = count >= 0 ? Enumerable.Range(0, count).Select<int, IOperation>(i => CreateMockOperation()).ToList() : null;
            mock.Expect(x => x.CreateOperations(It.IsAny<IEnumerable<IType>>()))
                .Returns(Operations);
            Resolver.AddDependencyInstance(typeof(IOperationCreator), mock.Object, DependencyLifetime.Singleton);

        }

        IOperation CreateMockOperation()
        {
            var operation = new Mock<IOperation>();
            operation.Expect(x => x.ToString()).Returns("Fake method");
            operation.ExpectGet(x => x.Name).Returns("OperationName");
            return operation.Object;
        }

        protected void when_sending_notification()
        {
            when_sending_notification<KnownStages.IHandlerSelection>();
        }
    }
}