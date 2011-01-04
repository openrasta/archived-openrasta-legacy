using System;
using System.Collections.Generic;

namespace OpenRasta.TypeSystem
{
    /// <summary>
    /// Represents an object that can create new objects or update existing objects
    /// from a list of property values.
    /// </summary>
    public interface ITypeBuilder : IMemberBuilder
    {
        /// <summary>
        /// Gets the list of property changes available to this type.
        /// </summary>
        IDictionary<string, IPropertyBuilder> Changes { get; }

        /// <summary>
        /// Gets the <see cref="IType"/> for which properties are being set.
        /// </summary>
        IType Type { get; }

        /// <summary>
        /// Updates an object instance with the property values contained by the
        /// type builder.
        /// </summary>
        /// <param name="destination">The destination instance.</param>
        /// <returns>The destination object.</returns>
        /// <remarks>The instance is returned by this method for the case where
        /// a copy-by-value object is being returned, such as a <see cref="DateTime"/> object.
        /// </remarks>
        object Update(object destination);

        /// <summary>
        /// Creates an instance of an object of the type specified in the Type property.
        /// </summary>
        /// <returns></returns>
        object Create();
    }
}