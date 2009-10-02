using System.Collections.Generic;

namespace OpenRasta.TypeSystem
{
    public interface IMethod : IAttributeProvider
    {
        IMember Owner { get; }
        string Name { get; }
        IEnumerable<IParameter> InputMembers { get; }
        IEnumerable<IMember> OutputMembers { get; }
        IEnumerable<object> Invoke(object target, params object[] parameters);
    }
}