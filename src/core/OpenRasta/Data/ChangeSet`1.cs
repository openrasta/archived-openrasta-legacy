#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion

using System.Collections.Generic;
using OpenRasta.Binding;
using OpenRasta.TypeSystem;

namespace OpenRasta.Data
{
    /// <summary>
    /// Represents a set of changes that can be applied to a type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ChangeSet<T> where T : class
    {
        public ChangeSet(ITypeBuilder typeBuilder)
        {
            TypeBuilder = typeBuilder;
        }

        /// <summary>
        /// Gets the list of changes to be applied to an object.
        /// </summary>
        public IDictionary<string, IPropertyBuilder> Changes
        {
            get { return TypeBuilder.Changes; }
        }

        public ITypeBuilder TypeBuilder { get; private set; }

        /// <summary>
        /// Gets the binder used to build the changeset.
        /// </summary>
        /// <param name="typeSystem"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public static IObjectBinder GetBinder(ITypeSystem typeSystem, IMember member)
        {
            var innerMember = typeSystem.FromClr<T>();
            return new ChangeSetBinder<T>(innerMember.Type, member.Name);
        }

        /// <summary>
        /// Applies the changes in this <see cref="ChangeSet{T}"/> to the provided instance.
        /// </summary>
        /// <param name="testObject">The instance of an object on which to apply the changes.</param>
        public void Apply(T testObject)
        {
            TypeBuilder.Update(testObject);
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
