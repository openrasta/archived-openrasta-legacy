using System;

namespace OpenRasta.Pipeline
{
    public interface IPipelineExecutionOrder
    {
        IPipelineExecutionOrderAnd Before(Type contributorType);
        IPipelineExecutionOrderAnd After(Type contributorType);
    }
}