using System.Collections.Generic;
using System.Linq;
using OpenRasta.DI;

namespace OpenRasta.OperationModel.Interceptors
{
    public class SystemAndAttributesOperationInterceptorProvider : IOperationInterceptorProvider
    {
        readonly IOperationInterceptor[] _systemInterceptors;

        public SystemAndAttributesOperationInterceptorProvider(IDependencyResolver resolver)
            : this(resolver.ResolveAll<IOperationInterceptor>().ToArray())
        {
        }

        public SystemAndAttributesOperationInterceptorProvider(IOperationInterceptor[] systemInterceptors)
        {
            _systemInterceptors = systemInterceptors;
        }

        public IEnumerable<IOperationInterceptor> GetInterceptors(IOperation operation)
        {
            return _systemInterceptors
                .Concat(GetInterceptorAttributes(operation))
                .Concat(GetInterceptorProviderAttributes(operation))
                .ToList();
        }

        static IEnumerable<IOperationInterceptor> GetInterceptorAttributes(IOperation operation)
        {
            return operation.FindAttributes<IOperationInterceptor>();
        }

        static IEnumerable<IOperationInterceptor> GetInterceptorProviderAttributes(IOperation operation)
        {
            return operation.FindAttributes<IOperationInterceptorProvider>().SelectMany(x => x.GetInterceptors(operation));
        }
    }
}