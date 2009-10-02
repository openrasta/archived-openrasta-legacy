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

namespace OpenRasta.Collections
{
    public class DictionaryBase<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary
    {
        readonly Dictionary<TKey, TValue> _baseDictionary;

        public DictionaryBase()
        {
            _baseDictionary = new Dictionary<TKey, TValue>();
        }

        public DictionaryBase(IEqualityComparer<TKey> comparer)
        {
            _baseDictionary = new Dictionary<TKey, TValue>(comparer);
        }

        public int Count
        {
            get { return _baseDictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((IDictionary)_baseDictionary).IsReadOnly; }
        }

        public ICollection<TKey> Keys
        {
            get { return _baseDictionary.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return _baseDictionary.Values; }
        }

        protected IEqualityComparer<TKey> Comparer
        {
            get { return _baseDictionary.Comparer; }
        }

        bool IDictionary.IsFixedSize
        {
            get { return ((IDictionary)_baseDictionary).IsFixedSize; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<TKey, TValue>>)_baseDictionary).IsReadOnly; }
        }

        bool ICollection.IsSynchronized
        {
            get { return ((ICollection)_baseDictionary).IsSynchronized; }
        }

        ICollection IDictionary.Keys
        {
            get { return ((IDictionary)_baseDictionary).Keys; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection)_baseDictionary).SyncRoot; }
        }

        ICollection IDictionary.Values
        {
            get { return ((IDictionary)_baseDictionary).Values; }
        }

        public virtual TValue this[TKey key]
        {
            get { return _baseDictionary[key]; }
            set { _baseDictionary[key] = value; }
        }

        object IDictionary.this[object key]
        {
            get { return this[(TKey)key]; }
            set { this[(TKey)key] = (TValue)value; }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_baseDictionary).CopyTo(array, index);
        }

        public virtual void Clear()
        {
            _baseDictionary.Clear();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_baseDictionary).Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_baseDictionary).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (_baseDictionary.ContainsKey(item.Key) &&
                (ReferenceEquals(item.Value,_baseDictionary[item.Key]) ||
                    item.Value.Equals(_baseDictionary[item.Key])))
                return Remove(item.Key);
            return false;
        }

        void IDictionary.Add(object key, object value)
        {
            Add((TKey)key, (TValue)value);
        }

        bool IDictionary.Contains(object key)
        {
            return ((IDictionary)_baseDictionary).Contains(key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)_baseDictionary).GetEnumerator();
        }

        void IDictionary.Remove(object key)
        {
            Remove((TKey)key);
        }

        public virtual void Add(TKey key, TValue value)
        {
            _baseDictionary.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return _baseDictionary.ContainsKey(key);
        }

        public virtual bool Remove(TKey key)
        {
            return _baseDictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _baseDictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_baseDictionary).GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<TKey, TValue>>)_baseDictionary).GetEnumerator();
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