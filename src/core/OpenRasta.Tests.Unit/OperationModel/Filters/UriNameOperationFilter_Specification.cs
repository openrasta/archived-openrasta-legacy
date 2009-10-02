using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenRasta.OperationModel.Filters;
using OpenRasta.Testing;

namespace OpenRasta.Tests.Unit.OperationModel.Filters
{
    public class uriname_filter_context : operation_filter_context<Handler, UriNameOperationFilter>
    {
        protected override UriNameOperationFilter create_filter()
        {
            return new UriNameOperationFilter(Context);
        }
    }

    public class when_a_uri_name_is_present : uriname_filter_context
    {
        [Test]
        public void methods_with_the_attribute_are_included()
        {
            given_pipeline_selectedHandler<Handler>();
            given_filter();
            given_request_uriName("RouteName");
            given_request_httpmethod("GET");
            given_operations();

            when_filtering_operations();

            FilteredOperations.ShouldHaveCountOf(2);

            FilteredOperations.SingleOrDefault(x => x.Name == "GetForRouteName")
                .ShouldNotBeNull();
            FilteredOperations.SingleOrDefault(x => x.Name == "PostForRouteName")
                .ShouldNotBeNull();
        }
    }
    public class when_no_uri_name_is_resent : uriname_filter_context
    {
        [Test]
        public void no_operation_gets_removed()
        {
            given_pipeline_selectedHandler<Handler>();
            given_filter();
            given_request_httpmethod("GET");
            given_operations();

            when_filtering_operations();

            FilteredOperations.ShouldHaveSameElementsAs(Operations);
        }
    }
}
