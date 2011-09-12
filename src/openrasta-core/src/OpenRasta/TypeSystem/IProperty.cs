using System.Collections.Generic;
using OpenRasta.Binding;

namespace OpenRasta.TypeSystem
{
    public interface IProperty : IMember
    {
        /// <summary>
        /// Defines the parameters for indexed properties
        /// </summary>
        object[] PropertyParameters { get; }

        /// <summary>
        /// Defines the member (an <see cref="IType"/> or another <see cref="IProperty"/>) owning this property
        /// </summary>
        IMember Owner { get; }

        /// <summary>
        /// Gets a value defining if the property can be written to.
        /// </summary>
        bool CanWrite { get; }

        /// <summary>
        /// Returns a new instance of an instance builder.
        /// </summary>
        /// <returns></returns>
        IPropertyBuilder CreateBuilder(IMemberBuilder parentBuilder);

        /// <summary>
        /// Gets a list of the intermediate properties required along the chain call to assign this property.
        /// </summary>
        IEnumerable<IMember> GetCallStack();

        /// <summary>
        /// Reads the value of this property on the target instance.
        /// </summary>
        /// <param name="target"></param>
        /// <returns>The value of the property</returns>
        object GetValue(object target);

        /// <summary>
        /// Tries to set the value of this property on an instance of an object.
        /// </summary>
        /// <param name="target">The instance on which to assign the value.</param>
        /// <param name="value">The value to assign.</param>
        /// <returns><c>true</c> if the assignment was successful, otherwise <c>false</c>.</returns>
        bool TrySetValue(object target, object value);

        /// <summary>
        /// Tries to assign the value of this property on an instance of an object, using a series of properties and a converter.
        /// </summary>
        bool TrySetValue<T>(object target, IEnumerable<T> values, ValueConverter<T> converter);
    }
}