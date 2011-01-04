using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;
using OpenRasta.Binding;
using OpenRasta.Codecs;
using OpenRasta.OperationModel;
using OpenRasta.OperationModel.CodecSelectors;
using OpenRasta.OperationModel.Hydrators;
using OpenRasta.Testing;
using OpenRasta.Tests.Unit.Fakes;
using OpenRasta.Tests.Unit.OperationModel.Filters;
using OpenRasta.Web;

namespace OpenRasta.Tests.Unit.OperationModel.CodecSelectors
{
    public class when_there_is_no_request_entity : requestcodecselector_context
    {
        [Test]
        public void only_operations_already_ready_for_invocation_get_returned()
        {
            given_request_entity_is_zero();
            given_filter();
            given_operations();

            when_filtering_operations();

            FilteredOperations.ShouldHaveCountOf(2);
            
            then_operation_should_be_selected("Get");
            then_operation_should_be_selected("GetWithOptionalValue");

        }

        IOperation then_operation_should_be_selected(string methodName)
        {
            return FilteredOperations.FirstOrDefault(x => x.Name == methodName).ShouldNotBeNull();
        }

        void given_request_entity_is_zero()
        {
            Context.Request.Entity.ContentLength = 0;
        }
    }

    public class when_there_is_no_content_type : requestcodecselector_context
    {
        [Test]
        public void the_content_type_is_set_to_application_octet_stream()
        {
            given_filter();
            given_operations();
            given_request_header_content_type((string)null);
            given_registration_codec<ApplicationOctetStreamCodec>();
            given_request_entity_body(new byte[] { 0 });

            when_filtering_operations();

            var selectedCodec = FilteredOperations.First(x=>x.Name == "PostForStream");
            selectedCodec.GetRequestCodec().CodecRegistration.MediaType.Matches(MediaType.ApplicationOctetStream)
                .ShouldBeTrue();
        }
    }
    public class when_there_is_a_request_entity : requestcodecselector_context
    {
        [Test]
        public void operations_without_any_member_do_not_get_a_codec_assigned()
        {
            given_filter();
            given_operations();
            given_request_header_content_type(MediaType.ApplicationOctetStream);
            given_registration_codec<ApplicationOctetStreamCodec>();
            given_request_entity_body(new byte[] { 0 });

            when_filtering_operations();

            FilteredOperations.First(x => x.Name == "Get").GetRequestCodec().ShouldBeNull();   
        }
        [Test]
        public void operations_with_partially_filled_members_still_get_codec_assigned()
        {
            given_filter();
            given_operations();
            given_request_header_content_type(MediaType.ApplicationXWwwFormUrlencoded);

            given_registration_codec<ApplicationXWwwFormUrlencodedKeyedValuesCodec>();
            given_request_entity_body("firstname=Frodo");

            given_operation_property(x => x.Name == "GetFrodo", "lastname", "baggins");

            when_filtering_operations();

            FilteredOperations.First(x => x.Name == "GetFrodo").GetRequestCodec()
                .ShouldNotBeNull()
                .CodecRegistration.CodecType
                    .ShouldBe<ApplicationXWwwFormUrlencodedKeyedValuesCodec>();
        }

        void given_operation_property(Func<IOperation, bool> predicate, string propertyName, string propertyValue)
        {
            Operations.First(predicate).Inputs.First().Binder.SetProperty(
                propertyName, 
                new object[]{propertyValue},
                (v,t)=>BindingResult.Success(v));
        }
    }
    public class when_a_codec_is_not_found : requestcodecselector_context
    {
        [Test]
        public void the_operation_is_not_selected()
        {
            given_filter();
            given_operations();
            given_request_header_content_type(MediaType.ApplicationOctetStream);
            given_registration_codec<ApplicationOctetStreamCodec>();
            given_request_entity_body(new byte[] { 0 });

            when_filtering_operations();

            FilteredOperations.FirstOrDefault(x => x.Name == "Post").ShouldBeNull();
        }
    }

    public class requestcodecselector_context : operation_filter_context<CodecSelectorHandler, RequestCodecSelector>
    {
        protected override RequestCodecSelector create_filter()
        {
            return new RequestCodecSelector(Codecs, Context.Request);
        }
    }

    public class CodecSelectorHandler
    {
        public object Get()
        {
            return null;
        }

        public void Post(string value)
        {
        }
        public void PostForStream(Stream stream)
        {
            
        }
        public void GetWithOptionalValue([Optional]int optionalIndex){}

        public void GetFrodo(Frodo frodo)
        {
            
        }
    }
}