using System;

namespace OpenRasta.TypeSystem
{
    public static class TypeExtensions
    {
        public static bool Equals<T>(this IType type)
        {
            return type.TypeSystem.FromClr<T>().Equals(type);
        }
    }
}