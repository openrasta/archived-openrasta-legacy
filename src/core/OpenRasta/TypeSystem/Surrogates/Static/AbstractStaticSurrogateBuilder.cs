using System;

namespace OpenRasta.TypeSystem.Surrogates.Static
{
    public abstract class AbstractStaticSurrogateBuilder : ISurrogateBuilder
    {
        public bool CanCreateFor(IMember type)
        {
            var native = type as INativeMember;
            if (native == null) return false;
            var targetType = native.NativeType;
            return CanCreateFor(targetType);
        }

        public abstract bool CanCreateFor(Type type);

        public IType Create(IMember type)
        {
            return type.TypeSystem.FromClr(Create(((INativeMember)type).NativeType));
        }

        public abstract Type Create(Type type);
    }
}