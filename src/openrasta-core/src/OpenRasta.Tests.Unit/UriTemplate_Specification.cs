#region License

/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */

#endregion

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using NUnit.Framework;
using OpenRasta;
using OpenRasta.Collections;
using OpenRasta.Collections.Specialized;
using OpenRasta.Testing;

namespace UriTemplate_Specification
{
    public class uritemplate_context : context
    {
        protected IEnumerable<Uri> BaseUris = new List<Uri> {new Uri("http://localhost")};

        public IEnumerable<string> BindingUriByName(string template, object values)
        {
            foreach (Uri baseUri in BaseUris)
                yield return new OpenRasta.UriTemplate(template)
                    .BindByName(baseUri, values.ToNameValueCollection()).ToString();
        }

        public void GivenBaseUris(params string[] uris)
        {
            BaseUris = new List<Uri>(uris.Select(u => new Uri(u)));
        }
    }

    [TestFixture]
    public class when_accessing_path_segments : uritemplate_context
    {
        [Test]
        public void all_valid_variables_are_returned()
        {
            new OpenRasta.UriTemplate("weather/{state}/{city}").PathSegmentVariableNames
                .ShouldHaveSameElementsAs(new[] {"STATE", "CITY"});
        }
    }

    [TestFixture]
    public class when_binding_urls_by_name : uritemplate_context
    {
        [Test]
        public void the_values_in_the_query_string_are_injected()
        {
            BindingUriByName("/test?query={value}", new {value = "myQuery"})
                .ShouldAllBe("http://localhost/test?query=myQuery");
        }
    }

    [TestFixture]
    public class when_binding_uris_by_name_in_a_vpath : uritemplate_context
    {
        public when_binding_uris_by_name_in_a_vpath()
        {
            GivenBaseUris("http://localhost/foo", "http://localhost/foo/");
        }

        [Test]
        public void a_query_string_is_appended_successfully()
        {
            BindingUriByName("?query={value}", new {value = "myQuery"}).ShouldAllBe("http://localhost/foo/?query=myQuery");
        }

        [Test]
        public void a_segment_is_appended_successfully()
        {
            BindingUriByName("/{value}", new {value = "myQuery"}).ShouldAllBe("http://localhost/foo/myQuery");
        }
        [Test]
        public void an_unprefixed_segment_is_appended_successfully()
        {
            BindingUriByName("{value}", new { value = "myQuery" }).ShouldAllBe("http://localhost/foo/myQuery");
        }
    }
    [TestFixture]
    public class when_matching_urls : uritemplate_context
    {
        OpenRasta.UriTemplateMatch ThenTheMatch;

        void GivenAMatching(string template, string candidate)
        {
            GivenAMatching("http://localhost/", template, candidate);
        }

        void GivenAMatching(string baseUri, string template, string candidate)
        {
            ThenTheMatch = new OpenRasta.UriTemplate(template).Match(baseUri.ToUri(), candidate.ToUri());
        }

        [Test]
        public void a_template_on_the_root_gets_a_match()
        {
            GivenAMatching("/", "http://localhost/");
            
            ThenTheMatch.ShouldNotBeNull();
        }

        [Test]
        public void matching_urls_with_different_host_names_returns_no_match()
        {
            var table = new OpenRasta.UriTemplate("/temp");
            table.Match(new Uri("http://localhost"), new Uri("http://notlocalhost/temp")).ShouldBeNull();
        }

        [Test]
        public void the_base_uri_is_the_one_provided_in_the_match()
        {
            GivenAMatching("/weather/{state}/{city}", "http://localhost/weather/Washington/Seattle");
            ThenTheMatch.BaseUri
                .ShouldBe(BaseUris.First());
        }

        [Test]
        public void the_match_has_the_correct_relative_path_segments()
        {
            GivenAMatching("weather/{state}/{city}", "http://localhost/weather/Washington/Seattle");
            ThenTheMatch.RelativePathSegments
                .ShouldHaveSameElementsAs(new[] {"weather", "Washington", "Seattle"});
        }

        [Test]
        public void the_match_has_the_correct_segment_variables()
        {
            GivenAMatching("/weather/{state}/{city}", "http://localhost/weather/Washington/Seattle");

            ThenTheMatch.PathSegmentVariables
                .ShouldHaveSameElementsAs(new NameValueCollection().With("STATE", "Washington").With("city", "Seattle"));
        }

        [Test]
        public void the_match_includes_dots()
        {
            GivenAMatching("/users/{username}", "http://localhost/users/sebastien.lambla");
            ThenTheMatch.PathSegmentVariables.ShouldBe(new NameValueCollection().With("username", "sebastien.lambla"));
        }

        [Test]
        public void the_template_matches_when_in_a_virtual_directory()
        {
            GivenAMatching("http://localhost/vdir/", "/test/", "http://localhost/vdir/test/");
            ThenTheMatch.ShouldNotBeNull();
        }
        [Test]
        public void the_template_matches_when_in_a_virtual_directory_without_trailing_slash()
        {
            GivenAMatching("http://localhost/vdir", "/test/", "http://localhost/vdir/test/");
            ThenTheMatch.ShouldNotBeNull();
        }

        [Test]
        public void there_is_no_match_when_a_segment_doesnt_match()
        {
            GivenAMatching("/weather/{state}/{city}", "http://localhost/temperature/Washington/Seattle");

            ThenTheMatch.ShouldBeNull();
        }

        [Test]
        public void there_is_no_match_with_different_segment_counts()
        {
            GivenAMatching("/weather/{state}/{city}", "http://localhost/nowt");

            ThenTheMatch.ShouldBeNull();
        }
    }

    [TestFixture]
    public class when_binding_by_name : uritemplate_context
    {
        [Test]
        public void a_wildcard_is_not_generated()
        {
            var baseUri = new Uri("http://localhost");
            NameValueCollection variableValues = new NameValueCollection().With("state", "washington").With("CitY",
                                                                                                            "seattle");

            new OpenRasta.UriTemplate("weather/{state}/{city}/*").BindByName(baseUri, variableValues)
                .ShouldBe("http://localhost/weather/washington/seattle/".ToUri());
        }

        [Test]
        public void the_variable_names_are_not_case_sensitive()
        {
            var baseUri = new Uri("http://localhost");
            NameValueCollection variableValues = new NameValueCollection().With("StAte", "washington").With("CitY",
                                                                                                            "seattle");

            new OpenRasta.UriTemplate("weather/{state}/{city}/").BindByName(baseUri, variableValues)
                .ShouldBe("http://localhost/weather/washington/seattle/".ToUri());
        }

        [Test]
        public void the_variables_are_replaced_in_the_generated_uri()
        {
            NameValueCollection variableValues = new NameValueCollection().With("state", "washington").With("city",
                                                                                                            "seattle");

            new OpenRasta.UriTemplate("weather/{state}/{city}/").BindByName("http://localhost".ToUri(), variableValues)
                .ShouldBe("http://localhost/weather/washington/seattle/");
        }
    }

    public class when_matching_querystrings : uritemplate_context
    {
        [Test]
        public void a_query_parameter_with_no_variable_is_ignored()
        {
            var template = new OpenRasta.UriTemplate("/test?query=3");
            template.QueryStringVariableNames.Count.ShouldBe(0);
        }

        [Test]
        public void a_url_matching_result_in_the_query_value_variable_being_set()
        {
            var table = new OpenRasta.UriTemplate("/test?query={queryValue}");
            OpenRasta.UriTemplateMatch match = table.Match(new Uri("http://localhost"), new Uri("http://localhost/test?query=search"));

            match.ShouldNotBeNull();

            match.QueryStringVariables["queryValue"].ShouldBe("search");
        }

        [Test]
        public void a_url_not_matching_a_literal_query_string_will_not_match()
        {
            var table = new OpenRasta.UriTemplate("/test?query=literal");
            OpenRasta.UriTemplateMatch match = table.Match(new Uri("http://localhost"), new Uri("http://localhost/test?query=notliteral"));
            match.ShouldBeNull();
        }

        [Test]
        public void multiple_query_parameters_are_processed()
        {
            var template = new OpenRasta.UriTemplate("/test?query1={test}&query2={test2}");
            template.QueryStringVariableNames.Contains("test").ShouldBeTrue();
            template.QueryStringVariableNames.Contains("test2").ShouldBeTrue();
        }
         [Test]  
        public void a_url_matching_multiple_query_parameters_should_match()  
        {  
           var template = new OpenRasta.UriTemplate("/test?query1={test}&query2={test2}");  
           var match = template.Match(new Uri("http://localhost"), new Uri("http://localhost/test?query1=test1&query2=test2"));  
           match.ShouldNotBeNull();  
        }  
   
        [Test]  
        public void a_parameter_different_by_last_letter_to_query_parameters_should_not_match()  
        {  
           var template = new OpenRasta.UriTemplate("/test?query1={test}&query2={test2}");
           var match = template.Match(new Uri("http://localhost"), new Uri("http://localhost/test?query1=test1&query3=test2"));
           match.ShouldNotBeNull();
           match.PathSegmentVariables.Count.ShouldBe(0);
            match.QueryStringVariables.Count.ShouldBe(1);
           match.QueryParameters.Count.ShouldBe(2);
        }  
   
        [Test]  
        public void more_than_two_query_parameters_with_similar_names_are_processed()  
        {  
           var template = new OpenRasta.UriTemplate("/test?query1={test}&query2={test2}&query3={test3}");  
           template.QueryStringVariableNames.Contains("test").ShouldBeTrue();  
           template.QueryStringVariableNames.Contains("test2").ShouldBeTrue();  
           template.QueryStringVariableNames.Contains("test3").ShouldBeTrue();  
        }  
   
        [Test]  
        public void a_url_matching_three_query_string_parameters_will_match()  
        {  
           var table = new OpenRasta.UriTemplate("/test?q={searchTerm}&p={pageNumber}&s={pageSize}");  
           OpenRasta.UriTemplateMatch match = table.Match(new Uri("http://localhost"), new Uri("http://localhost/test?q=&p=1&s=10"));  
           match.ShouldNotBeNull();  
           match.QueryStringVariables["searchTerm"].ShouldBe(string.Empty);
           match.QueryStringVariables["pageNumber"].ShouldBe("1");
           match.QueryStringVariables["pageSize"].ShouldBe("10");  
        }  
   
        [Test]  
        public void a_url_with_extra_query_string_parameters_will_match()  
        {  
           var template = new OpenRasta.UriTemplate("/test?q={searchTerm}&p={pageNumber}&s={pageSize}");  
           OpenRasta.UriTemplateMatch match = template.Match(new Uri("http://localhost/"), new Uri("http://localhost/test?q=test&p=1&s=10&contentType=json"));  
           match.ShouldNotBeNull();  
        }  
   

        [Test]
        public void the_query_parameters_are_exposed()
        {
            var table = new OpenRasta.UriTemplate("/test?query={queryValue}");
            table.QueryStringVariableNames.Contains("queryValue").ShouldBeTrue();
        }

        [Test]
        public void the_template_matches_when_query_strings_are_not_present()
        {
            var template = new UriTemplate("/temperature?unit={unit}");
            var match = template.Match(new Uri("http://localhost"), new Uri("http://localhost/temperature"));

            match.ShouldNotBeNull();
            match.PathSegmentVariables.Count.ShouldBe(0);
            match.QueryParameters.Count.ShouldBe(1);
        }

        [Test]
        public void the_template_matches_when_query_strings_are_present()
        {
            var template = new UriTemplate("/temperature?unit={unit}");
            var match = template.Match(new Uri("http://localhost"), new Uri("http://localhost/temperature"));

            match.ShouldNotBeNull();
            match.PathSegmentVariables.Count.ShouldBe(0);
            match.QueryParameters.Count.ShouldBe(1);
        }
    }

    [TestFixture]
    public class when_comparing_templates : uritemplate_context
    {
        bool TheResult;

        public void GivenTwoTemplates(string template1, string template2)
        {
            TheResult = new OpenRasta.UriTemplate(template1).IsEquivalentTo(new OpenRasta.UriTemplate(template2));
        }

        [Test]
        public void a_template_isnt_equivalent_to_a_null_reference()
        {
            TheResult = new OpenRasta.UriTemplate("weather/{state}/{city}").IsEquivalentTo(null);

            TheResult.ShouldBeFalse();
        }

        [Test]
        public void AWildcardMappingDoesntMatchANonWildcard()
        {
            GivenTwoTemplates(
                "weather/{state}/*",
                "weather/{state}/something");

            TheResult.ShouldBeFalse();
        }

        [Test]
        public void different_number_of_segments_make_two_templates_not_equivalent()
        {
            GivenTwoTemplates(
                "weather/{state}/{city}",
                "weather/{country}/");

            TheResult.ShouldBeFalse();
        }

        [Test]
        public void templates_with_different_query_strings_are_not_equivalent()
        {
            GivenTwoTemplates(
                "weather/{state}/{city}?forecast={day}&temp=1",
                "weather/{state}/{city}?forecast={day}");
            TheResult.ShouldBeFalse();
        }

        [Test]
        public void the_literal_path_segments_must_match()
        {
            GivenTwoTemplates(
                "weather/{state}/test",
                "weather/{state}/test2");
            TheResult.ShouldBeFalse();
        }

        [Test]
        public void the_preceding_slash_character_is_ignored()
        {
            GivenTwoTemplates(
                "weather/{state}/{city}?forecast={day}",
                "/weather/{country}/{village}?forecast={type}");
            TheResult.ShouldBeTrue();
        }

        [Test]
        public void the_trailing_slash_character_is_ignored()
        {
            GivenTwoTemplates(
                "weather/{state}/{city}",
                "weather/{country}/{village}/");
            TheResult.ShouldBeTrue();
        }

        [Test]
        public void the_trailing_slash_character_is_ignored_after_a_wildcard()
        {
            GivenTwoTemplates(
                "weather/{state}/*",
                "weather/{state}/*/");
            TheResult.ShouldBeFalse();
        }
    }
}

#region Full license

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#endregion
