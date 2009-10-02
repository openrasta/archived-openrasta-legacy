using System.Collections.Generic;
using OpenRasta.Web;

namespace OpenRasta.OperationModel
{
    public interface IOperationExecutor
    {
        OperationResult Execute(IEnumerable<IOperation> operations);
    }
}