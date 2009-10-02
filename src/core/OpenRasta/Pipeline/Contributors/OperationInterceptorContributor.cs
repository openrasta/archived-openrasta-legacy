using System.Linq;
using OpenRasta.DI;
using OpenRasta.OperationModel;
using OpenRasta.OperationModel.Interceptors;
using OpenRasta.Web;
using OpenRasta.Pipeline;

namespace OpenRasta.Pipeline.Contributors
{
    public class OperationInterceptorContributor : IPipelineContributor
    {
        readonly IDependencyResolver _resolver;
        readonly IOperationInterceptorProvider _operationInterceptorProvider;

        public OperationInterceptorContributor(IDependencyResolver resolver, IOperationInterceptorProvider operationInterceptorProvider)
        {
            _resolver = resolver;
            _operationInterceptorProvider = operationInterceptorProvider;
        }

        public void Initialize(IPipeline pipelineRunner)
        {
            pipelineRunner.Notify(WrapOperations)
                .After<KnownStages.IRequestDecoding>()
                .And
                .Before<KnownStages.IOperationExecution>();
        }

        PipelineContinuation WrapOperations(ICommunicationContext context)
        {
            context.PipelineData.Operations = from op in context.PipelineData.Operations
                                              let interceptors = _resolver.ResolveAll<IOperationInterceptor>()
                                                  .Concat(_operationInterceptorProvider.GetInterceptors(op))
                                              select (IOperation)new OperationWithInterceptors(op, interceptors);

            return PipelineContinuation.Continue;
        }
    }
}