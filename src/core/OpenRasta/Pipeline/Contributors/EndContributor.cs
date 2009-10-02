using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenRasta.Web;
using OpenRasta.Pipeline;

namespace OpenRasta.Pipeline.Contributors
{
    public class EndContributor : KnownStages.IEnd
    {
        public void Initialize(IPipeline pipelineRunner)
        {
            var notification = pipelineRunner.Notify(ReturnFinished);
            IPipelineExecutionOrderAnd and = null;
            foreach(var contributor in pipelineRunner.Contributors.Where(x=>x != this))
            {
                if (and == null)
                {
                    and = notification.After(contributor.GetType());
                }
                else
                {
                    and = and.And.After(contributor.GetType());
                }
            }
        }

        PipelineContinuation ReturnFinished(ICommunicationContext arg)
        {
            return PipelineContinuation.Finished;
        }
    }
}