using System.Collections.Generic;
using System.Reflection;
using Ninject;
using Ninject.Components;
using Ninject.Parameters;
using Ninject.Selection.Heuristics;

namespace OpenRasta.DI.Ninject
{
    /// <summary>
    /// Determines whether members should be injected during activation by checking
    /// if they provide a public setter and have an existing binding.
    /// </summary>
    public class AllResolvablePropertiesInjectionHeuristic : NinjectComponent, IInjectionHeuristic
    {
        private readonly IKernel _kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AllResolvablePropertiesInjectionHeuristic"/> class.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public AllResolvablePropertiesInjectionHeuristic(IKernel kernel)
        {
            _kernel = kernel;
        }

        private static readonly IEnumerable<IParameter> EmptyParameters = new IParameter[] { };

        /// <summary>
        /// Returns a value indicating whether the specified member should be injected.
        /// </summary>
        /// <param name="member">The member in question.</param>
        /// <returns>
        ///     <see langword="true"/> if the member should be injected; otherwise <see langword="false"/>.
        /// </returns>
        public bool ShouldInject(MemberInfo member)
        {
            if (member.MemberType != MemberTypes.Property)
                return false;

            var propertyInfo = member as PropertyInfo;
            if (propertyInfo == null)
                return false;

            if (propertyInfo.GetSetMethod() == null)
                return false;

            // If the types are the same, or if the property is an interface or abstract class
            // that the declaring type implements (which would cause a cyclic resolution)
            if ((propertyInfo.PropertyType == propertyInfo.DeclaringType)
                || ((propertyInfo.DeclaringType.IsAssignableFrom(propertyInfo.PropertyType))))
                return false;

            var request = _kernel.CreateRequest(propertyInfo.PropertyType, null, EmptyParameters, true, false);
            return _kernel.CanResolve(request);
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