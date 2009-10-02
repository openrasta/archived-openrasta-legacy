using System;
using System.Collections.Generic;
using System.Linq;
using OpenRasta.DI;
using OpenRasta.OperationModel;
using OpenRasta.Web;
using OpenRasta.Pipeline;

namespace OpenRasta.Pipeline.Contributors
{
    public abstract class AbstractOperationProcessing<TProcessor, TStage> : IPipelineContributor
        where TProcessor : IOperationProcessor<TStage>
        where TStage : IPipelineContributor
    {
        readonly IDependencyResolver _resolver;

        protected AbstractOperationProcessing(IDependencyResolver resolver)
        {
            _resolver = resolver;
        }

        public virtual PipelineContinuation ProcessOperations(ICommunicationContext context)
        {
            context.PipelineData.Operations = ProcessOperations(context.PipelineData.Operations).ToList();
            if (context.PipelineData.Operations.Count() == 0)
                return OnOperationsEmpty(context);
            return OnOperationProcessingComplete(context.PipelineData.Operations) ?? PipelineContinuation.Continue;
        }

        public virtual IEnumerable<IOperation> ProcessOperations(IEnumerable<IOperation> operations)
        {
            var chain = GetMethods().Chain();
            return chain == null ? new IOperation[0] : chain(operations);
        }

        public virtual void Initialize(IPipeline pipelineRunner)
        {
            InitializeWhen(pipelineRunner.Notify(ProcessOperations));
        }

        protected abstract void InitializeWhen(IPipelineExecutionOrder pipeline);

        protected virtual PipelineContinuation? OnOperationProcessingComplete(IEnumerable<IOperation> ops)
        {
            return null;
        }

        protected virtual PipelineContinuation OnOperationsEmpty(ICommunicationContext context)
        {
            context.OperationResult = new OperationResult.MethodNotAllowed();

            return PipelineContinuation.RenderNow;
        }

        IEnumerable<Func<IEnumerable<IOperation>, IEnumerable<IOperation>>> GetMethods()
        {
            var operationProcessors = _resolver.ResolveAll<TProcessor>();

            foreach (var filter in operationProcessors)
                yield return filter.Process;
        }
    }
}