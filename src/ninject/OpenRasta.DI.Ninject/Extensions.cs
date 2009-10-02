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
using System.Reflection;
using Ninject;
using Ninject.Parameters;

namespace OpenRasta.DI.Ninject
{
    /// <summary>
    /// Extensions to enable more readable code.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Determines whether the specified service type is bindable.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <param name="kernel">The kernel.</param>
        /// <returns>
        /// 	<see langword="true"/> if the specified service type is bindable; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsBindable(this Type serviceType, IKernel kernel)
        {
            var request = kernel.CreateRequest(serviceType, null, new IParameter[] { }, false);
            return kernel.CanResolve(request);
        }

        /// <summary>
        /// Determines whether the specified member has a particular attribute.
        /// </summary>
        /// <param name="member">The member to check.</param>
        /// <param name="type">The attribute type to check for.</param>
        /// <returns>
        /// 	<see langword="true"/> if the specified member has attribute; otherwise, <see langword="true"/>.
        /// </returns>
        public static bool HasAttribute(this ICustomAttributeProvider member, Type type)
        {
            return member.IsDefined(type, true);
        }

        /// <summary>
        /// Gets a unique key for a given type.
        /// </summary>
        /// <returns>Returns the AssemblyQualifiedName.</returns>
        public static string GetKey(this Type type)
        {
            return type.AssemblyQualifiedName;
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