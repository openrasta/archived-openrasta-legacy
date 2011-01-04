using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using NUnit.Framework;
using OpenRasta.Configuration;
using OpenRasta.Testing;

namespace OpenRasta.Tests.Integration.Regressions
{
    public class a_querystring_parameter_which_should_be_bound_to_a_type_that_does_not_expose_setters_and_instead_requires_use_of_a_constructor
        : server_context
    {
        private const string URL = "/queries/FooReportResource?LastReportDate={FooReportSpecification.lastReportDate}&ApplicableFoos={FooReportSpecification.ApplicableFoos}";

        public a_querystring_parameter_which_should_be_bound_to_a_type_that_does_not_expose_setters_and_instead_requires_use_of_a_constructor()
        {
            ConfigureServer(() =>
                            ResourceSpace.Has.ResourcesOfType<FooReportResource>()
                                .AtUri(URL)
                                .HandledBy<FooReportResourceHandler>()
                                .AsXmlDataContract());
        }

        [Test]
        public void the_specification_should_reflect_the_querystring_arguments()
        {
            const string dateOfLastReport = "2010-05-23Z";
            string[] fooIdentifiers = { "FooId1", "FooId2", "FooId3", "FooId4" };
            string requestUri = String.Format("/queries/FooReportResource?LastReportDate={0}&ApplicableFoos={1}"
                                              , dateOfLastReport
                                              , String.Join(",", fooIdentifiers));

            given_request("GET", requestUri);
            when_reading_response();
            TheResponse.AsFooReportResource().Foo.ShouldBe(
                new FooBuilder(DateTime.Parse(dateOfLastReport)
                               , fooIdentifiers).ToString());
        }

        #region Supporting Shizzle

        [DataContract]
        public class FooReportResource
        {
            public FooReportResource()
            {}

            [DataMember]
            public string Foo { get; set; }
        }

        public class FooReportSpecification
        {
            private readonly DateTime _lastReportDate;
            private readonly IEnumerable<FooIdentifier> _applicableFoos;

            public FooReportSpecification(DateTime lastReportDate, IEnumerable<FooIdentifier> applicableFoos)
            {
                _lastReportDate = lastReportDate;
                _applicableFoos = applicableFoos;
            }

            public IEnumerable<FooIdentifier> ApplicableFoos
            {
                get { return _applicableFoos; }
            }

            public DateTime LastReportDate
            {
                get { return _lastReportDate; }
            }
        }

        public class FooIdentifier
        {
            private readonly string _identifier;

            public FooIdentifier(string identifier)
            {
                _identifier = identifier;
            }

            public string Identifier { get { return _identifier; } }
        }

        public class FooBuilder
        {
            private readonly DateTime _theDate;
            private readonly string[] _identifiers;

            public FooBuilder(DateTime theDate, params string[] identifiers)
            {
                _theDate = theDate;
                _identifiers = identifiers;
            }

            public new string ToString()
            {
                StringBuilder responseBuilder = new StringBuilder();
                responseBuilder.AppendFormat("LastReportDate:{0};", _theDate);
                foreach (string identifier in _identifiers)
                {
                    responseBuilder.AppendFormat("{0},", identifier);
                }
                return responseBuilder.ToString();
            }
        }

        public class FooReportResourceHandler
        {
            public FooReportResource Get(FooReportSpecification specification)
            {
                var fooIdentifiers = specification.ApplicableFoos.Select(f => f.Identifier).ToArray();

                return new FooReportResource { Foo = new FooBuilder(specification.LastReportDate, fooIdentifiers).ToString() };
            }
        }

        #endregion
    }

    public static class context_helper
    {
        public static a_querystring_parameter_which_should_be_bound_to_a_type_that_does_not_expose_setters_and_instead_requires_use_of_a_constructor.FooReportResource AsFooReportResource(this HttpWebResponse response)
        {
            DataContractSerializer serializer = new DataContractSerializer(
                typeof(a_querystring_parameter_which_should_be_bound_to_a_type_that_does_not_expose_setters_and_instead_requires_use_of_a_constructor.FooReportResource));
            object preCast = serializer.ReadObject(response.GetResponseStream());
            var postCast = preCast as a_querystring_parameter_which_should_be_bound_to_a_type_that_does_not_expose_setters_and_instead_requires_use_of_a_constructor.FooReportResource;
            return postCast;
        }
    }

}
