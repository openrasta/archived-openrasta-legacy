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
using System.Collections.ObjectModel;
using System.Text;
using OpenRasta.Collections;
using OpenRasta.Web.Markup.Attributes;

namespace OpenRasta.Web.Markup.Attributes
{
    public class CharacterSeparatedAttributeNode<T> : XhtmlAttributeNode<IList<T>>
    {
        readonly Func<T, string> _write;
        readonly Func<string, T> _read;
        public string SeparatorCharacter { get; set; }

        public CharacterSeparatedAttributeNode(string name, string separator, Func<T,string> write, Func<string,T> read)
            : base(name, false)
        {
            _write = write;
            _read = read;
            Writer = Write;
            Reader = Read;
            Value = new List<T>();
            SeparatorCharacter = separator;
        }

        string[] Split(string value)
        {
            return value.Split(new[]{SeparatorCharacter},StringSplitOptions.RemoveEmptyEntries);
        }

        IList<T> Read(string value)
        {
            string[] entries = Split(value);
            Value.Clear();
            foreach(var entry in entries)
                Value.Add(_read(entry));
            return Value;
        }

        string Write(IList<T> values)
        {
            var sb = new StringBuilder();
            foreach(var val in values)
            {
                if (sb.Length > 0) sb.Append(SeparatorCharacter);
                sb.Append(_write(val));
            }
            return sb.ToString();
        }

        public override bool IsDefault
        {
            get
            {
                return Value.Count == 0;
            }
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
