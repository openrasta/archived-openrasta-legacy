// ReSharper disable UnusedTypeParameter
using OpenRasta.Pipeline;

namespace OpenRasta.OperationModel
{
    public interface IOperationProcessor<T> : IOperationProcessor
        where T : IPipelineContributor
    {
    }
}

// ReSharper restore UnusedTypeParameter