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
using OpenRasta.TypeSystem;

namespace OpenRasta.Binding
{
    public class KeyedValuesBinder : IObjectBinder
    {
        readonly bool _isEnumerable;
        readonly string _name;
        readonly string _typeName;

        object _cachedBuiltObject;
        bool _isInstanceConstructed;

        public KeyedValuesBinder(IType target) : this(target, target.Name)
        {
        }

        public KeyedValuesBinder(IType target, string name)
        {
            _isEnumerable = !target.Equals<string>() && target.Type.IsEnumerable;
            Builder = target.CreateBuilder();
            _name = name;
            _typeName = target.TypeName;

            Prefixes = new List<string> { _name, _typeName };
            PathManager = new PathManager();
        }

        public bool IsEmpty
        {
            get { return !Builder.HasValue; }
        }

        public ICollection<string> Prefixes { get; private set; }

        protected ITypeBuilder Builder { get; private set; }
        protected IPathManager PathManager { get; set; }

        public virtual BindingResult BuildObject()
        {
            if (IsEmpty && !_isEnumerable)
                return BindingResult.Failure();
            if (_isInstanceConstructed)
                return BindingResult.Success(_cachedBuiltObject);

            _cachedBuiltObject = Builder.Create();

            _isInstanceConstructed = true;
            return BindingResult.Success(_cachedBuiltObject);
        }

        public virtual bool SetInstance(object builtInstance)
        {
            if (Builder.Value != null)
                throw new InvalidOperationException("An instance was already set by passing a constructor key.");
            _isInstanceConstructed = false;

            return Builder.TrySetValue(builtInstance);
        }

        public bool SetProperty<TValue>(string key, IEnumerable<TValue> values, ValueConverter<TValue> converter)
        {
            _isInstanceConstructed = false;
            var keyType = PathManager.GetPathType(Prefixes, key);
            bool success;

            if (keyType.Type == PathComponentType.Constructor)
                success = SetConstructorValue(values, converter);
            else
                success = SetPropertyValue(key, keyType.ParsedValue, values, converter);

            if (!success)
                success = SetPropertyValue(key, key, values, converter);
            return success;
        }

        bool SetConstructorValue<TValue>(IEnumerable<TValue> values, ValueConverter<TValue> converter)
        {
            return Builder.TrySetValue(values, converter);
        }

        bool SetPropertyValue<TValue>(string key, string property, IEnumerable<TValue> values, ValueConverter<TValue> converter)
        {
            var propertyBuilder = Builder.GetProperty(property ?? key);
            if (propertyBuilder == null) return false;
            return propertyBuilder.TrySetValue(values, converter);
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