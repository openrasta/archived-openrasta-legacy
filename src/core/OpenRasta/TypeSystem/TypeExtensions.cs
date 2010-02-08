using System;
using OpenRasta.DI;

namespace OpenRasta.TypeSystem
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns an instance of the type, optionally through the container if it is supported.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public static object CreateInstance(this IType type, IDependencyResolver resolver)
        {
            var typeForResolver = type as IResolverAwareType;
            return resolver == null || typeForResolver == null ? type.CreateInstance() : typeForResolver.CreateInstance(resolver);
        }

        public static bool Equals<T>(this IType type)
        {
            return type.TypeSystem.FromClr<T>().Equals(type);
        }
    }
}