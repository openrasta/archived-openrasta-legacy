using System;

namespace OpenRasta.TypeSystem
{
    /// <summary>
    /// Represents the type system used by OpenRasta for executing objects.
    /// </summary>
    public interface ITypeSystem
    {
        /// <summary>
        /// Gets or sets the surrogate factory used to build surrogate types.
        /// </summary>
        ISurrogateFactory SurrogateFactory { get; set; }
        IType FromClr(Type t);
        IType FromInstance(object instance);
    }
    public static class TypeSystemExtensions
{
    public static IType FromClr<T>(this ITypeSystem typeSystem)
    {
        return typeSystem.FromClr(typeof(T));
    }
}
}