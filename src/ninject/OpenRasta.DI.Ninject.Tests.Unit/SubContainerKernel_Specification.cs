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
using NUnit.Framework;
using OpenRasta.Testing;
using OpenRasta.Tests.Unit.DI;

namespace OpenRasta.DI.Ninject.Tests.Unit
{
    [TestFixture]
    public class when_using_a_parent_ninject_kernel : context
    {
        public IKernel ParentKernel { get; set; }
        public IKernel Kernel { get; set; }

        protected override void SetUp()
        {
            base.SetUp();
            ParentKernel = new StandardKernel();
            Kernel = new SubContainerKernel(ParentKernel);
        }

        [Test]
        public void a_type_registered_with_the_parent_should_be_resolved()
        {
            ParentKernel.Bind<ISimple>().To<Simple>();
            var simple = Kernel.Get<ISimple>();

            simple.ShouldNotBeNull();
            simple.ShouldBeOfType<Simple>();
        }

        [Test]
        public void a_type_registered_with_both_kernels_should_be_resolved()
        {
            ParentKernel.Bind<ISimple>().To<Simple>();
            Kernel.Bind<ISimple>().To<Simple>();

            var simpleList = Kernel.GetAll<ISimple>();

            simpleList.ShouldHaveCountOf(2);
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