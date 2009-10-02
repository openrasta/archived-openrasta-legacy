#region License

/* Authors:
 *      Aaron Lerch (aaronlerch@gmail.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion

using System;
using Ninject;
using Ninject.Activation;
using Ninject.Activation.Providers;
using Ninject.Planning;
using Ninject.Selection;
using Ninject.Syntax;
using OpenRasta.DI.Internal;
using OpenRasta.Pipeline;

namespace OpenRasta.DI.Ninject
{
    /// <summary>
    /// A Ninject provider that resolves/caches instances on a OpenRasta PerRequest basis
    /// using <see cref="IContextStore"/>.
    /// </summary>
    public class PerRequestProvider : StandardProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PerRequestProvider"/> class.
        /// </summary>
        /// <param name="type">The type (or prototype) of instances the provider creates.</param>
        /// <param name="planner">The <see cref="IPlanner"/> component.</param>
        /// <param name="selector">The <see cref="ISelector"/> component</param>
        public PerRequestProvider(Type type, IPlanner planner, ISelector selector)
            : base(type, planner, selector)
        { }

        /// <summary>
        /// Creates an instance within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The created instance.</returns>
        public override object Create(IContext context)
        {
            var store = GetStore(context.Kernel);
            string key = context.Request.Service.GetKey();

            if (store[key] != null)
            {
                return store[key];
            }

            var instance = base.Create(context);
            store[key] = instance;

            store.GetContextInstances().Add(new ContextStoreDependency(key, instance, 
                                                                       new ContextStoreDependencyCleaner(context.Kernel)));

            return store[key];
        }

        private static IContextStore GetStore(IResolutionRoot kernel)
        {
            var store = kernel.TryGet<IContextStore>();
            if (store == null)
                throw new InvalidOperationException(
                    "There is no IContextStore implementation registered in the container.");
            return store;
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