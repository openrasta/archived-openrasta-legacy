namespace OpenRasta.Pipeline
{
    public static class PipelineExecutionOrderExtensions
    {
        public static IPipelineExecutionOrderAnd After<TContributor>(this IPipelineExecutionOrder pipeline) where TContributor : IPipelineContributor
        {
            return pipeline.After(typeof(TContributor));
        }
        public static IPipelineExecutionOrderAnd Before<TContributor>(this IPipelineExecutionOrder pipeline) where TContributor : IPipelineContributor
        {
            return pipeline.Before(typeof(TContributor));
        }
    }
}