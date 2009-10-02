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
using OpenRasta.Binding;

namespace OpenRasta.TypeSystem
{
    /// <summary>
    /// Represents an instance of a method parameter.
    /// </summary>
    public class ParameterInstance : MemberBuilder
    {
        readonly ReflectionParameter _parameter;
        bool _hasBeenSet;
        object _value;
        public ParameterInstance(ReflectionParameter parameter, IObjectBinder binder)
            : base(parameter)
        {
            _parameter = parameter;

            Binder = binder;
        }

        public bool IsReadyForAssignment { get { return HasAssignedValue || HasBinderValue || Parameter.IsOptional; } }

        /// <summary>
        /// The parameter has been assigned a value through its Value property.
        /// </summary>
        public bool HasAssignedValue
        {
            get
            {
                return _hasBeenSet || ((Parameter.TargetType.IsEnum || Parameter.TargetType.IsPrimitive) && HasBinderValue);
            }
        }
        public bool HasBinderValue { get { return !Binder.IsEmpty; } }
        public bool HasDefaultValue { get { return Parameter.DefaultValue == DBNull.Value; } }

        public override object Value
        {
            get { return _hasBeenSet ? _value : (!Binder.IsEmpty ? Binder.BuildObject().Instance : Parameter.DefaultValue); }
        }

        public override bool HasValue
        {
            get { return HasAssignedValue || HasBinderValue; }
        }

        public override bool TrySetValue(object value)
        {

            _value = value;
            _hasBeenSet = true;
            return true;
        }

        public override bool TrySetValue<T>(IEnumerable<T> values, ValueConverter<T> converter)
        {
            throw new InvalidOperationException();
        }

        public IObjectBinder Binder { get; private set; }

        public ReflectionParameter Parameter
        {
            get { return _parameter; }
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