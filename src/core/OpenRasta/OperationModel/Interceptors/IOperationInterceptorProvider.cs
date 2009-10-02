using System.Collections.Generic;

namespace OpenRasta.OperationModel.Interceptors
{
    public interface IOperationInterceptorProvider
    {
        IEnumerable<IOperationInterceptor> GetInterceptors(IOperation operation);
    }
}