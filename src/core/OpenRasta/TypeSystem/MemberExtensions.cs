namespace OpenRasta.TypeSystem
{
    public static class MemberExtensions
    {
        public static bool IsAssignableTo<T>(this IMember member)
        {
            return member.IsAssignableTo(member.TypeSystem.FromClr(typeof(T)));
        }
        public static bool IsAssignableFrom<T>(this IMember member)
        {
            return member.TypeSystem.FromClr(typeof(T)).IsAssignableTo(member);
        }
    }
}