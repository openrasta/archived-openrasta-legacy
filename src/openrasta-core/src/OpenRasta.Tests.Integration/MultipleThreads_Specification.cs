using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using NUnit.Framework;
using OpenRasta.Configuration;
using OpenRasta.Configuration.Fluent;
using OpenRasta.Testing;
using OpenRasta.Tests.Integration;
using OpenRasta.Web;

namespace MultipleThreads_Specification
{

    [TestFixture]
    public class test_container : server_context
    {
        public test_container()
        {
            ConfigureServer(
                () => ResourceSpace.Has.ResourcesOfType<Customer>().AtUri("/customers/{id}").HandledBy<CustomerHandler>()
            );
        }
        [Test, RequiresMTA]
        public void testing_multiple_concurrent_requests()
        {
            var list = Enumerable.Range(0, 1000).Select(i => new test_case(_port)).ToList();

            foreach (var item in list)
            {
                test_case @case = item;
                WaitCallback cb = s => @case.a_response_is_received_upon_a_get();
                ThreadPool.QueueUserWorkItem(cb);
            }
            foreach (var item in list)
                if(!item.Done.WaitOne(TimeSpan.FromSeconds(10)))
                    Assert.Fail("Threads didn't respond in the allocated time");

            var exceptions = list.Select(i => i.collectedException).Where(i=>i!=null).ToList();
            if (exceptions.Count > 0)
                throw new AssertionException(exceptions.Aggregate("", (s, e) => s + e.ToString() + "\r\n"));
        } 
    }
    public class test_case : server_context
    {
        static Random Random = new Random();
        public ManualResetEvent Done = new ManualResetEvent(false);
        public Exception collectedException = null;

        public test_case(int port)
            : base(port)
        {

        }

        public void a_response_is_received_upon_a_get()
        {
            try
            {
                var id = Random.Next();
                given_request("GET", "/customers/" + id);
                when_reading_response_as_a_string(Encoding.ASCII);

                TheResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
                TheResponseAsString.ShouldBe(id.ToString());
            }
            catch (Exception e)
            {
                collectedException = e;
                throw;
            }
            finally
            {
                Done.Set();
            }
        }
    }
    public class CustomerHandler
    {
        public OperationResult Get(int id)
        {
            return new OperationResult.OK { ResponseResource = id.ToString() };
        }

    }
}
