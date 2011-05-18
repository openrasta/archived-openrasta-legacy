using System;
using System.IO;
using NUnit.Framework;
using OpenRasta.Codecs;
using OpenRasta.Codecs.jsonp;
using OpenRasta.Hosting.InMemory;
using OpenRasta.Testing;

namespace OpenRasta.Tests.Unit.Codecs
{
    public class when_writing_a_jsonp_representation : context
    {
        JsonPCodec<JsonDataContractCodec> codec;
        InMemoryResponse stub;

        public void GivenACodec(string handlerName)
        {
            codec = new JsonPCodec<JsonDataContractCodec>(handlerName, new JsonDataContractCodec());
            stub = new InMemoryResponse();
        }

        public void When_writing_content(object entity)
        {
            codec.WriteTo(entity, stub.Entity, null);
        }

        [Test]
        public void the_content_is_written_to_the_stream()
        {
           GivenACodec("my_handler");
           When_writing_content(new Customer { Name = "hello" });
            
           stub.Entity.Stream.Position.ShouldNotBe(0);
        }

        [Test]
        public void when_the_handler_is_a_naked_function()
        {
            GivenACodec("my_handler");
            When_writing_content(new Customer{ Name = "hello"});
            var content = ReadContent();
            content.ShouldBe(@"my_handler({""Name"":""hello""});");
        }

        [Test]
        public void when_the_handler_is_a_method_call()
        {
            GivenACodec("myobj.foo");
            When_writing_content(new Customer { Name = "hello" });
            var content = ReadContent();
            content.ShouldBe(@"myobj.foo({""Name"":""hello""});");
        }

        [Test]
        public void when_an_identifier_contains_a_dollar()
        {
            GivenACodec("$jsonp.handle");
            When_writing_content(new Customer { Name = "hello" });
            var content = ReadContent();
            content.ShouldBe(@"$jsonp.handle({""Name"":""hello""});");
        }

        [Test]
        public void when_the_handler_is_a_poor_mans_switch()
        {
            GivenACodec("myobj['foo']");
            When_writing_content(new Customer { Name = "hello" });
            var content = ReadContent();
            content.ShouldBe(@"myobj['foo']({""Name"":""hello""});");
        }

        [Test]
        public void when_the_handler_is_a_poor_mans_switch_with_double_quotes()
        {
            GivenACodec("myobj[\"foo\"]");
            When_writing_content(new Customer { Name = "hello" });
            var content = ReadContent();
            content.ShouldBe(@"myobj[""foo""]({""Name"":""hello""});");
        }

        [Test]
        public void when_the_handler_is_an_xss_attack_waiting_to_happen()
        {
            Exception thrown = null;

            GivenACodec("alert('bad-hax');void");

            try
            {
                When_writing_content(new Customer { Name = "hello" });
            }
            catch (Exception e)
            {
                thrown = e;   
            }

            thrown.ShouldBeOfType<InvalidJsonCallbackException>();
        }

        string ReadContent()
        {
            stub.Entity.Stream.Position = 0;
            return new StreamReader(stub.Entity.Stream).ReadToEnd();
        }

        public class Customer { public string Name { get; set; } }
    }


}