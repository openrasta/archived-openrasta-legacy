using System.Collections.Generic;

namespace OpenRasta.TypeSystem
{
    /// <summary>
    /// Represents the component responsible for generating and parsing object paths.
    /// </summary>
    public interface IPathManager
    {
        /// <summary>
        /// Splits an object path into its components.
        /// </summary>
        /// <param name="objectPath">The object path to parse.</param>
        /// <returns>The individual components in the path.</returns>
        IEnumerable<PathComponent> ReadComponents(string objectPath);

        /// <summary>
        /// Gets the type of the provided object path, given a list of prefixes.
        /// </summary>
        /// <param name="prefixes">The prefixes to ignore when parsing, usually the type name and parameter name.</param>
        /// <param name="objectPath">The object path to parse.</param>
        /// <returns></returns>
        PathComponent GetPathType(IEnumerable<string> prefixes, string objectPath);

        /// <summary>
        /// Writes the components of a member access path to a string.
        /// </summary>
        /// <param name="components">The list of components that constitute the object path.</param>
        /// <returns></returns>
        string WriteComponents(IEnumerable<PathComponent> components);
    }
}