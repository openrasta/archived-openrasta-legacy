using OpenRasta.DI;
using OpenRasta.OperationModel;
using OpenRasta.OperationModel.Interceptors;
using OpenRasta.Web;
using OpenRasta.Pipeline;

namespace OpenRasta.Pipeline.Contributors
{
    public class OperationInvokerContributor : KnownStages.IOperationExecution
    {
        readonly IDependencyResolver _resolver;

        public OperationInvokerContributor(IDependencyResolver resolver)
        {
            _resolver = resolver;
        }

        public void Initialize(IPipeline pipelineRunner)
        {
            pipelineRunner.Notify(ExecuteOperations).After<KnownStages.IRequestDecoding>();
        }

        PipelineContinuation ExecuteOperations(ICommunicationContext context)
        {
            var executor = _resolver.Resolve<IOperationExecutor>();
            try
            {

                context.OperationResult = executor.Execute(context.PipelineData.Operations);
            }
            catch (InterceptorException)
            {
                if (context.OperationResult != null)
                    return PipelineContinuation.RenderNow;
                throw;
            }
            return PipelineContinuation.Continue;
        }
    }
}