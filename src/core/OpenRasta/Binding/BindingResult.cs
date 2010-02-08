#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion

namespace OpenRasta.Binding
{
    /// <summary>
    /// Represents the result of a binding.
    /// </summary>
    public class BindingResult
    {
        BindingResult(bool successfull, object instance)
        {
            Successful = successfull;
            if (successfull)
                Instance = instance;
        }

        /// <summary>
        /// Gets the instance of the built object.
        /// </summary>
        /// <remarks>
        /// The value of this property is undefined when the binding has not been successful.
        /// </remarks>
        public object Instance { get; private set; }

        /// <summary>
        /// Gets a value defining if the binding was successful.
        /// </summary>
        public bool Successful { get; private set; }

        /// <summary>
        /// Creates a binding result for failing bindings.
        /// </summary>
        /// <returns></returns>
        public static BindingResult Failure()
        {
            return new BindingResult(false, null);
        }

        /// <summary>
        /// Creates a binding result for successful bindings.
        /// </summary>
        /// <param name="instance">The object successful built by the binder.</param>
        /// <returns>An instance of the <see cref="BindingResult"/> type.</returns>
        public static BindingResult Success(object instance)
        {
            return new BindingResult(true, instance);
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