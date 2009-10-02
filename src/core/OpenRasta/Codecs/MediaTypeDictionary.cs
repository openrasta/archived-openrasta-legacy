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
using OpenRasta.Web;

namespace OpenRasta.Codecs
{
    public class MediaTypeDictionary<TValue> : IEnumerable<TValue>
    {
        readonly Dictionary<string, IList<TValue>> _store = new Dictionary<string, IList<TValue>>();
        readonly Dictionary<string, List<TValue>> _subwildcard = new Dictionary<string, List<TValue>>();
        readonly List<TValue> _wildcard = new List<TValue>();

        public void Add(MediaType mediaType, TValue value)
        {
            if (mediaType == null)
                throw new ArgumentNullException("mediaType", "mediaType is null.");

            if (mediaType.IsWildCard)
                _wildcard.Add(value);
            else if (mediaType.IsSubtypeWildcard)
                GetSubtypeWildcardRegistration(mediaType.TopLevelMediaType).Add(value);
            else
            {
                AddIfNotPresent(GetForMediaType(mediaType), value);
                AddIfNotPresent(GetForSubTypeWildcard(mediaType), value);
                AddIfNotPresent(GetForWildcard(), value);
            }
        }

        public void Clear()
        {
            _store.Clear();
            _wildcard.Clear();
            _subwildcard.Clear();
        }

        public IEnumerable<TValue> Matching(MediaType mediaType)
        {
            // match the cache if a key already exists
            foreach (var item in GetForMediaType(mediaType))
                yield return item;

            // try to match subtype
            if (!mediaType.IsTopLevelWildcard && _subwildcard.ContainsKey(mediaType.TopLevelMediaType))
                foreach (var item in _subwildcard[mediaType.TopLevelMediaType])
                    yield return item;

            foreach (var item in _wildcard)
                yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (var value in _store.SelectMany(key => key.Value))
                yield return value;
        }

        void AddIfNotPresent(IList<TValue> list, TValue value)
        {
            if (!list.Contains(value))
                list.Add(value);
        }

        IList<TValue> GetForMediaType(MediaType mediaType)
        {
            return GetOrCreate(mediaType.MediaType);
        }

        IList<TValue> GetForSubTypeWildcard(MediaType mediaType)
        {
            return GetOrCreate(mediaType.TopLevelMediaType + "/*");
        }

        IList<TValue> GetForWildcard()
        {
            return GetOrCreate("*/*");
        }

        IList<TValue> GetOrCreate(string key)
        {
            IList<TValue> value;
            if (!_store.TryGetValue(key, out value))
                _store[key] = value = new List<TValue>();
            return value;
        }

        List<TValue> GetSubtypeWildcardRegistration(string topLevelMediaType)
        {
            if (!_subwildcard.ContainsKey(topLevelMediaType))
                _subwildcard[topLevelMediaType] = new List<TValue>();
            return _subwildcard[topLevelMediaType];
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