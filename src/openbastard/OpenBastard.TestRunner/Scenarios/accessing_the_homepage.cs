using System.Diagnostics;
using OpenBastard.Infrastructure;
using OpenRasta.Testing;
using OpenRasta.Web;

namespace OpenBastard.Scenarios
{
    /// <summary>
    /// As a user-agent, I want to access the homepage so I can get links to various parts of the application
    /// </summary>
    public class accessing_the_homepage : context.accessing_the_homepage
    {
        public void the_homepage_can_be_retrieved_using_xml()
        {
            given_request_to("/")
                .Get()
                .Accept(MediaType.Xml);
            when_retrieving_the_response();

            Response.StatusCode.ShouldBe(200);
            Response.Entity.ContentType.ShouldBe(MediaType.Xml);
            Response.Entity.ContentLength.Value.ShouldBeGreaterThan(0);
        }
    }

    namespace context
    {
        public abstract class accessing_the_homepage : environment_context
        {
        }
    }
}