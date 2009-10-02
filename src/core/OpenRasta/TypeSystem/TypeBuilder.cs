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
        readonly Dictionary<IMember, IPropertyBuilder> _propertyInstancesCache =
            new Dictionary<IMember, IPropertyBuilder>();

        object _generatedValue;

        bool _generatedValueIsCached;
        object _objectBeingAppliedTo;
        object _rootValue;

        public TypeBuilder(IType type)
            : base(type)
        {
            Changes = new PropertyDictionary(this);
        }

        public IType Type
        {
            get { return Member as IType; }
        }

        public IDictionary<string, IPropertyBuilder> Changes { get; private set; }

        public override bool HasValue
        {
            get { return _rootValue != null || Changes.Count > 0; }
        }

        public override IPropertyBuilder GetProperty(string propertyPath)
        {
            EnsureWriteable();
            return base.GetProperty(propertyPath);
        }

        public override object Value
        {
            get { return _generatedValueIsCached ? _generatedValue : _objectBeingAppliedTo ?? _rootValue; }
        }

        public override bool TrySetValue(object value)
        {
            EnsureWriteable();
            if (Type.CanSetValue(value))
            {
                _rootValue = value;
                return true;
            }
            return false;
        }

        public override bool TrySetValue<T>(IEnumerable<T> values, ValueConverter<T> converter)
        {
            EnsureWriteable();

            object result;
            if (!Type.TryCreateInstance(values, converter, out result))
                return false;
            _rootValue = result;
            return true;
        }

        /// <exception cref="ArgumentNullException"><c>instance</c> is null.</exception>
        public object Create()
        {
            if (_generatedValueIsCached)
                return _generatedValue;

            bool rootIsNew = _rootValue == null;
            _rootValue = _rootValue ?? Type.CreateInstance();

            ProcessAllProperties();

            _generatedValue = _rootValue;
            _generatedValueIsCached = true;
            if (rootIsNew)
                _rootValue = null;
            return _generatedValue;
        }

        /// <exception cref="ArgumentNullException"><c>instance</c> is null.</exception>
        public void Apply(object instance)
        {
            if (instance == null) throw new ArgumentNullException("instance");

            _objectBeingAppliedTo = instance;
            ProcessAllProperties();
            _objectBeingAppliedTo = null;
        }

        void ProcessAllProperties()
        {
            foreach (var propertyAssignment in BuildPropertyAssignments())
            {
                IMemberBuilder currentBuilder = this;
                foreach (var traversedAccessor in propertyAssignment.CallStack)
                {
                    // ignore the root
                    if (traversedAccessor.Equals(Type))
                        continue;

                    IPropertyBuilder currentPropertyBuilder = null;

                    // we have an intermediary property to create alongst the chain path
                    if (!traversedAccessor.Equals(propertyAssignment.Builder.Property))
                    {
                        var property = (IProperty) traversedAccessor;
                        try
                        {
                            if (currentBuilder.Value != null)
                            {
                                object existingValue = property.GetValue(currentBuilder.Value);
                                currentPropertyBuilder = GetRealPropertyInstance(traversedAccessor, existingValue);
                            }
                        }
                        catch
                        {
                        }
                        if (currentPropertyBuilder == null)
                            currentPropertyBuilder = GetRealPropertyInstance(traversedAccessor);
                    }
                    else
                    {
                        // we have the last property in the chain to create
                        currentPropertyBuilder = GetRealPropertyInstance(
                            propertyAssignment.Builder.Property,
                            propertyAssignment.Builder.Value);
                    }

                    if (currentPropertyBuilder.Owner == null)
                        currentPropertyBuilder.SetOwner(currentBuilder);
                    currentBuilder = currentPropertyBuilder;
                }
            }
        }

        IPropertyBuilder GetRealPropertyInstance(IMember accessor)
        {
            return GetRealPropertyInstance(accessor, null);
        }

        IPropertyBuilder GetRealPropertyInstance(IMember accessor, object value)
        {
            IPropertyBuilder pi;
            if (_propertyInstancesCache.TryGetValue(accessor, out pi))
            {
                if (value != null)
                    pi.TrySetValue(value);
            }
            else
            {
                var propertyAccessor = (IProperty) accessor;
                pi = propertyAccessor.CreateBuilder();
                if (value != null)
                    pi.TrySetValue(value);
                _propertyInstancesCache.Add(accessor, pi);
            }
            return pi;
        }

        void EnsureWriteable()
        {
            if (!_generatedValueIsCached) return;

            _propertyInstancesCache.Clear();
            _generatedValueIsCached = false;
        }

        IList<PropertyAssignment> BuildPropertyAssignments()
        {
            var props = new List<PropertyAssignment>();

            foreach (PropertyBuilder property in Changes.Values)
            {
                var assignment = new PropertyAssignment {Builder = property};
                var callStack = new List<IMember>(property.Property.GetCallStack());
                callStack.Reverse();
                assignment.CallStack = callStack.AsReadOnly();
                props.Add(assignment);
            }

            // we sort so that the smaller callstacks get called first.
            props.Sort();
            return props.AsReadOnly();
        }

        struct PropertyAssignment : IComparable<PropertyAssignment>
        {
            public IList<IMember> CallStack;
            public IPropertyBuilder Builder;

            public int CompareTo(PropertyAssignment other)
            {
                int stackComparison = CallStack.Count.CompareTo(other.CallStack.Count);
                return stackComparison == 0
                           ? Builder.IndexAtCreation.CompareTo(other.Builder.IndexAtCreation)
                           : stackComparison;
            }
        }

        class PropertyDictionary : IDictionary<string, IPropertyBuilder>
        {
            public PropertyDictionary(TypeBuilder owner)
            {
                Owner = owner;
            }

            public TypeBuilder Owner { get; private set; }

            public bool ContainsKey(string key)
            {
                return Owner._propertiesCache.ContainsKey(key) && Owner._propertiesCache[key] != null &&
                       Owner._propertiesCache[key].HasValue;
            }

            public void Add(string key, IPropertyBuilder value)
            {
                throw new NotSupportedException();
            }

            public bool Remove(string key)
            {
                throw new NotSupportedException();
            }

            public bool TryGetValue(string key, out IPropertyBuilder value)
            {
                if (ContainsKey(key))
                {
                    value = Owner._propertiesCache[key];
                    return true;
                }
                value = null;
                return false;
            }

            public IPropertyBuilder this[string key]
            {
                get
                {
                    IPropertyBuilder property;
                    if (!TryGetValue(key,out property))
                        throw new ArgumentOutOfRangeException();
                    return property;
                }
                set { throw new NotSupportedException(); }
            }

            public ICollection<string> Keys
            {
                get { return TheOnesWithValues().Select(kv => kv.Key).ToList(); }
            }

            public ICollection<IPropertyBuilder> Values
            {
                get { return TheOnesWithValues().Select(kv => kv.Value).ToList(); }
            }

            public IEnumerator<KeyValuePair<string, IPropertyBuilder>> GetEnumerator()
            {
                foreach (var value in TheOnesWithValues())
                    yield return value;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
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

            public int Count
            {
                get { return TheOnesWithValues().Count(); }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            IEnumerable<KeyValuePair<string, IPropertyBuilder>> TheOnesWithValues()
            {
                return Owner._propertiesCache.Where(kv => kv.Value != null && kv.Value.HasValue);
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