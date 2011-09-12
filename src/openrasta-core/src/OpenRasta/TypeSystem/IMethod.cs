using System.Collections.Generic;

namespace OpenRasta.TypeSystem
{
    /// <summary>
    /// Represents a method on a type.
    /// </summary>
    public interface IMethod : IAttributeProvider
    {
        /// <summary>
        /// Gets the member owning the method.
        /// </summary>
        IMember Owner { get; }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the list of input members to a method.
        /// </summary>
        IEnumerable<IParameter> InputMembers { get; }

        /// <summary>
        /// Gets the list of output members to a method.
        /// </summary>
        IEnumerable<IMember> OutputMembers { get; }

        /// <summary>
        /// Invokes a method on a target instance.
        /// </summary>
        /// <param name="target">The target instance on which to call the method.</param>
        /// <param name="parameters">The parameters passed to the method when being invoked.</param>
        /// <returns>A list of objects returned by the method.</returns>
        IEnumerable<object> Invoke(object target, params object[] parameters);
    }
}