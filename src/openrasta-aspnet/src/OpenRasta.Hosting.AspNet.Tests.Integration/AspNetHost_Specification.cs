using System.Net;
using System.Text;
using NUnit.Framework;
using OpenRasta.Configuration;
using OpenRasta.Hosting.AspNet.Tests.Integration;
using OpenRasta.Testing;
using OpenRasta.Web;

namespace AspNetHost_Specification
{
    public class when_issueing_a_get_for_a_resource : aspnet_server_context
    {
        public when_issueing_a_get_for_a_resource()
        {
            ConfigureServer(
                () => ResourceSpace.Has.ResourcesOfType<Customer>()
                          .AtUri("/{customerId}")
                          .HandledBy<CustomerHandler>());
        }

        [Test]
        public void the_request_is_matched_to_the_parameter()
        {
            GivenATextRequest("PATCH", "/3", "new customer name", "UTF-16");
            GivenTheResponseIsInEncoding(Encoding.ASCII);

            TheResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            TheResponse.ContentType.ShouldContain("text/plain");
            TheResponse.Headers["Location"].ShouldBe("http://127.0.0.1:6687/3");

            TheResponseAsString.ShouldBe("new customer name");
        }
    }

    public class when_accessing_a_uri_meant_for_a_handler : aspnet_server_context
    {
        public when_accessing_a_uri_meant_for_a_handler()
        {
            ConfigureServer(() => ResourceSpace.Has.ResourcesOfType<Customer>()
                                      .AtUri("/customer/{customerId}")
                                      .HandledBy<CustomerHandler>());
        }

        [Test]
        public void oepnrasta_doesnt_process_the_request()
        {
            GivenARequest("GET", "/customer/3.notimplemented");
            GivenTheResponseIsInEncoding(Encoding.ASCII);

            TheResponse.StatusCode.ShouldBe(HttpStatusCode.NotImplemented);
        }
    }

    public class when_accessing_an_unmapped_uri : aspnet_server_context
    {
        public when_accessing_an_unmapped_uri()
        {
            ConfigureServer(() => ResourceSpace.Has.ResourcesOfType<Customer>()
                                      .AtUri("/customer/{customerId}")
                                      .HandledBy<CustomerHandler>());
        }

        [Test]
        public void openrasta_doesnt_process_the_request()
        {
            GivenARequest("GET", "/mappedCustomers");
            GivenTheResponseIsInEncoding(Encoding.ASCII);

            TheResponse.StatusCode.ShouldBe(HttpStatusCode.MethodNotAllowed);
        }
    }

    public class Customer
    {
        public int CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class CustomerHandler
    {
        public OperationResult Get(string customerId)
        {
            return new OperationResult.OK();
        }

        public OperationResult Patch(int customerId, string customerName)
        {
            return new OperationResult.OK
                {
                    ResponseResource = customerName, 
                    RedirectLocation = new Customer { CustomerID = customerId }.CreateUri()
                };
        }
    }
}