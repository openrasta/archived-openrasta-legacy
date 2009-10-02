using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenRasta.Diagnostics;
using OpenRasta.OperationModel.Filters;
using OpenRasta.Testing;
using OpenRasta.Web;

namespace OpenRasta.Tests.Unit.OperationModel.Filters
{
    namespace UriParameters_Specification
    {
        public class when_there_is_no_uri_parameter : uriparameters_context
        {
            [Test]
            public void all_operations_are_selected()
            {
                given_filter();
                given_operations();

                when_filtering_operations();

                FilteredOperations.ShouldHaveSameElementsAs(Operations);
            }
        }
        public class when_there_is_one_uri_parameter_list : uriparameters_context
        {
            [Test]
            public void an_operation_having_all_parameters_is_selected()
            {
                given_filter();
                given_operations();
                given_pipeline_uriparams(new NameValueCollection { { "index", "42" }, { "content", "value" } });

                when_filtering_operations();

                FilteredOperations.ShouldHaveCountOf(1).First().Name.ShouldBe("Post");
            }
            [Test]
            public void operations_not_having_the_correct_parameter_is_excluded()
            {
                given_filter();
                given_operations();
                given_pipeline_uriparams(new NameValueCollection { { "unknown", "value" }, { "content", "value" } });

                when_filtering_operations();

                FilteredOperations.ShouldHaveCountOf(0);
            }
            [Test]
            public void operations_with_the_wrong_parameter_type_are_not_selected()
            {
                given_filter();
                given_operations();
                given_pipeline_uriparams(new NameValueCollection { { "index", "notanumber" }, { "content", "value" } });

                when_filtering_operations();

                FilteredOperations.ShouldHaveCountOf(0);
            }
        }
    }
    public abstract class uriparameters_context : operation_filter_context<UriParameterFakeHandler, UriParametersFilter>
    {
        protected override void SetUp()
        {
            base.SetUp();
            Context.PipelineData.SelectedResource = new UriRegistration(null,null);// { UriTemplateParameters = new List<NameValueCollection>() };
        }
        protected FakeErrorCollector ErrorCollector { get; set; }

        protected override UriParametersFilter create_filter()
        {
            ErrorCollector = new FakeErrorCollector();
            return new UriParametersFilter(Context, ErrorCollector);
        }
    }

    public class UriParameterFakeHandler
    {
        public object Get(int index)
        {
            return null;
        }
        public object Post(int index, string content)
        {
            return null;
        }
    }
    public class FakeErrorCollector : IErrorCollector
    {
        public FakeErrorCollector()
        {
            Errors = new List<Error>();
        }
        public void AddServerError(Error error)
        {

            Errors.Add(error);

        }

        public IList<Error> Errors { get; set; }
    }
}
