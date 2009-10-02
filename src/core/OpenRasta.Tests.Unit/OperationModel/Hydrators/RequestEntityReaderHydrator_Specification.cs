using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenRasta.Binding;
using OpenRasta.Codecs;
using OpenRasta.Diagnostics;
using OpenRasta.OperationModel;
using OpenRasta.OperationModel.Hydrators;
using OpenRasta.OperationModel.Hydrators.Diagnostics;
using OpenRasta.Testing;
using OpenRasta.Tests.Unit.Fakes;
using OpenRasta.Tests.Unit.OperationModel.Filters;
using OpenRasta.TypeSystem;
using OpenRasta.Web;
using OpenRasta.Web.Codecs;

namespace OpenRasta.Tests.Unit.OperationModel.Hydrators
{
    public class when_multiple_operations_are_defined_without_codec : request_entity_reader_context
    {
        [Test]
        public void the_one_with_the_highest_number_of_satisfied_parameters_and_ready_for_invocation_is_selected()
        {
            given_filter();
            given_operations();

            given_operation_value("PostName", "frodo", new Frodo());

            given_operation_value("PostAddress", "frodo", new Frodo());
            given_operation_value("PostAddress", "address", new Address());

            when_entity_is_read();

            ResultOperation.Name.ShouldBe("PostAddress");
        }
    }
    public class when_multiple_operations_are_defined_with_codecs : request_entity_reader_context
    {
        [Test]
        public void the_one_with_the_highest_score_is_selected()
        {
            given_filter();
            given_operations();
            given_operation_has_codec_match<ApplicationOctetStreamCodec>("PostName", MediaType.Xml, 1.0f);
            given_operation_has_codec_match<ApplicationXWwwFormUrlencodedKeyedValuesCodec>("PostAddress", MediaType.Xml, 2.0f);

            when_filtering_operations();

            FilteredOperations
                .ShouldHaveCountOf(1)
            .First().GetRequestCodec().CodecRegistration.CodecType
                .ShouldBe<ApplicationXWwwFormUrlencodedKeyedValuesCodec>();
        }
        [Test]
        public void the_one_without_a_codec_is_not_selected()
        {
            given_filter();
            given_operations();

            given_operation_has_codec_match<ApplicationOctetStreamCodec>("PostName", MediaType.Xml, 1.0f);

            when_filtering_operations();

            FilteredOperations
                .ShouldHaveCountOf(1)
            .First().GetRequestCodec().CodecRegistration.CodecType
                .ShouldBe<ApplicationOctetStreamCodec>();
        }
    }
    public class when_codec_supports_keyed_values : request_entity_reader_context
    {
        [Test]
        public void the_keyed_values_are_used_to_build_the_parameter()
        {
            given_filter();
            given_operations();
            given_operation_has_codec_match<ApplicationXWwwFormUrlencodedKeyedValuesCodec>("PostName", MediaType.Xml, 1.0f);
            given_request_entity_body("Frodo.LastName=Baggins&Frodo.Unknown=avalue");

            when_entity_is_read();

            ResultOperation.Inputs.Required().First().Binder.BuildObject()
                .Instance.ShouldBeOfType<Frodo>()
                .LastName.ShouldBe("Baggins");
        }
    }
    public class when_codec_supports_object_building : request_entity_reader_context
    {
        [Test]
        public void the_object_is_built()
        {
            given_filter();
            given_operations();
            given_operation_has_codec_match<ApplicationOctetStreamCodec>("PostStream", MediaType.ApplicationOctetStream, 1.0f);
            given_request_entity_body(new byte[]{0});

            when_entity_is_read();

            ResultOperation.Name.ShouldBe("PostStream");
            ResultOperation.Inputs.Required().First().Binder.BuildObject()
                .Instance.ShouldBeOfType<Stream>()
                .ReadByte().ShouldBe(0);
        }
        [Test]
        public void an_error_is_collected_if_codec_raises_an_error()
        {

            given_filter();
            given_operations();
            
            given_operation_has_codec_match<XmlDataContractCodec>("PostName", MediaType.Xml, 1.0f);
            given_request_entity_body(new byte[] { 0 });

            when_filtering_operations();
            FilteredOperations.ShouldBeEmpty();
            Errors.Errors.ShouldHaveCountOf(1);

        }
    }
    public class when_no_operation_can_be_processed : request_entity_reader_context
    {
        [Test]
        public void no_operation_gets_selected()
        {
            given_filter();
            given_operations();

            when_filtering_operations();

            FilteredOperations.ShouldBeEmpty();
        }
    }
    public class when_no_processable_operation_ : request_entity_reader_context
    {
        [Test]
        public void no_operation_gets_selected()
        {
            given_filter();
            given_operations();

            when_filtering_operations();

            FilteredOperations.ShouldBeEmpty();
        }
    }
    public abstract class request_entity_reader_context : operation_filter_context<EntityReaderHandler, RequestEntityReaderHydrator>
    {
        protected override RequestEntityReaderHydrator create_filter()
        {
            return new RequestEntityReaderHydrator(Resolver, Request)
            {
                ErrorCollector = Errors,
                Log = new TraceSourceLogger<CodecLogSource>()
            };
        }

        protected void given_operation_has_codec_match<TCodec>(string name, MediaType mediaType, float codecScore)
        {
            Operations.First(x=>x.Name == name).SetRequestCodec(new CodecMatch(new CodecRegistration(typeof(TCodec),Guid.NewGuid(),mediaType), codecScore, 1));

        }
        protected void when_entity_is_read()
        {
            when_filtering_operations();
            FilteredOperations.ShouldHaveCountOf(1);

            ResultOperation = FilteredOperations.Single();
        }

        protected IOperation ResultOperation { get; set; }
    }

    public class EntityReaderHandler
    {
        public string PostName(Frodo frodo)
        {
            return null;
        }
        public string PostAddress(Frodo frodo, Address address)
        {
            return null;
        }
        public string PostStream(Stream anOject)
        {
            return null;
        }
        public string PostTwo(Frodo frodo1, Frodo frodo2){
            return null;}
    }
}
