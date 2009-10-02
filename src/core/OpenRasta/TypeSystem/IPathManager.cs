using System.Collections.Generic;
using OpenRasta.TypeSystem.ReflectionBased;

namespace OpenRasta.TypeSystem
{
    /// <summary>
    /// Represents the componenet responsible for generating and parsing object paths.
    /// </summary>
    public interface IPathManager
    {
        /// <summary>
        /// Gets the list of components in an object path.
        /// </summary>
        /// <param name="objectPath">The object path to parse.</param>
        /// <returns>The individual components in the path.</returns>
        IEnumerable<PathComponent> ReadComponents(string objectPath);

        PathComponent GetPathType(IEnumerable<string> prefixes, string objectPath);
        string WriteComponents(IEnumerable<PathComponent> components);
    }
}