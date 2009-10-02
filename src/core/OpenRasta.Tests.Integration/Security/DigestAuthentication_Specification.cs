using System.Net;
using System.Text;
using NUnit.Framework;
using OpenRasta.Configuration;
using OpenRasta.Configuration.Fluent;
using OpenRasta.DI;
using OpenRasta.Security;
using OpenRasta.Testing;
using OpenRasta.Tests.Integration;
using OpenRasta.Web;

namespace DigestAuthentication_Specification
{
    public class when_using_the_correct_credentials : server_context
    {
        public when_using_the_correct_credentials()
        {
            ConfigureServer(() =>
            {
                DependencyManager.GetService<IDependencyResolver>()
                    .AddDependency<IAuthenticationProvider, FakeAuthProvider>();

                ResourceSpace.Has.ResourcesOfType<Customer>()
                    .AtUri("/{somewhere}")
                    .AndAt("/unprotected").Named("unprotected")
                    .HandledBy<ProtectedCustomerHandler>();
            });
        }

        [Test]
        public void a_protected_resource_fails_with_unauthorized_error_when_no_credentials_are_provided()
        {
            given_request("GET", "/protected");

            when_reading_response();

            TheResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Test]
        public void an_unprotected_resource_returns_200_even_with_invalid_credentials()
        {
            given_client_credentials("username", "wrongpassword");
            given_request("GET", "/unprotected");

            when_reading_response_as_a_string(Encoding.ASCII);

            TheResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public void the_user_authentication_is_successfull_on_URIs_with_encoded_characters()
        {
            given_client_credentials("username", "password");
            given_request("GET", "/café");
            when_reading_response_as_a_string(Encoding.ASCII);

            TheResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public void the_user_is_authenticated_and_a_200_response_is_returned()
        {
            given_client_credentials("username", "password");
            given_request("GET", "/home");
            when_reading_response_as_a_string(Encoding.ASCII);

            TheResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
    }

    public class FakeAuthProvider : IAuthenticationProvider
    {
        public Credentials GetByUsername(string p)
        {
            return new Credentials { Username = p, Password = "password" };
        }
    }

    public class ProtectedCustomerHandler
    {
        [RequiresAuthentication]
        public OperationResult Get(string somewhere)
        {
            return new OperationResult.OK();
        }

        [HttpOperation(ForUriName = "unprotected")]
        public OperationResult Get()
        {
            return new OperationResult.OK();
        }
    }
}