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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenRasta.Binding;

namespace OpenRasta.TypeSystem
{
    public class TypeBuilder : MemberBuilder, ITypeBuilder
    {
        object _cacheValue;

        public TypeBuilder(IType type)
            : base(null, type)
        {
            Changes = new PropertyDictionary(this);
        }

        public IDictionary<string, IPropertyBuilder> Changes { get; private set; }

        public override bool HasValue
        {
            get { return _cacheValue != null || Changes.Count > 0; }
        }

        public IType Type
        {
            get { return Member as IType; }
        }

        public override object Value
        {
            get { return _cacheValue; }
        }

        public override object Apply(object target, out object assignedValue)
        {
            assignedValue = _cacheValue ?? target;
            if (_cacheValue != null)
                return _cacheValue;
            return target;
        }

        public override bool TrySetValue(object value)
        {
            if (Type.CanSetValue(value))
            {
                _cacheValue = value;
                return true;
            }

            return false;
        }

        public override bool TrySetValue<T>(IEnumerable<T> values, ValueConverter<T> converter)
        {
            object result;
            if (!Type.TryCreateInstance(values, converter, out result))
                return false;
            _cacheValue = result;
            return true;
        }

        /// <exception cref="ArgumentNullException"><c>instance</c> is null.</exception>
        public object Create()
        {
            var args = (from arg in Assignment.Children.Values
                        select arg.Builder.Value).ToArray();

            return AssignFrame(_cacheValue ?? Type.CreateInstance(args.ToArray()), Assignment);
        }

        /// <exception cref="ArgumentNullException"><c>instance</c> is null.</exception>
        public object Update(object instance)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            return AssignFrame(instance, Assignment);
        }
        object AssignFrame(object instance, AssignmentFrame currentFrame)
        {
            object assignedValue;
            var hasChildren = currentFrame.Children.Any();
            
            instance = currentFrame.Builder.Apply(instance, out assignedValue);

            object newValue = assignedValue;
            foreach (var childFrame in currentFrame.Children.Values)
                newValue = AssignFrame(newValue, childFrame);

            if (hasChildren && currentFrame.Builder.Value != newValue)
            {
                var oldValue = currentFrame.Builder.Value;
                currentFrame.Builder.TrySetValue(newValue);
                instance = currentFrame.Builder.Apply(instance, out assignedValue);
                currentFrame.Builder.TrySetValue(oldValue);
            }
            return instance;
        }

        class PropertyDictionary : IDictionary<string, IPropertyBuilder>
        {
            public PropertyDictionary(TypeBuilder owner)
            {
                Owner = owner;
            }

            public int Count
            {
                get { return TheOnesWithValues().Count(); }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public ICollection<string> Keys
            {
                get { return TheOnesWithValues().Select(kv => kv.Key).ToList(); }
            }

            public TypeBuilder Owner { get; private set; }

            public ICollection<IPropertyBuilder> Values
            {
                get { return TheOnesWithValues().Select(kv => kv.Value).ToList(); }
            }

            public IPropertyBuilder this[string key]
            {
                get
                {
                    IPropertyBuilder property;
                    if (!TryGetValue(key, out property))
                        throw new ArgumentOutOfRangeException();
                    return property;
                }

                set
                {
                    throw new NotSupportedException();
                }
            }

            public void Add(KeyValuePair<string, IPropertyBuilder> item)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(KeyValuePair<string, IPropertyBuilder> item)
            {
                return ContainsKey(item.Key);
            }

            public void CopyTo(KeyValuePair<string, IPropertyBuilder>[] array, int arrayIndex)
            {
                throw new NotSupportedException();
            }

            public bool Remove(KeyValuePair<string, IPropertyBuilder> item)
            {
                throw new NotSupportedException();
            }

            public void Add(string key, IPropertyBuilder value)
            {
                throw new NotSupportedException();
            }

            public bool ContainsKey(string key)
            {
                return Owner.PropertiesCache.ContainsKey(key) && Owner.PropertiesCache[key] != null &&
                       Owner.PropertiesCache[key].HasValue;
            }

            public bool Remove(string key)
            {
                throw new NotSupportedException();
            }

            public bool TryGetValue(string key, out IPropertyBuilder value)
            {
                if (ContainsKey(key))
                {
                    value = Owner.PropertiesCache[key];
                    return true;
                }

                value = null;
                return false;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public IEnumerator<KeyValuePair<string, IPropertyBuilder>> GetEnumerator()
            {
                foreach (var value in TheOnesWithValues())
                    yield return value;
            }

            IEnumerable<KeyValuePair<string, IPropertyBuilder>> TheOnesWithValues()
            {
                return Owner.PropertiesCache.Where(kv => kv.Value != null && kv.Value.HasValue);
            }
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