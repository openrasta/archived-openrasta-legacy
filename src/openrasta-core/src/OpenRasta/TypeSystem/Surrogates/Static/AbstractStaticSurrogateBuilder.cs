using System;

namespace OpenRasta.TypeSystem.Surrogates.Static
{
    public abstract class AbstractStaticSurrogateBuilder : ISurrogateBuilder
    {
        public bool CanCreateFor(IMember type)
        {
            return CanCreateFor(type.StaticType);
        }

        public abstract bool CanCreateFor(Type type);

        public IType Create(IMember type)
        {
            return type.TypeSystem.FromClr(Create(type.StaticType));
        }

        public abstract Type Create(Type type);
    }
}