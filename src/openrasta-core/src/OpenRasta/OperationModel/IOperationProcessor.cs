using System.Collections.Generic;

namespace OpenRasta.OperationModel
{
    public interface IOperationProcessor
    {
        IEnumerable<IOperation> Process(IEnumerable<IOperation> operations);        
    }
}