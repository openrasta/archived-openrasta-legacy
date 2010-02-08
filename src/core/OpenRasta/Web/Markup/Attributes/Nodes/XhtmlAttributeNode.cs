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

namespace OpenRasta.Web.Markup.Attributes
{
    public class XhtmlAttributeNode<T> : IAttribute<T>
    {
        protected Func<string, T> Reader;
        protected Func<T, string> Writer;
        T _value;
        bool _valueHasBeenSet;

        public XhtmlAttributeNode(string name, bool renderWhenDefault) : this(name, renderWhenDefault, null, null)
        {
        }

        public XhtmlAttributeNode(string name, bool renderWhenDefault, Func<T, string> write, Func<string, T> read)
        {
            Name = name;
            RendersOnDefaultValue = renderWhenDefault;
            Writer = write;
            Reader = read;
        }

        public string DefaultValue { get; set; }

        public virtual bool IsDefault
        {
            get
            {
                if (DefaultValue != null)
                {
                    return DefaultValue.Equals(Writer(Value));
                }

                return Writer(Value) == null;
            }
        }

        public string Name { get; set; }
        public bool RendersOnDefaultValue { get; set; }

        public string SerializedValue
        {
            get { return _valueHasBeenSet ? Writer(Value) : DefaultValue; }
            set
            {
                _valueHasBeenSet = true;
                Value = Reader(value);
            }
        }

        public T Value
        {
            get { return (!_valueHasBeenSet && DefaultValue != null) ? Reader(DefaultValue) : _value; }
            set
            {
                _value = value;
                _valueHasBeenSet = true;
            }
        }

        public override string ToString()
        {
            return SerializedValue;
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