#region License

/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion

using System;
using System.Collections.Generic;
using OpenRasta.Binding;

namespace OpenRasta.TypeSystem
{
    public class PropertyBuilder : MemberBuilder, IPropertyBuilder
    {
        object _cachedValue;
        bool _hasValue;

        public PropertyBuilder(IProperty property)
            : base(property)
        {
        }

        public int IndexAtCreation { get; set; }

        public IProperty Property
        {
            get { return Member as IProperty; }
        }

        public override bool CanWrite
        {
            get { return Property.CanWrite; }
        }

        public IMemberBuilder Owner { get; private set; }

        public override object Value
        {
            get
            {
                if (Owner != null && _hasValue)
                    return Property.GetValue(Owner.Value);
                return _cachedValue;
            }
        }

        public override bool HasValue
        {
            get { return _hasValue; }
        }

        object CachedValue
        {
            get
            {
                return _cachedValue;
            }

            set
            {
                _cachedValue = value;
                _hasValue = true;
            }
        }

        public override bool TrySetValue(object value)
        {
            if (Owner != null)
            {
                _hasValue = true;
                return SetValueOnParent(value);
            }
            if (!Property.CanSetValue(value))
                return false;
            CachedValue = value;

            return true;
        }

        /// <summary>
        /// Tries to assign a property value and return true if it was successfully assigned or if the parent wasn't available.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public override bool TrySetValue<T>(IEnumerable<T> values, ValueConverter<T> converter)
        {
            if (Owner != null)
            {
                EnsureParentHasValue();
                return Property.TrySetValue(Owner.Value, values, converter);
            }

            object convertedValue;
            bool success = Property.Type.TryCreateInstance(values, converter, out convertedValue);

            if (!success) return false;
            CachedValue = convertedValue;

            return true;
        }

        /// <exception cref="ArgumentNullException"><c>parentBuilder</c> is null.</exception>
        /// <exception cref="InvalidOperationException">The property instance has already been attached to a parent.</exception>
        public virtual void SetOwner(IMemberBuilder parentBuilder)
        {
            if (parentBuilder == null) throw new ArgumentNullException("parentBuilder");
            if (Owner != null)
                throw new InvalidOperationException("The property instance has already been attached to a parent.");

            Owner = parentBuilder;
            SetValueOnParent(CachedValue);
        }

        protected virtual bool SetValueOnParent(object value)
        {
            EnsureParentHasValue();
            return Property.TrySetValue(Owner.Value, value);
        }

        void EnsureParentHasValue()
        {
            if (Owner.Value == null)
                Owner.TrySetValue(Owner.Member.Type.CreateInstance());
        }
    }
}

#region Full license

// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion