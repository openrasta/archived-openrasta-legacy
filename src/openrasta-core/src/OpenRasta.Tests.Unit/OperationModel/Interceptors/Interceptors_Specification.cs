using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Moq.Language.Flow;
using NUnit.Framework;
using OpenRasta.OperationModel;
using OpenRasta.OperationModel.Interceptors;
using OpenRasta.Testing;
using OpenRasta.Tests.Unit.OperationModel.MethodBased;

namespace OpenRasta.Tests.Unit.OperationModel.Interceptors
{
    public class when_getting_interceptors_for_an_operation : interceptors_context<HandlerWithInterceptors>
    {
        [Test]
        public void system_interceptors_are_returned()
        {
            var systemInterceptor = new SystemInterceptor();

            given_interceptor_provider(systemInterceptor);
            given_operation("GetALife");

            when_creating_interceptors();

            Interceptors.OfType<SystemInterceptor>().Count().ShouldBe(1);
        }
        [Test]
        public void attribute_interceptor_providers_are_returned()
        {
            given_interceptor_provider();
            given_operation("GetALife");

            when_creating_interceptors();

            Interceptors.OfType<SomeKindOfInterceptorAttribute.InlineInterceptor>().Count().ShouldBe(1);
        }
        [Test]
        public void attribute_interceptors_are_returned()
        {
            given_interceptor_provider();
            given_operation("GetALife");

            when_creating_interceptors();

            Interceptors.OfType<AttributePosingAsAnInterceptor>().Count().ShouldBe(1);
        }
    }
    public class when_wrapping_an_operation_context : interceptors_context<HandlerWithInterceptors>
    {
        [Test]
        public void the_delegated_members_are_the_ones_of_the_wrapped_operation()
        {
            given_operation("GetALife");

            when_creating_wrapper();

            WrappedOperation.ExtendedProperties.ShouldBeTheSameInstanceAs(Operation.ExtendedProperties);
            WrappedOperation.Inputs.ShouldBeTheSameInstanceAs(Operation.Inputs);
            WrappedOperation.Name.ShouldBeTheSameInstanceAs(Operation.Name);
        }

        [Test]
        public void an_interceptor_throwing_an_exception_in_pre_condition_prevents_execution_from_continuing()
        {
            given_mock_operation(op=>op.Expect(x => x.Invoke()).Throws<InvalidOperationException>());
            given_mock_interceptor(i=>i.Expect(x=>x.BeforeExecute(It.IsAny<IOperation>())).Throws<ArgumentException>());
            given_wrapper();

            Executing(invoking_wrapped_operation)
                .ShouldThrow<InterceptorException>()
                .InnerException.ShouldBeOfType<ArgumentException>();

        }

        [Test]
        public void an_interceptor_returning_false_in_pre_condition_prevents_execution_from_continuing()
        {
            given_mock_operation(op => op.Expect(x => x.Invoke()).Throws<InvalidOperationException>());
            given_mock_interceptor(i => i.Expect(x => x.BeforeExecute(It.IsAny<IOperation>())).Returns(false));
            given_wrapper();

            Executing(invoking_wrapped_operation)
                .ShouldThrow<InterceptorException>()
                .InnerException.ShouldBeNull();
        }
        [Test]
        public void an_interceptor_throwing_an_exception_in_post_condition_prevents_execution_from_continuing()
        {

            given_mock_operation(op => op.Expect(x => x.Invoke()).Returns(new OutputMember[0]).Verifiable());
            given_mock_interceptor(before => before.Returns(true),
                                   after => after.Throws<ArgumentException>());
            
            given_wrapper();

            Executing(invoking_wrapped_operation)
                .ShouldThrow<InterceptorException>()
                .InnerException.ShouldBeOfType<ArgumentException>();
            MockOperation.Verify(x => x.Invoke());

        }

        [Test]
        public void an_interceptor_returning_false_in_post_condition_prevents_execution_from_continuing()
        {
            given_mock_operation(op => op.Expect(x => x.Invoke()).Returns(new OutputMember[0]).Verifiable());
            given_mock_interceptor(before => before.Returns(true),
                                   after => after.Returns(false));
            given_wrapper();

            Executing(invoking_wrapped_operation)
                .ShouldThrow<InterceptorException>()
                .InnerException.ShouldBeNull();
            MockOperation.Verify(x=>x.Invoke());
        }
        [Test]
        public void an_interceptor_can_replace_the_original_invoke_call()
        {
            var emptyResult = new OutputMember[0];
            given_mock_operation(op=>op.Expect(x=>x.Invoke()).Throws<InvalidOperationException>());
            given_mock_interceptor(()=>emptyResult);
            given_wrapper();

            invoking_wrapped_operation();

            InvokeResult.ShouldBeTheSameInstanceAs(emptyResult);
        }
    }
    public abstract class interceptors_context<T> : operation_context<T> {
        SystemAndAttributesOperationInterceptorProvider InterceptorProvider;
        protected IEnumerable<IOperationInterceptor> Interceptors;
        protected OperationWithInterceptors WrappedOperation;
        protected Mock<IOperation> MockOperation;
        protected IEnumerable<OutputMember> InvokeResult;

        protected void when_creating_interceptors()
        {
            Interceptors = InterceptorProvider.GetInterceptors(Operation);

        }

        protected void given_interceptor_provider(params IOperationInterceptor[] interceptors)
        {
            InterceptorProvider = new SystemAndAttributesOperationInterceptorProvider(interceptors);
        }

        protected void given_mock_interceptor(Func<IEnumerable<OutputMember>> overriddenMethod)
        {
            given_mock_interceptor(i =>
            {
                i.Expect(x => x.BeforeExecute(It.IsAny<IOperation>())).Returns(true);
                i.Expect(x => x.AfterExecute(It.IsAny<IOperation>(), It.IsAny<IEnumerable<OutputMember>>())).Returns(true);
                i.Expect(x => x.RewriteOperation(It.IsAny<Func<IEnumerable<OutputMember>>>())).Returns(overriddenMethod);
            });
            
        }

        protected void given_mock_interceptor(Action<ISetup<OperationInterceptor, bool>> before, Action<ISetup<OperationInterceptor,bool>> after)
        {
            given_mock_interceptor(i =>
            {
                before(i.Expect(x => x.BeforeExecute(It.IsAny<IOperation>())));
                after(i.Expect(x => x.AfterExecute(It.IsAny<IOperation>(), It.IsAny<IEnumerable<OutputMember>>())));
            });
        }

        protected void given_mock_interceptor(Action<Mock<OperationInterceptor>> interceptorConfig)
        {
            var mock = new Mock<OperationInterceptor>();
            interceptorConfig(mock);
            Interceptors = new[] { mock.Object };
        }

        protected void given_mock_operation(Action<Mock<IOperation>> mockConfig)
        {
            MockOperation = new Mock<IOperation>();
            mockConfig(MockOperation);
            Operation = MockOperation.Object;
        }

        protected void invoking_wrapped_operation()
        {
            InvokeResult = WrappedOperation.Invoke();
        }

        protected void given_wrapper()
        {
            when_creating_wrapper();
        }

        void given_interceptors(params IOperationInterceptor[] interceptors)
        {
            Interceptors = interceptors;
        }

        protected void when_creating_wrapper()
        {
            WrappedOperation = new OperationWithInterceptors(Operation, Interceptors);

        }
    }
    public class HandlerWithInterceptors
    {
        [SomeKindOfInterceptor, AttributePosingAsAnInterceptor]
        public int GetALife()
        {
            return 0;
        }
    }
    public class AttributePosingAsAnInterceptor : InterceptorAttribute
    {
        
    }
    public class SomeKindOfInterceptorAttribute : Attribute, IOperationInterceptorProvider {
        public IEnumerable<IOperationInterceptor> GetInterceptors(IOperation operation)
        {
            yield return new InlineInterceptor();
        }
    public class InlineInterceptor : OperationInterceptor {}
    }


    public class SystemInterceptor : OperationInterceptor {}
}
