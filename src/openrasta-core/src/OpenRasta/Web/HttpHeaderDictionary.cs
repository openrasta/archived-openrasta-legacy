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
using System.Globalization;

namespace OpenRasta.Web
{
    /// <summary>
    /// Provides a list of http headers. In dire need of refactoring to use specific header types similar to http digest.
    /// </summary>
    public class HttpHeaderDictionary : IDictionary<string, string>
    {
        readonly IDictionary<string, string> _base = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        ContentDispositionHeader _contentDisposition;
        long? _contentLength;
        MediaType _contentType;
        string HDR_CONTENT_DISPOSITION = "Content-Disposition";
        string HDR_CONTENT_LENGTH = "Content-Length";
        string HDR_CONTENT_TYPE = "Content-Type";

        public HttpHeaderDictionary() { }

        public HttpHeaderDictionary(NameValueCollection sourceDictionary)
        {
            foreach (string key in sourceDictionary.Keys)
                this[key] = sourceDictionary[key];
        }

        public MediaType ContentType { get { return _contentType; } set { SetValue(ref _contentType, HDR_CONTENT_TYPE, value); } }
        public long? ContentLength { get { return _contentLength; } set { SetValue(ref _contentLength, HDR_CONTENT_LENGTH, value); } }
        public ContentDispositionHeader ContentDisposition { get { return _contentDisposition; } set { SetValue(ref _contentDisposition, HDR_CONTENT_DISPOSITION, value); } }

        public void Add(string key, string value)
        {
            _base.Add(key, value);
            UpdateValue(key, value);
        }

        public bool Remove(string key)
        {
            bool result = _base.Remove(key);
            UpdateValue(key, null);
            return result;
        }

        public string this[string key]
        {
            get
            {
                string result;
                if (_base.TryGetValue(key, out result))
                    return result;
                return null;
            }
            set
            {
                _base[key] = value;
                UpdateValue(key, value);
            }
        }

        public bool ContainsKey(string key) { return _base.ContainsKey(key); }

        public ICollection<string> Keys { get { return _base.Keys; } }

        public bool TryGetValue(string key, out string value) { return _base.TryGetValue(key, out value); }

        public ICollection<string> Values { get { return _base.Values; } }

        public void Add(KeyValuePair<string, string> item) { _base.Add(item.Key, item.Value); }

        public void Clear() { _base.Clear(); }

        public bool Contains(KeyValuePair<string, string> item) { return _base.ContainsKey(item.Key); }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) { (_base).CopyTo(array, arrayIndex); }

        public int Count { get { return _base.Count; } }

        public bool IsReadOnly { get { return false; } }

        public bool Remove(KeyValuePair<string, string> item) { return Remove(item.Key); }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() { return _base.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        void UpdateValue(string headerName, string value)
        {
            if (headerName.Equals(HDR_CONTENT_TYPE, StringComparison.OrdinalIgnoreCase))
                _contentType = new MediaType(value);
            else if (headerName.Equals(HDR_CONTENT_LENGTH, StringComparison.OrdinalIgnoreCase))
            {
                long contentLength;
                if (long.TryParse(value,NumberStyles.Float,CultureInfo.InvariantCulture, out contentLength))
                    _contentLength = contentLength;
            }
            else if (headerName.Equals(HDR_CONTENT_DISPOSITION, StringComparison.OrdinalIgnoreCase))
            {
                _contentDisposition = new ContentDispositionHeader(value);
            }
        }

        void SetValue<T>(ref T typedKey, string key, T value)
        {
            typedKey = value;
            _base[key] = value == null ? null : value.ToString();
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