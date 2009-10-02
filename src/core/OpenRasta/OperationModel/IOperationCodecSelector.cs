using OpenRasta.Pipeline;

namespace OpenRasta.OperationModel
{
    public interface IOperationCodecSelector : IOperationProcessor<KnownStages.ICodecRequestSelection>
    {
    }
}