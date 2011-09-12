using System;
using System.Collections.Generic;

namespace OpenRasta.TypeSystem
{
    /// <summary>
    /// Represents a member.
    /// </summary>
    public interface IMember : IAttributeProvider
    {
        /// <summary>
        /// Gets the name of the current member.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the name of the type of the current member.
        /// </summary>
        string TypeName { get; }

        /// <summary>
        /// Gets the type system from which this type was created.
        /// </summary>
        ITypeSystem TypeSystem { get; set; }

        /// <summary>
        /// Gets the type of the current member
        /// </summary>
        IType Type { get; }

        Type StaticType { get; }

        /// <summary>
        /// Gets a value defining if the member is an enumerable.
        /// </summary>
        /// <remarks>
        /// Dictionary and string types are not considered to be enumerable.
        /// </remarks>
        bool IsEnumerable { get; }

        /// <summary>
        /// Gets the first indexer accepting the value <paramref name="indexerValue"/> as a parameter.
        /// </summary>
        /// <param name="indexerValue">The indexer parameter value.</param>
        /// <returns>Returns an instance of <see cref="IProperty"/>, or <c>null</c> if none were found.</returns>
        IProperty GetIndexer(string indexerValue);

        /// <summary>
        /// Gets the first property defined on the member with a name matching the <paramref name="propertyName"/> value.
        /// </summary>
        /// <param name="propertyName">The name of the property to find</param>
        /// <returns></returns>
        IProperty GetProperty(string propertyName);

        /// <summary>
        /// Gets the first method with a name matching the <paramref name="methodName"/> value.
        /// </summary>
        /// <param name="methodName">The name of the method to find.</param>
        /// <returns></returns>
        IMethod GetMethod(string methodName);

        /// <summary>
        /// Gets the list of all available methods on this member.
        /// </summary>
        /// <returns></returns>
        IList<IMethod> GetMethods();

        /// <summary>
        /// Returns a value indicating the passed instance can be assigned to the current member.
        /// </summary>
        /// <param name="value">The value being assigned.</param>
        /// <returns><c>true</c> if the value can be assigned to this member, otherwise <c>false</c>.</returns>
        bool CanSetValue(object value);
    }
}