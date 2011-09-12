using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenRasta.Binding;

namespace OpenRasta.TypeSystem.Surrogated
{
    [DebuggerDisplay("Name={OriginalNativeType.Name}, Alien={OriginalAlienType.Name}")]
    public class AlienType : AlienMember, IType
    {
        public AlienType(IType alienType, IMember nativeType)
            : base(alienType, nativeType)
        {
            OriginalAlienType = alienType;
            OriginalNativeType = nativeType;
        }

        public override IType Type
        {
            get { return this; }
        }

        protected IType OriginalAlienType { get; private set; }
        protected IMember OriginalNativeType { get; set; }

        public int CompareTo(IType other)
        {
            return OriginalAlienType.CompareTo(other);
        }

        public ITypeBuilder CreateBuilder()
        {
            throw new NotImplementedException();
        }

        public object CreateInstance(params object[] arguments)
        {
            return OriginalAlienType.CreateInstance();
        }

        public object CreateInstance()
        {
            return OriginalAlienType.CreateInstance();
        }

        public bool IsAssignableFrom(IType type)
        {
            return OriginalAlienType.IsAssignableFrom(type);
        }

        public bool TryCreateInstance<T>(IEnumerable<T> values, ValueConverter<T> converter, out object result)
        {
            return OriginalAlienType.TryCreateInstance(values, converter, out result);
        }
    }
}