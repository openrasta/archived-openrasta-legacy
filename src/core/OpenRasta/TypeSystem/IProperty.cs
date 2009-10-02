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
        /// Defines the member (an IType or another IProperty) owning this property
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
        new IPropertyBuilder CreateBuilder();

        /// <summary>
        /// Gets a list of the intermediate properties required along the chain call to assign this property.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IMember> GetCallStack();

        object GetValue(object target);
        bool TrySetValue(object target, object value);
        bool TrySetValue<T>(object target, IEnumerable<T> values, ValueConverter<T> converter);
    }
}