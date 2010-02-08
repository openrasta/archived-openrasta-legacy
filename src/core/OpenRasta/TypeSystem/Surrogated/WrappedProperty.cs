using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenRasta.Binding;

namespace OpenRasta.TypeSystem.Surrogated
{
    [DebuggerDisplay("{global::OpenRasta.TypeSystem.DebuggerStrings.Property(_property)}")]
    public class WrappedProperty : WrappedMember, IProperty
    {
        readonly IProperty _property;

        public WrappedProperty(IMember owner, IProperty property)
            : base(property)
        {
            if (property == null) throw new ArgumentNullException("property");
            _property = property;
            Owner = owner;
        }

        public bool CanWrite
        {
            get { return _property.CanWrite; }
        }

        public IMember Owner { get; private set; }

        public object[] PropertyParameters
        {
            get { return _property.PropertyParameters; }
        }

        public IPropertyBuilder CreateBuilder(IMemberBuilder builder)
        {
            return _property.CreateBuilder(builder);
        }

        public IEnumerable<IMember> GetCallStack()
        {
            IMember current = this;
            while (current != null)
            {
                yield return current;
                var currentIsProp = current as IProperty;
                if (currentIsProp != null)
                    current = currentIsProp.Owner;
                else
                    break;
            }
        }

        public object GetValue(object target)
        {
            return _property.GetValue(target);
        }

        public bool TrySetValue<T>(object target, IEnumerable<T> values, ValueConverter<T> converter)
        {
            return _property.TrySetValue(target, values, converter);
        }

        public bool TrySetValue(object target, object value)
        {
            return _property.TrySetValue(target, value);
        }
    }
}