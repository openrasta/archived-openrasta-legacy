using System;

namespace OpenRasta.TypeSystem
{
    /// <summary>
    /// Represents the type system used by OpenRasta for manipulating types.
    /// </summary>
    public interface ITypeSystem
    {
        /// <summary>
        /// Gets the surrogate provider used for wrapping types with other types.
        /// </summary>
        ISurrogateProvider SurrogateProvider { get; }
        /// <summary>
        /// Gets the instance of the path manager used to parse and generate string components.
        /// </summary>
        IPathManager PathManager { get; }
        /// <summary>
        /// Gets a type given an instance of a CLR Type object.
        /// </summary>
        /// <param name="type">The CLR Type for which to retrieve an <see cref="IType"/>.</param>
        /// <returns>An implementation of <see cref="IType"/>.</returns>
        IType FromClr(Type type);
        /// <summary>
        /// Gets an implementation of an <see cref="IType"/> object from an instance of an object.
        /// </summary>
        /// <param name="instance">The instance of an object for which to retrieve an <see cref="IType"/>.</param>
        /// <returns>An implementatino of <see cref="IType"/>.</returns>
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