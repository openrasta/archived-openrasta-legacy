using OpenRasta.DI;
using OpenRasta.OperationModel;
using OpenRasta.OperationModel.Filters;
using OpenRasta.Pipeline;

namespace OpenRasta.Pipeline.Contributors
{
    public class OperationFilterContributor :
        AbstractOperationProcessing<IOperationFilter, KnownStages.IOperationFiltering>,
        KnownStages.IOperationFiltering
    {
        public OperationFilterContributor(IDependencyResolver resolver) : base(resolver)
        {
        }

        protected override void InitializeWhen(IPipelineExecutionOrder pipeline)
        {
            pipeline.After<KnownStages.IOperationCreation>();
        }
    }
}