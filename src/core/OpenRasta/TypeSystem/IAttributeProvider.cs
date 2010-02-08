using System;
using System.Collections.Generic;

namespace OpenRasta.TypeSystem
{
    /// <summary>
    /// Represents an object that provides attributes.
    /// </summary>
    public interface IAttributeProvider
    {
        /// <summary>
        /// Find an attribute of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to read.</typeparam>
        /// <returns>The instance of the attribute, or <c>null</c> if none was found.</returns>
        T FindAttribute<T>() where T : class;

        /// <summary>
        /// Find all attributes of the specified type defined on a member.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to read.</typeparam>
        /// <returns>Instances of the attribute, or an empty list if none were found.</returns>
        IEnumerable<T> FindAttributes<T>() where T : class;
    }
}