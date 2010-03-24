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
using OpenRasta.Collections;


namespace OpenRasta.Web.Markup.Attributes
{
    public class XhtmlAttributeCollection : IAttributeCollection
    {
        readonly NullBehaviorDictionary<string, IAttribute> _attributes = new NullBehaviorDictionary<string, IAttribute>();

        public IDictionary<string, Func<IAttribute>> AllowedAttributes { get; set; }

        public int Count
        {
            get { return _attributes.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public IAttribute this[string key]
        {
            get
            {
                if (_attributes[key] == null && AllowedAttributes != null)
                {
                    if (AllowedAttributes.ContainsKey(key))
                        _attributes[key] = AllowedAttributes[key]();
                    else
                        _attributes[key] = new PrimaryTypeAttributeNode<string>(key);
                }
                return _attributes[key];
            }
            set { _attributes[key] = value; }
        }

        IAttribute IList<IAttribute>.this[int index]
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public string GetAttribute(string attributeName)
        {
            return GetAttribute<string>(attributeName);
        }

        public T GetAttribute<T>(string attributeName)
        {
            IAttribute attrib;
            if (!_attributes.TryGetValue(attributeName, out attrib))
                _attributes.Add(attributeName, attrib = CreateAttribute<T>(attributeName));
            return ((IAttribute<T>)attrib).Value;
        }

        public void SetAttribute<T>(string attributeName, T value)
        {
            IAttribute attrib;
            if (!_attributes.TryGetValue(attributeName, out attrib))
                _attributes.Add(attributeName, attrib = CreateAttribute<T>(attributeName));
            ((IAttribute<T>)attrib).Value = value;
        }

        public void Add(IAttribute item)
        {
            _attributes.Add(item.Name, item);
        }

        public void Clear()
        {
            _attributes.Clear();
        }

        public bool Contains(IAttribute item)
        {
            return _attributes.ContainsKey(item.Name);
        }

        public void CopyTo(IAttribute[] array, int arrayIndex)
        {
            _attributes.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(IAttribute item)
        {
            return _attributes.Remove(item.Name);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<IAttribute> GetEnumerator()
        {
            return _attributes.Values.GetEnumerator();
        }

        public int IndexOf(IAttribute item)
        {
            throw new NotSupportedException();
        }

        public void Insert(int index, IAttribute item)
        {
            _attributes.Add(item.Name, item);
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        IAttribute<T> CreateAttribute<T>(string name)
        {
            Type attribType = typeof(T);
            if (AllowedAttributes != null && AllowedAttributes.ContainsKey(name))
                return (IAttribute<T>)AllowedAttributes[name]();
            //if (!dynamicAttributesPermitted)
            //    throw new ArgumentOutOfRangeException("name", "Attribute {0} is not allowed on this element.".With(name));

            if (attribType.IsValueType
                || attribType == typeof(string)
                || typeof(Nullable).IsAssignableFrom(attribType))
                return new PrimaryTypeAttributeNode<T>(name);
            if (attribType == typeof(MediaType))
                return (IAttribute<T>)new XhtmlAttributeNode<MediaType>(name, false, media => media.ToString(), str => new MediaType(str));
            if (attribType == typeof(IList<Uri>))
                return (IAttribute<T>)new CharacterSeparatedAttributeNode<Uri>(name, " ", uri => uri.ToString(), s => new Uri(s, UriKind.Absolute));
            if (attribType == typeof(IList<MediaType>))
                return (IAttribute<T>)new CharacterSeparatedAttributeNode<MediaType>(name, " ", mediatype => mediatype.ToString(), str => new MediaType(str));
            if (attribType == typeof(IList<string>))
                return (IAttribute<T>)new CharacterSeparatedAttributeNode<string>(name, " ", i => i, i => i);
            throw new InvalidOperationException("Could not automatically create attribute of type " + typeof(T));
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