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
using System.Security.Principal;
using System.Threading;
using NUnit.Framework;
using OpenRasta.Hosting.InMemory;
using OpenRasta.Security;
using OpenRasta.Testing;
using OpenRasta.Web.Pipeline;

namespace PrincipalAuthorizationAttribute_Specification
{
    public class when_the_user_is_not_authenticated : context
    {
        [Test]
        public void the_filter_doesnt_authorize_the_execution()
        {
            var context = new InMemoryCommunicationContext();
            var principal = new PrincipalAuthorizationAttribute { InRoles = new[] { "Administrators"}};

            principal.ExecuteBefore(context)
                .ShouldBe(PipelineContinuation.RenderNow);
        }
    }

    [TestFixture]
    public class when_the_user_is_authenticated : context
    {
        [Test]
        public void the_role_is_matched_and_execution_continues()
        {
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("name"), new[] {"Administrator"});

            var rastaContext = new InMemoryCommunicationContext();
            var authorizer = new PrincipalAuthorizationAttribute() { InRoles = new[] { "Administrator" } };
            authorizer.ExecuteBefore(rastaContext)
                .ShouldBe(PipelineContinuation.Continue);

            rastaContext.OperationResult.ShouldBeNull();
        }
        [Test]
        public void the_username_is_matched_and_execution_continues()
        {
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("johndoe"), new[] { "Administrator" });

            var rastaContext = new InMemoryCommunicationContext();
            var authorizer = new PrincipalAuthorizationAttribute() { Users = new[] { "johndoe" } };
            authorizer.ExecuteBefore(rastaContext)
                .ShouldBe(PipelineContinuation.Continue);

            rastaContext.OperationResult.ShouldBeNull();
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