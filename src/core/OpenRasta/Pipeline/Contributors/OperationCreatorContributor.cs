using System.Collections.Generic;
using System.Linq;
using OpenRasta.Diagnostics;
using OpenRasta.OperationModel;
using OpenRasta.Pipeline.Diagnostics;
using OpenRasta.Web;
using OpenRasta.Pipeline;

namespace OpenRasta.Pipeline.Contributors
{
    public class OperationCreatorContributor : KnownStages.IOperationCreation
    {
        readonly IOperationCreator _creator;

        public OperationCreatorContributor(IOperationCreator creator)
        {
            _creator = creator;
            Logger = NullLogger<PipelineLogSource>.Instance;
        }

        public ILogger<PipelineLogSource> Logger { get; set; }

        public void Initialize(IPipeline pipelineRunner)
        {
            pipelineRunner.Notify(CreateOperations).After<KnownStages.IHandlerSelection>();
        }

        PipelineContinuation CreateOperations(ICommunicationContext context)
        {
            if (context.PipelineData.SelectedHandlers != null)
            {
                context.PipelineData.Operations = _creator.CreateOperations(context.PipelineData.SelectedHandlers).ToList();
                LogOperations(context.PipelineData.Operations);
                if (context.PipelineData.Operations.Count() == 0)
                {
                    context.OperationResult = CreateMethodNotAllowed(context);
                    return PipelineContinuation.RenderNow;
                }
            }
            return PipelineContinuation.Continue;
        }

        OperationResult.MethodNotAllowed CreateMethodNotAllowed(ICommunicationContext context)
        {
            return new OperationResult.MethodNotAllowed(context.Request.Uri, context.Request.HttpMethod, context.PipelineData.ResourceKey);
        }

        void LogOperations(IEnumerable<IOperation> operations)
        {
            if (operations.Count() > 0)
            {
                foreach (var operation in operations)
                    Logger.WriteDebug("Created operation named {0} with signature {1}", operation.Name, operation.ToString());
            }
            else
            {
                Logger.WriteDebug("No operation was created.");
            }
        }
    }
}
