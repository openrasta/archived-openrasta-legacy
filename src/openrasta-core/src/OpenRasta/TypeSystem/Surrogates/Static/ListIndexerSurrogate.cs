#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion
// ReSharper disable UnusedMember.Global
using System;
using System.Collections;
using System.Collections.Generic;
using OpenRasta.TypeSystem.ReflectionBased;

namespace OpenRasta.TypeSystem.Surrogates.Static
{
    /// <summary>
    /// Provides a surrogate for types implementing <see cref="IList" />(Of T)
    /// </summary>
    public class ListIndexerSurrogate<T> : ISurrogate
    {
        readonly Dictionary<int, int> _binderIndexToRealIndex = new Dictionary<int, int>();
        IList<T> _value;

        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value.GetType().InheritsFrom(typeof(ListIndexerSurrogate<>)))
                    _value = new List<T>();
                else if (value is IList<T>)
                    _value = (IList<T>)value;
                else
                    throw new ArgumentException();
            }
        }

        public T this[int index]
        {
            get
            {
                if (_binderIndexToRealIndex.ContainsKey(index))
                {
                    int realIndex = _binderIndexToRealIndex[index];
                    return _value[realIndex];
                }

                return default(T);
            }

            set
            {
                if (_binderIndexToRealIndex.ContainsKey(index))
                {
                    _value[_binderIndexToRealIndex[index]] = value;
                }
                else
                {
                    _value.Add(value);
                    int realIndex = _value.IndexOf(value);
                    _binderIndexToRealIndex[index] = realIndex;
                }
            }
        }
    }
}

// ReSharper restore UnusedMember.Global
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