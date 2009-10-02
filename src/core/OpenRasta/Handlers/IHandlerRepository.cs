using System.Collections.Generic;
using OpenRasta.TypeSystem;

namespace OpenRasta.Handlers
{
    public interface IHandlerRepository
    {
        void AddResourceHandler(object resourceKey, IType handlerType);

        void Clear();

        IEnumerable<IType> GetHandlerTypesFor(object resourceKey);
    }
}