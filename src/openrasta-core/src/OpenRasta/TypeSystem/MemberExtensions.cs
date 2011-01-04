using System;

namespace OpenRasta.TypeSystem
{
    public static class MemberExtensions
    {
        public static bool IsAssignableTo<T>(this IMember member)
        {
            return member.IsAssignableTo(typeof(T));
        }

        public static bool IsAssignableTo(this IMember type, Type parentType)
        {
            return type.TypeSystem.FromClr(parentType).IsAssignableFrom(type.Type);
        }
        public static bool IsAssignableTo(this IMember member, IMember other)
        {
            return other.Type.IsAssignableFrom(member.Type);
        }
        public static bool IsAssignableFrom<T>(this IMember member)
        {
            return member.Type.IsAssignableFrom(member.TypeSystem.FromClr<T>());
        }
    }
}