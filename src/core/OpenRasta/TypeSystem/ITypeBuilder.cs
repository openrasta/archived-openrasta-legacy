using System.Collections.Generic;

namespace OpenRasta.TypeSystem
{
    public interface ITypeBuilder : IMemberBuilder
    {
        IDictionary<string, IPropertyBuilder> Changes { get; }
        IType Type { get; }
        void Apply(object destination);
        object Create();
    }
}