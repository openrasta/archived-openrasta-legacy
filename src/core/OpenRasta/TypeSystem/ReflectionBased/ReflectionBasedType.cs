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
using System.Diagnostics;
using OpenRasta.Binding;
using OpenRasta.DI;

namespace OpenRasta.TypeSystem.ReflectionBased
{
    /// <summary>
    /// Represents a CLR-based type.
    /// </summary>
    [DebuggerDisplay("Name={TargetType.Name}, FullName={TargetType.ToString()}")]
    public class ReflectionBasedType : ReflectionBasedMember<ITypeBuilder>, IResolverAwareType
    {
        public ReflectionBasedType(ITypeSystem typeSystem, Type type)
            : base(typeSystem, type)
        {
        }

        public override IType Type
        {
            get { return this; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ReflectionBasedType))
                return false;
            return TargetType.Equals(((ReflectionBasedType)obj).TargetType);
        }

        public override int GetHashCode()
        {
            return TargetType.GetHashCode();
        }

        public int CompareTo(IType other)
        {
            if (other == null || other.StaticType == null)
                return -1;
            return TargetType.GetInheritanceDistance(other.StaticType);
        }

        public object CreateInstance(IDependencyResolver resolver)
        {
            if (resolver == null) throw new ArgumentNullException("resolver");
            return resolver.Resolve(TargetType, UnregisteredAction.AddAsTransient);
        }

        public ITypeBuilder CreateBuilder()
        {
            return new TypeBuilder(this);
        }

        public virtual object CreateInstance()
        {
            return TargetType.CreateInstance();
        }

        public virtual object CreateInstance(params object[] arguments)
        {
            return TargetType.CreateInstance(arguments);
        }

        public bool IsAssignableFrom(IType member)
        {
            return member != null && member.CompareTo(this) >= 0;
        }

        public bool TryCreateInstance<T>(IEnumerable<T> values, ValueConverter<T> converter, out object result)
        {
            result = null;
            try
            {
                result = TargetType.CreateInstanceFrom(values, converter);
            }
            catch (NotSupportedException)
            {
                return false;
            }
            catch (InvalidCastException)
            {
                return false;
            }

            return result != null;
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