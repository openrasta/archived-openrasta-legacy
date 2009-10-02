#region License

/* Authors:
 *      Aaron Lerch (aaronlerch@gmail.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion

using Ninject;
using OpenRasta.Pipeline;

namespace OpenRasta.DI.Ninject
{
    /// <summary>
    /// A class to clean items out of the <see cref="IContextStore"/>.
    /// </summary>
    public class ContextStoreDependencyCleaner : IContextStoreDependencyCleaner
    {
        private readonly IKernel _kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextStoreDependencyCleaner"/> class.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public ContextStoreDependencyCleaner(IKernel kernel)
        {
            _kernel = kernel;
        }

        /// <summary>
        /// Destructs the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="instance">The instance.</param>
        public void Destruct(string key, object instance)
        {
            var store = _kernel.Get<IContextStore>();
            store[key] = null;
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