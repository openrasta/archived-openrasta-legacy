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

namespace OpenRasta.Binding
{
    /// <summary>
    /// Represents a component able to build instances of objects from object paths and values.
    /// </summary>
    public interface IObjectBinder
    {
        /// <summary>
        /// Gets a value defining if the binder contains any values.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Gets the list of prefixes that the binder will ignore when parsing the keys and value pairs.
        /// </summary>
        ICollection<string> Prefixes { get; }

        /// <summary>
        /// Tries to set a property value based on a key and a list of values.
        /// </summary>
        /// <typeparam name="TValue">The type of values being used to assign to the destination property.</typeparam>
        /// <param name="key">The object path to the property to assign.</param>
        /// <param name="values">The values to be used in realizing the value for the property</param>
        /// <param name="converter">The converter responsible for converting between the source values and the destination value.</param>
        /// <returns><c>true</c> if the assignment was successful, otherwise <c>false</c>.</returns>
        bool SetProperty<TValue>(string key, IEnumerable<TValue> values, ValueConverter<TValue> converter);

        /// <summary>
        /// Tries to set an instance as the object used when assigning values.
        /// </summary>
        /// <param name="builtInstance">The instance of an object being used for binding.</param>
        /// <returns><c>true</c> if the assignment was successful, otherwise <c>false</c>.</returns>
        bool SetInstance(object builtInstance);

        /// <summary>
        /// Attempts to build an object and return a <see cref="BindingResult"/> instance containing the result of the building.
        /// </summary>
        /// <returns>An instance of the <see cref="BindingResult"/> type containing the result of the binding.</returns>
        BindingResult BuildObject();
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