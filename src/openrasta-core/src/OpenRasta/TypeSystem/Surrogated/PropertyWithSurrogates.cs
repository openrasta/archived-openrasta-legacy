using System.Collections.Generic;
using System.Diagnostics;
using OpenRasta.Binding;

namespace OpenRasta.TypeSystem.Surrogated
{
    [DebuggerDisplay("{global::OpenRasta.TypeSystem.DebuggerStrings.Property(this)}")]
    public class PropertyWithSurrogates : MemberWithSurrogates, IProperty
    {
        readonly IProperty _nativeProperty;

        public PropertyWithSurrogates(IProperty nativeProperty, IEnumerable<IType> alienTypes)
            : base(nativeProperty, alienTypes)
        {
            _nativeProperty = nativeProperty;
        }

        public bool CanWrite
        {
            get { return _nativeProperty.CanWrite; }
        }

        public IMember Owner
        {
            get { return _nativeProperty.Owner; }
        }

        public object[] PropertyParameters
        {
            get { return _nativeProperty.PropertyParameters; }
        }

        public IPropertyBuilder CreateBuilder(IMemberBuilder parentBuilder)
        {
            return new PropertyWithSurrogatesBuilder(this, parentBuilder, AlienTypes);
        }

        public IEnumerable<IMember> GetCallStack()
        {
            return _nativeProperty.GetCallStack();
        }

        public object GetValue(object target)
        {
            return _nativeProperty.GetValue(target);
        }

        public bool TrySetValue<T>(object target, IEnumerable<T> values, ValueConverter<T> converter)
        {
            return _nativeProperty.TrySetValue(target, values, converter);
        }

        public bool TrySetValue(object target, object value)
        {
            return _nativeProperty.TrySetValue(target, value);
        }
    }
}