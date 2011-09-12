using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using NUnit.Framework;
using OpenRasta.Configuration;
using OpenRasta.Configuration.Fluent;
using OpenRasta.Testing;
using OpenRasta.Web.UriDecorators;

namespace OpenRasta.Tests.Integration.UriDecorators
{
    public class when_file_extensions_are_activated : server_context
    {
        public when_file_extensions_are_activated()
        {
            ConfigureServer(()=>
            {
                ResourceSpace.Has.ResourcesOfType<Customer>()
                    .AtUri("/customer")
                    .HandledBy<CustomerHandler>()
                    .AsXmlDataContract();

                ResourceSpace.Uses.UriDecorator<ContentTypeExtensionUriDecorator>();
            });
        }
        [Test]
        public void a_request_on_the_generic_uri_returns_the_correct_entity()
        {
            given_request("GET", "/customer");
            when_reading_response();
            TheResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            TheResponse.ContentType.ShouldContain("application/xml");
        }
        [Test]
        public void a_request_on_the_extension_uri_returns_the_correct_entity()
        {
            given_request("GET", "/customer.xml");
            when_reading_response();
            TheResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            TheResponse.ContentType.ShouldContain("application/xml");
        }
    }

}
