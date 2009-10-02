using System.Collections.Generic;
using OpenRasta.TypeSystem;

namespace OpenRasta.OperationModel
{
    public interface IOperationCreator
    {
        IEnumerable<IOperation> CreateOperations(IEnumerable<IType> handlers);
    }
}