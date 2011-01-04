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
using System.Collections.Specialized;
using System.Reflection;
using System.Text;

namespace OpenRasta.Collections
{
    namespace Specialized
    {
        public static class SpecializedCollectionExtensions
        {
            public static void AddOrReplace<TKey, TValue>(this IDictionary<TKey, TValue> target, 
                                                          IDictionary<TKey, TValue> values)
            {
                foreach (var key in values.Keys)
                {
                    if (target.ContainsKey(key))
                        target[key] = values[key];
                    else
                        target.Add(key, values[key]);
                }
            }

            public static void AddReplace(this NameValueCollection baseCollection, NameValueCollection collection)
            {
                foreach (string key in collection.AllKeys)
                    baseCollection[key] = collection[key];
            }

            public static IDictionary<string, string> ToCaseInvariantDictionary(this object objectToConvert)
            {
                return ToDictionary(objectToConvert, StringComparer.OrdinalIgnoreCase);
            }

            public static IDictionary<string, string> ToDictionary(this object objectToConvert)
            {
                return ToDictionary(objectToConvert, StringComparer.CurrentCulture);
            }

            public static IDictionary<string, string> ToDictionary(this object objectToConvert, 
                                                                   IEqualityComparer<string> comparer)
            {
                if (objectToConvert is IDictionary<string, string>)
                    return objectToConvert as IDictionary<string, string>;

                var dic = new Dictionary<string, string>(comparer);
                foreach (var value in GetValues(objectToConvert))
                    dic.Add(value.Key, value.Value == null ? null : value.Value.ToString());
                return dic;
            }

            public static NameValueCollection ToNameValueCollection(this object objectToConvert)
            {
                if (objectToConvert == null)
                    throw new ArgumentNullException("objectToConvert");
                if (objectToConvert is NameValueCollection)
                    return (NameValueCollection)objectToConvert;
                var values = new NameValueCollection();
                foreach (var value in GetValues(objectToConvert))
                    values.Add(value.Key, value.Value != null ? value.Value.ToString() : null);
                return values;
            }

            public static IDictionary<string, object> ToProperties(this object objectToConvert)
            {
                if (objectToConvert is IDictionary<string, object>)
                    return objectToConvert as IDictionary<string, object>;
                var dic = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                foreach (var value in GetValues(objectToConvert))
                    dic.Add(value.Key, value.Value);
                return dic;
            }

            static IEnumerable<KeyValuePair<string, object>> GetValues(object obj)
            {
                var objType = obj.GetType();
                foreach (var pi in objType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (pi.GetIndexParameters().Length == 0)
                        yield return new KeyValuePair<string, object>(pi.Name, pi.GetValue(obj, null));
                }
            }
        }
    }

    public static class CollectionExtensions
    {
        public static void AddRange<T>(this IList<T> source, IEnumerable<T> newItems)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            foreach (var item in newItems)
                source.Add(item);
        }

        public static NameValueCollection With(this NameValueCollection collection, string name, string value)
        {
            collection.Add(name, value);
            return collection;
        }

        public static void RemoveMatching<T>(this IList<T> list, Predicate<T> predicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                {
                    list.RemoveAt(i);
                    i--;
                }
            }
        }

        public static string ToHtmlFormEncoding(this NameValueCollection collection)
        {
            if (collection == null)
                return string.Empty;
            var sb = new StringBuilder();
            foreach (var key in collection.Keys)
                sb.Append(key).Append("=").Append(collection[key.ToString()]).Append(";");
            return sb.ToString();
        }

#if !__OW_PROFILE_sl30__ && !__OW_PROFILE_sl40__

        public static string Find(this NameValueCollection collectionToSearch, 
                                  string name, 
                                  StringComparison comparisonType)
        {
            for (int i = 0; i < collectionToSearch.Count; i++)
            {
                if (string.Compare(collectionToSearch.GetKey(i), name, comparisonType) == 0)
                    return collectionToSearch[i];
            }
            return null;
        }
#endif

        public static IDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> objectToWrap)
        {
            return new ReadOnlyDictionaryWrapper<TKey, TValue>(objectToWrap);
        }

        public static IDictionary<string, string[]> ToDictionary(this NameValueCollection collection)
        {
            var target = new Dictionary<string, string[]>();
            foreach (string key in collection.AllKeys)
                target.Add(key, collection.GetValues(key));
            return target;
        }

        public static IEnumerable<TValue> StartingWith<TValue>(IDictionary<string, TValue> dic, 
                                                               string prefix, 
                                                               StringComparison comparison)
        {
            foreach (var kv in dic)
            {
                if (kv.Key.StartsWith(prefix, comparison))
                    yield return kv.Value;
            }
        }

        class ReadOnlyDictionaryWrapper<TKey, TValue> : IDictionary<TKey, TValue>
        {
            readonly IDictionary<TKey, TValue> _wrapped;

            public ReadOnlyDictionaryWrapper(IDictionary<TKey, TValue> wrappedClass)
            {
                _wrapped = wrappedClass;
            }

            public int Count
            {
                get { return _wrapped.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public ICollection<TKey> Keys
            {
                get { return _wrapped.Keys; }
            }

            public ICollection<TValue> Values
            {
                get { return _wrapped.Values; }
            }

            public TValue this[TKey key]
            {
                get { return _wrapped[key]; }
                set { ErrorIsReadOnly(); }
            }

            public void Add(KeyValuePair<TKey, TValue> item)
            {
                ErrorIsReadOnly();
            }

            public void Clear()
            {
                ErrorIsReadOnly();
            }

            public bool Contains(KeyValuePair<TKey, TValue> item)
            {
                return _wrapped.Contains(item);
            }

            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            {
                _wrapped.CopyTo(array, arrayIndex);
            }

            public bool Remove(KeyValuePair<TKey, TValue> item)
            {
                return false;
            }

            public void Add(TKey key, TValue value)
            {
                ErrorIsReadOnly();
            }

            public bool ContainsKey(TKey key)
            {
                return _wrapped.ContainsKey(key);
            }

            public bool Remove(TKey key)
            {
                return false;
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                return _wrapped.TryGetValue(key, out value);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)_wrapped).GetEnumerator();
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                return _wrapped.GetEnumerator();
            }

            void ErrorIsReadOnly()
            {
                throw new InvalidOperationException("The dictionary is read-only");
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