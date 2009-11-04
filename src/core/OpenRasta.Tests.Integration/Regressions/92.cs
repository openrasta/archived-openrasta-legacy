using System;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using NUnit.Framework;
using OpenRasta.Codecs;
using OpenRasta.Configuration;
using OpenRasta.Configuration.Fluent;
using OpenRasta.Data;
using OpenRasta.Testing;
using OpenRasta.Web;

namespace OpenRasta.Tests.Integration.Regressions
{
    public class changeset_not_hydrated_properly : server_context
    {
        public changeset_not_hydrated_properly()
        {
            ConfigureServer(() =>
            {
                ResourceSpace.Has.ResourcesOfType<Product>()
                    .AtUri("/products/{name}")
                    .HandledBy<ProductHandler>()
                    .AsXmlDataContract();
            });
        }

        protected SyndicationFeed Feed { get; set; }

        protected SyndicationItem Item { get; set; }

        [Test]
        public void the_handler_for_individual_items_is_selected()
        {
            given_request("POST", "/products/moq",Encoding.UTF8.GetBytes("Description=The+description+of+MOQ"), MediaType.ApplicationXWwwFormUrlencoded);

            when_reading_response();

            TheResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
    }

    public class ProductHandler
    {
        public OperationResult Post(string name, ChangeSet<Product> product)
        {
            if (name == "moq" && product.Changes.Count == 1)
                return new OperationResult.OK();
            return new OperationResult.MethodNotAllowed();
        }
    }

    public class Product
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}