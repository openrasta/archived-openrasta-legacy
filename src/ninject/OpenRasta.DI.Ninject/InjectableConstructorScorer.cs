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
using System.Linq;
using Ninject.Activation;
using Ninject.Components;
using Ninject.Planning.Directives;
using Ninject.Selection.Heuristics;

namespace OpenRasta.DI.Ninject
{
    /// <summary>
    /// Scores constructors by either looking for the existence of an injection marker
    /// attribute, or by counting the number of parameters that can be injected.
    /// </summary>
    public class InjectableConstructorScorer : NinjectComponent, IConstructorScorer
    {
        /// <summary>
        /// Gets the score for the specified constructor. Looks at the number of "resolvable" arguments.
        /// </summary>
        /// <param name="context">The injection context.</param>
        /// <param name="directive">The constructor.</param>
        /// <returns>The constructor's score.</returns>
        public int Score(IContext context, ConstructorInjectionDirective directive)
        {
            if (directive.Constructor.HasAttribute(Settings.InjectAttribute))
                return Int32.MaxValue;

            var bindableParameters = from param in directive.Constructor.GetParameters()
                                     where !param.IsOut && !param.IsRetval && !param.IsOptional
                                           && param.ParameterType.IsBindable(context.Kernel)
                                     select param;
            return bindableParameters.Count();
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