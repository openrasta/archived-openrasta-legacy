using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenRasta.Binding;

namespace OpenRasta.TypeSystem.Surrogated
{
    [DebuggerDisplay("Name={_wrappedType.Name}, FullName={_wrappedType.TargetType.ToString()}")]
    public class TypeWithSurrogates : MemberWithSurrogates, IType
    {
        readonly IType _wrappedType;

        public TypeWithSurrogates(IType wrappedType, IEnumerable<IType> alienTypes)
            : base(wrappedType, alienTypes)
        {
            _wrappedType = wrappedType;
        }

        public override IType Type
        {
            get { return this; }
        }

        public int CompareTo(IType other)
        {
            if (other is TypeWithSurrogates)
                return _wrappedType.CompareTo(((TypeWithSurrogates)other)._wrappedType);
            return _wrappedType.CompareTo(other);
        }

        public ITypeBuilder CreateBuilder()
        {
            return new TypeWithSurrogatesBuilder(this, AlienTypes);
        }

        public object CreateInstance()
        {
            return _wrappedType.CreateInstance();
        }

        public object CreateInstance(params object[] arguments)
        {
            return _wrappedType.CreateInstance(arguments);
        }

        public bool IsAssignableFrom(IType type)
        {
            return _wrappedType.IsAssignableFrom(type);
        }

        public bool TryCreateInstance<T>(IEnumerable<T> values, ValueConverter<T> converter, out object result)
        {
            return _wrappedType.TryCreateInstance(values, converter, out result);
        }
    }
}