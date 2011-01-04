using System.Collections;
using System.Collections.Generic;
using OpenRasta.TypeSystem;

namespace OpenRasta.OperationModel
{
    public interface IOperation : IAttributeProvider
    {
        IEnumerable<InputMember> Inputs { get; }
        IDictionary ExtendedProperties { get; }
        string Name { get; }
        IEnumerable<OutputMember> Invoke();
    }
}