using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Moq;
using NUnit.Framework;
using OpenRasta.Hosting.InMemory;
using OpenRasta.OperationModel;
using OpenRasta.Security;
using OpenRasta.Testing;
using OpenRasta.Web;

namespace OpenRasta.Tests.Unit.Security
{
    public class when_the_user_is_in_the_correct_group : context
    {
        [Test]
        public void execution_is_not_allowed()
        {
            var context = new InMemoryCommunicationContext();
            context.User = new GenericPrincipal(new GenericIdentity("name"), new[] { "Administrator" });

            var authenticationInterceptor = new RequiresRoleInterceptor(context) { Role = "Administrator" };

            authenticationInterceptor.BeforeExecute(new Mock<IOperation>().Object)
                .ShouldBeTrue();
        }
    }

    public class when_the_user_is_not_in_the_correct_group : context
    {
        [Test]
        public void execution_is_not_allowed()
        {
            var context = new InMemoryCommunicationContext();
            context.User = new GenericPrincipal(new GenericIdentity("name"), new []{"Administrator"});

            var authenticationInterceptor = new RequiresRoleInterceptor(context) { Role = "SuperUser" };

            authenticationInterceptor.BeforeExecute(new Mock<IOperation>().Object)
                .ShouldBeFalse();
            context.OperationResult.ShouldBeOfType<OperationResult.Unauthorized>();

        }
    }
}
