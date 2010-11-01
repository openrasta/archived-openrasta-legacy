using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using OpenRasta.Authentication;
using OpenRasta.DI;
using OpenRasta.Pipeline;
using OpenRasta.Pipeline.Contributors;
using OpenRasta.Testing;
using OpenRasta.Tests;
using OpenRasta.Web;

namespace given_an_authentication_contributor
{
    public abstract class given_an_authentication_contributor : openrasta_context
    {
        protected override void SetUp()
        {
            base.SetUp();
            given_pipeline_contributor<AuthenticationChallengerContributor>();
        }
    }

    public class when_the_pipeline_is_notified_IOperationExecution : given_an_authentication_contributor
    {
        [Test]
        public void then_authentication_challenger_is_invoked()
        {
            // given

            // when
            when_sending_notification<KnownStages.IOperationExecution>();

            // then
            IsContributorExecuted.ShouldBeTrue();
        }
    }

    public class when_the_pipeline_is_notified_IResponseCoding : given_an_authentication_contributor
    {
        [Test]
        public void then_authentication_challenger_is_invoked()
        {
            // given

            // when
            when_sending_notification<KnownStages.IResponseCoding>();

            // then
            IsContributorExecuted.ShouldBeTrue();
        }
    }

    namespace _and_scheme
    {

        public abstract class _and_scheme : given_an_authentication_contributor
        {
            protected Mock<IAuthenticationScheme> mockScheme = new Mock<IAuthenticationScheme>();

            protected override void SetUp()
            {
                base.SetUp();
                given_dependency(mockScheme.Object);
                given_pipeline_contributor<AuthenticationChallengerContributor>();
            }
        }

        public class when_the_context_is_unauthorized : _and_scheme
        {
            [Test]
            public void then_the_authentication_scheme_is_challenged()
            {
                // given
                Context.OperationResult = new OperationResult.Unauthorized();

                // when
                when_sending_notification<KnownStages.IOperationExecution>();

                // then
                mockScheme.Verify(s => s.Challenge(Context.Response));
            }
        }

        public class when_the_context_is_ok : given_an_authentication_contributor
        {
            [Test]
            public void then_the_authentication_scheme_is_not_challenged()
            {
                // given
                var mockScheme = new Mock<IAuthenticationScheme>(MockBehavior.Strict);

                given_dependency(mockScheme.Object);

                Context.OperationResult = new OperationResult.OK();

                // when
                when_sending_notification<KnownStages.IOperationExecution>();

                // then
                mockScheme.VerifyAll();
            }
        }
    }
}


