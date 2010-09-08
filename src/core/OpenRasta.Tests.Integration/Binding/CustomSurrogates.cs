using System;
using System.Net;
using System.Text;
using NUnit.Framework;
using OpenRasta.Configuration;
using OpenRasta.Configuration.Fluent;
using OpenRasta.DI;
using OpenRasta.Security;
using OpenRasta.Testing;
using OpenRasta.Tests.Integration;
using OpenRasta.TypeSystem;
using OpenRasta.TypeSystem.Surrogates;
using OpenRasta.TypeSystem.Surrogates.Static;
using OpenRasta.Web;

namespace Dynamic_surrogates
{
    public class adding_custom_surrogate : context.surrogates_context
    {

        [Test, Ignore("not implemented yet.")]
        public void surrogate_is_used()
        {
            given_request("GET", "/customer/3");

            when_reading_response();

        }
    }

    public class MySurrogate : AbstractStaticSurrogate<Customer>
    {
        public int Id
        {
            get { return 0; }
            set{throw new InvalidOperationException();}
        }
    }

    namespace context
    {
        public abstract class surrogates_context : server_context
        {

            public surrogates_context()
        {
            ConfigureServer(() =>
            {

                ResourceSpace.Has.ResourcesOfType<Customer>()
                    .AtUri("/customer/{id}")
                    .HandledBy<Handler>();

                ResourceSpace.Uses.CustomDependency<ISurrogateBuilder, MySurrogate>(DependencyLifetime.Transient);
            });
        }
        }
    }
    
    public class Handler
    {
        public OperationResult Get(Customer customer)
        {
            return new OperationResult.OK();
        }

    }
}