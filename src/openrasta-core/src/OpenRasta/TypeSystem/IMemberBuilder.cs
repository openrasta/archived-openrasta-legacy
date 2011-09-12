using System.Collections.Generic;
using OpenRasta.Binding;

namespace OpenRasta.TypeSystem
{
    public interface IMemberBuilder
    {
        IMember Member { get; }

        object Value { get; }
        bool HasValue { get; }
        bool CanWrite { get; }

        IPropertyBuilder GetProperty(string propertyPath);
        bool TrySetValue(object value);
        bool TrySetValue<T>(IEnumerable<T> values, ValueConverter<T> converter);

        object Apply(object target, out object assignedValue);
    }
}