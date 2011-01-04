using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moq;
using NUnit.Framework;
using OpenRasta.Binding;
using OpenRasta.OperationModel;
using OpenRasta.OperationModel.MethodBased;
using OpenRasta.Testing;
using OpenRasta.TypeSystem;

namespace OpenRasta.Tests.Unit.OperationModel.MethodBased
{
    public class when_there_is_a_method_filter: method_based_operation_creator_context
    {
        public Mock<IMethodFilter> MockFilter { get; set; }

        [Test]
        public void a_filter_is_called_that_filters_all_operations()
        {
            given_operation_creator(filter_selecting_first_method());
            given_handler<MockHandler>();

            when_creating_operations();

            then_operation_count_should_be(1);
            then_filter_method_was_called();
        }

        void then_filter_method_was_called()
        {
            MockFilter.VerifyAll();
        }

        IMethodFilter[] filter_selecting_first_method()
        {
            MockFilter = new Mock<IMethodFilter>();
            MockFilter.Expect(x => x.Filter(It.IsAny<IEnumerable<IMethod>>())).Returns(mock_filter()).Verifiable();
            return new[] { MockFilter.Object };
        }
        Func<IEnumerable<IMethod>,IEnumerable<IMethod>> mock_filter()
        {
            return methods => new[] { methods.First() };
        }

    }
    public class when_there_is_no_method_filter : method_based_operation_creator_context
    {
        [Test]
        public void by_default_operations_are_created_for_all_public_instance_and_static_methods()
        {
            given_operation_creator(null);
            given_handler<MockHandler>();

            when_creating_operations();

            then_operation_count_should_be_same_as_public_methods_on_handler(typeof(MockHandler));
        }
        void then_operation_count_should_be_same_as_public_methods_on_handler(Type handlerType)
        {
            Operations.Count().ShouldBe(handlerType.GetMethods(BindingFlags.Instance |
                                                               BindingFlags.Static |
                                                               BindingFlags.Public |
                                                               BindingFlags.FlattenHierarchy).Length);
        }
    }

    public class method_based_operation_creator_context : operation_creator_context<MethodBasedOperationCreator>
    {
        protected IList<IType> Handlers { get; set; }
        protected IEnumerable<IOperation> Operations { get; set; }

        protected void then_operation_count_should_be(int count)
        {
            Operations.Count().ShouldBe(count);
        }

        public void given_operation_creator(IMethodFilter[] filters)
        {
            OperationCreator = new MethodBasedOperationCreator(filters, Resolver, new DefaultObjectBinderLocator());
        }

        protected void given_handler<T>()
        {
            Handlers = Handlers ?? new List<IType>();
            Handlers.Add(TypeSystem.FromClr<T>());
        }

        protected void when_creating_operations()
        {
            Operations = OperationCreator.CreateOperations(Handlers);
        }
    }

    public abstract class operation_creator_context<T> : openrasta_context
        where T : IOperationCreator
    {
        protected T OperationCreator { get; set; }
    }

    public class MockHandler
    {
        public static void Delete()
        {
        }

        public void Get()
        {
        }

        public void Post()
        {
        }
        public void Put()
        {
        }
    }
}