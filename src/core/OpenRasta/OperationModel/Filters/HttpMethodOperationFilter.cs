using System;
using System.Collections.Generic;
using System.Linq;
using OpenRasta.Diagnostics;
using OpenRasta.Web;

namespace OpenRasta.OperationModel.Filters
{
    public class HttpMethodOperationFilter : IOperationFilter
    {
        readonly IRequest _request;

        public HttpMethodOperationFilter(IRequest request)
        {
            _request = request;
            Log = NullLogger.Instance;
        }

        public ILogger Log { get; set; }

        public IEnumerable<IOperation> Process(IEnumerable<IOperation> operations)
        {
            operations = operations.ToList();
            var operationWithMatchingName = OperationsWithMatchingName(operations);
            var operationWithMatchingAttribute = OperationsWithMatchingAttribute(operations);
            Log.WriteDebug("Found {0} operation(s) with a matching name.", operationWithMatchingName.Count());
            Log.WriteDebug("Found {0} operation(s) with matching [HttpOperation] attribute.", operationWithMatchingAttribute.Count());
            return operationWithMatchingName.Union(operationWithMatchingAttribute);
        }

        IEnumerable<IOperation> OperationsWithMatchingAttribute(IEnumerable<IOperation> operations)
        {
            return from operation in operations
                   let httpAttribute = operation.FindAttribute<HttpOperationAttribute>()
                   where httpAttribute != null && httpAttribute.MatchesHttpMethod(_request.HttpMethod)
                   select operation;
        }

        IEnumerable<IOperation> OperationsWithMatchingName(IEnumerable<IOperation> operations)
        {
            return from operation in operations
                   where operation.Name.StartsWith(_request.HttpMethod, StringComparison.OrdinalIgnoreCase)
                   select operation;
        }
    }
}