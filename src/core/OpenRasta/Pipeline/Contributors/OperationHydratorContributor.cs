using OpenRasta.DI;
using OpenRasta.OperationModel;
using OpenRasta.Web;
using OpenRasta.Pipeline;

namespace OpenRasta.Pipeline.Contributors
{
    public class OperationHydratorContributor :
        AbstractOperationProcessing<IOperationHydrator, KnownStages.IRequestDecoding>,
        KnownStages.IRequestDecoding
    {
        public OperationHydratorContributor(IDependencyResolver resolver) : base(resolver)
        {
        }

        protected override void InitializeWhen(IPipelineExecutionOrder pipeline)
        {
            pipeline.After<KnownStages.ICodecRequestSelection>();
        }
        protected override PipelineContinuation OnOperationsEmpty(ICommunicationContext context)
        {
            return base.OnOperationsEmpty(context);
        }
    }
}