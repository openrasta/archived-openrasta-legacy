using System.Collections.Generic;
using System.Linq;
using OpenRasta.Binding;
using OpenRasta.OperationModel;
using OpenRasta.OperationModel.MethodBased;
using OpenRasta.Testing;
using OpenRasta.Tests.Unit.OperationModel.MethodBased;
using OpenRasta.TypeSystem;
using OpenRasta.Web;

namespace OpenRasta.Tests.Unit.OperationModel.Filters
{
    public abstract class operation_filter_context<THandler, TFilter> : operation_context<THandler>
        where TFilter : IOperationProcessor
    {
        protected TFilter Filter { get; set; }
        protected IEnumerable<IOperation> FilteredOperations { get; set; }
        protected IEnumerable<IOperation> Operations { get; set; }

        public void given_filter()
        {
            Filter = create_filter();
        }

        protected abstract TFilter create_filter();

        protected void given_operations()
        {
            Operations = new MethodBasedOperationCreator(
                new[] { new TypeExclusionMethodFilter<object>() },
                Resolver,
                new DefaultObjectBinderLocator()).CreateOperations(new[] { TypeSystem.FromClr<THandler>() }).ToList();
        }

        protected void when_filtering_operations()
        {
            FilteredOperations = Filter.Process(Operations).ToList();
        }

        protected void given_operation_value(string methodName, string parameterName, object parameterValue)
        {
            Operations.First(x => x.Name == methodName).Inputs.Required().First(x => x.Member.Name == parameterName).Binder.SetInstance(parameterValue)
                .ShouldBeTrue();
        }
    }
}