using OpenRasta.Pipeline;
using OpenRasta.Web;
using StructureMap.Pipeline;

namespace OpenRasta.DI.StructureMap.Pipeline.Contributors
{
	public class ReleaseAndDisposeAllHttpScopedObjects : IPipelineContributor
	{
		public void Initialize(IPipeline pipelineRunner)
		{
			pipelineRunner.Notify(ReleaseAndDispose)
				.After<KnownStages.IOperationExecution>()
				.And.Before<KnownStages.IOperationResultInvocation>();
		}

		private static PipelineContinuation ReleaseAndDispose(ICommunicationContext communicationContext)
		{
			HttpContextLifecycle.DisposeAndClearAll();
			return PipelineContinuation.Continue;
		}
	}
}