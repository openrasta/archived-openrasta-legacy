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
using System.Text;

namespace OpenRasta.TypeSystem.ReflectionBased.Surrogates
{
    public class DateTimeSurrogate : ISurrogate<DateTime>
    {
        private DateTime _value = DateTime.MinValue;
        public DateTime Value
        {
            get { return _value; }
            set { _value = value; }
        }
        public int Day
        {
            get { return Value.Day; }
            set { Value = new DateTime(Value.Year, Value.Month, value, Value.Hour, Value.Minute, Value.Second, Value.Millisecond, Value.Kind); }
        }
        public int Month
        {
            get { return Value.Month; }
            set { Value = new DateTime(Value.Year, value, Value.Day, Value.Hour, Value.Minute, Value.Second, Value.Millisecond, Value.Kind); }
        }
        public int Year
        {
            get { return Value.Year; }
            set { Value = new DateTime(value, Value.Month, Value.Day, Value.Hour, Value.Minute, Value.Second, Value.Millisecond, Value.Kind); }
        }
        public int Hour
        {
            get { return Value.Hour; }
            set { Value = new DateTime(Value.Year, Value.Month, Value.Day, value, Value.Minute, Value.Second, Value.Millisecond, Value.Kind); }
        }
        public int Minute
        {
            get { return Value.Minute; }
            set { Value = new DateTime(Value.Year, Value.Month, Value.Day, Value.Hour,value, Value.Second, Value.Millisecond, Value.Kind); }
        }
        public int Second
        {
            get { return Value.Second; }
            set { Value = new DateTime(Value.Year, Value.Month, Value.Day, Value.Hour, Value.Minute, value, Value.Millisecond, Value.Kind); }
        }
        public int Millisecond
        {
            get { return Value.Millisecond; }
            set { Value = new DateTime(Value.Year, Value.Month, Value.Day, Value.Hour, Value.Minute, Value.Second, value, Value.Kind); }        
        }

        object ISurrogate.Value
        {
            get
            {
                return Value;
            }
            set
            {
                Value = (DateTime)value;
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
