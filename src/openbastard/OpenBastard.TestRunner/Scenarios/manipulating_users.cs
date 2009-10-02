using System.Xml.Serialization;
using OpenBastard.Infrastructure;
using OpenBastard.Resources;
using OpenRasta.Testing;

namespace OpenBastard.Scenarios
{
    public class manipulating_users : context.manipulating_users
    {
        object XmlResponse;

        User then_user
        {
            get { return (User)XmlResponse; }
        }

        public void can_create_a_user()
        {
            given_request_to(Uris.USERS)
                .Post()
                .EntityAsMultipartFormData(
                FormData.Text("FirstName", "Frodo"), 
                FormData.Text("LastName", "Baggins")
                );

            when_retrieving_the_response_as_user();

            then_response_should_be_201_created();

            then_user.FirstName.ShouldBe("Frodo");
            then_user.LastName.ShouldBe("Baggins");
            then_user.Id.ShouldNotBeNull();
        }

        void then_response_should_be_201_created()
        {
            Response.StatusCode.ShouldBe(201);
        }

        void when_retrieving_the_response_as_user()
        {
            when_retrieving_the_response_as_xml<User>();
        }

        void when_retrieving_the_response_as_xml<T>()
        {
            when_retrieving_the_response();
            var serializer = new XmlSerializer(typeof(T));
            XmlResponse = serializer.Deserialize(Response.Entity.Stream);
        }
    }

    namespace context
    {
        public abstract class manipulating_users : environment_context
        {
        }
    }
}