#region License

/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */

#endregion

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using OpenRasta.Binding;

namespace OpenRasta.TypeSystem.ReflectionBased
{
    [DebuggerDisplay("{global::OpenRasta.TypeSystem.DebuggerStrings.Property(this)}")]
    public class ReflectionBasedProperty : ReflectionBasedMember<IPropertyBuilder>, IProperty
    {
        public ReflectionBasedProperty(ITypeSystem typeSystem, IMember parent, PropertyInfo property, object[] propertyParameters)
            : base(typeSystem, property.PropertyType)
        {
            PropertyParameters = propertyParameters ?? new object[0];
            Property = property;
            Owner = parent;
        }

        /// <summary>
        /// Defines the PropertyInfo of the property as defined on the owner type
        /// </summary>
        public PropertyInfo Property { get; private set; }

        public override string Name
        {
            get { return Property.Name; }
        }

        /// <summary>
        /// Defines the parameters for indexer properties
        /// </summary>
        public object[] PropertyParameters { get; private set; }

        /// <summary>
        /// Defines the type owning this property
        /// </summary>
        public IMember Owner { get; private set; }

        public override bool CanSetValue(object value)
        {
            return base.CanSetValue(value) && Property.CanWrite;
        }

        public bool CanWrite
        {
            get { return Property.CanWrite; }
        }

        public bool TrySetValue(object target, object value)
        {
            if (!Property.CanWrite)
                return false;

            Property.SetValue(target, value, PropertyParameters);

            return true;
        }

        public bool TrySetValue<T>(object target, IEnumerable<T> values, ValueConverter<T> converter)
        {
            if (!Property.CanWrite)
                return false;
            Property.SetValue(target, Property.PropertyType.CreateInstanceFrom(values, converter), PropertyParameters);
            return true;
        }

        public IPropertyBuilder CreateBuilder(IMemberBuilder parentBuilder)
        {
            return new PropertyBuilder(parentBuilder, this);
        }

        public IEnumerable<IMember> GetCallStack()
        {
            IMember current = this;
            while (current != null)
            {
                yield return current;
                var currentIsProp = current as IProperty;
                if (currentIsProp != null)
                    current = currentIsProp.Owner;
                else
                    break;
            }
        }

        public virtual object GetValue(object target)
        {
            try
            {
                return Property.GetValue(target, PropertyParameters);
            }
            catch
            {
                return null;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ReflectionBasedProperty))
                return false;

            var other = (ReflectionBasedProperty) obj;
            bool isEqual = Property.Equals(other.Property);
            if (!isEqual) return false;
            // equality depends on the owner stacks being equal and the index parameters being equal
            if ((PropertyParameters == null && other.PropertyParameters != null)
                || (PropertyParameters != null && other.PropertyParameters == null)
                || (PropertyParameters != null && PropertyParameters.Length != other.PropertyParameters.Length))
                return false;

            if (PropertyParameters != null)
                for (int i = 0; i < PropertyParameters.Length; i++)
                {
                    if (!PropertyParameters[i].Equals(other.PropertyParameters[i]))
                        return false;
                }

            List<IMember> thisOwners = GetCallStack().Skip(1).ToList();
            List<IMember> otherOwners = other.GetCallStack().Skip(1).ToList();
            if (thisOwners.Count != otherOwners.Count) return false;

            for (int i = 0; i < thisOwners.Count; i++)
            {
                if (!thisOwners[i].Equals(otherOwners[i])) return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            int hashCode = Property.GetHashCode();
            hashCode = GetCallStack().Skip(1).Aggregate(hashCode, (code, owner) => code ^ owner.GetHashCode());
            if (PropertyParameters != null)
                hashCode = PropertyParameters.Aggregate(hashCode, (hash, param) => hash ^ param.GetHashCode());
            return hashCode;
        }
    }
}

#region Full license

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#endregion