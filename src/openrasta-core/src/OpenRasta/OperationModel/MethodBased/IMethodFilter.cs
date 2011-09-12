using System.Collections.Generic;
using OpenRasta.TypeSystem;

namespace OpenRasta.OperationModel.MethodBased
{
    public interface IMethodFilter
    {
        IEnumerable<IMethod> Filter(IEnumerable<IMethod> methods);
    }
}