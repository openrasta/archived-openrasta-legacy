using System;
using OpenRasta.Web;

namespace OpenRasta.Pipeline
{
    public static class LegacyPipelineExtensions
    {

        [Obsolete("Please use Notify(Method).After<>()")]
        public static void ExecuteAfter<T>(this IPipeline pipeline, Func<ICommunicationContext, PipelineContinuation> notificationAction)
            where T : IPipelineContributor
        {
            pipeline.Notify(notificationAction).After<T>();
        }
        [Obsolete("Please use Notify(Method).Before<>()")]
        public static void ExecuteBefore<T>(this IPipeline pipeline,  Func<ICommunicationContext, PipelineContinuation> notificationAction)
            where T : IPipelineContributor
        {
            if (typeof(T) == typeof(KnownStages.IBegin))
                throw new InvalidOperationException(
                    "The BootstrapContributor is always the first contributor to be called.");

            pipeline.Notify(notificationAction).Before<T>();
        }
    }
}