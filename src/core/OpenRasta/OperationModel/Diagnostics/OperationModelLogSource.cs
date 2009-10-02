using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenRasta.Diagnostics;

namespace OpenRasta.OperationModel.Diagnostics
{
    [LogCategory("openrasta.operationmodel")]
    public class OperationModelLogSource : ILogSource
    {
    }
    public static class OperationModelLogSourceExtensions
    {
        public static void NoResourceOrUriName(this ILogger<OperationModelLogSource> log)
        {
            log.WriteDebug("No resource or no uri name. Not filtering.");
        } 
        public static void FoundOperations(this ILogger<OperationModelLogSource> log, ICollection<IOperation> operations)
        {
            log.WriteDebug("Found {0} operations with correct attributes", operations.Count);
        }
    }
}
