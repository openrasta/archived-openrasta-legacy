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
using System.Globalization;
using HandlerRepository_Specification;
using NUnit.Framework;
using OpenRasta;
using OpenRasta.Testing;
using OpenRasta.Tests.Unit.Fakes;
using OpenRasta.TypeSystem;
using OpenRasta.Web;

namespace TemplatedUriResolver_Specification
{
    public class matching_uris : context.templated_uri_resolver_context
    {
        UriRegistration matching_result;

        [Test]
        public void https_uris_are_supported()
        {
            given_uri_mapping("/Valinor/Olorin", typeof(Gandalf), CultureInfo.CurrentCulture, null);
            when_matching_uri("https://localhost/Valinor/Olorin");

            var resourceKey = matching_result
                .ShouldNotBeNull()
                .ResourceKey;
            resourceKey.ShouldBeOfType<IType>()
                .Equals<Gandalf>().ShouldBeTrue();
        }

        void when_matching_uri(string uri)
        {
            matching_result = Resolver.Match(new Uri(uri));
        }
    }
    [TestFixture]
    public class creating_uris : context.templated_uri_resolver_context
    {
        [Test]
        public void a_uri_by_name_is_found()
        {
            given_uri_mapping("/location1", typeof(IConvertible), CultureInfo.CurrentCulture, null);
            given_uri_mapping("/location2", typeof(IConvertible), CultureInfo.CurrentCulture, "location2");

            when_creating_uri<IConvertible>("location2", null);

            Result.ShouldBe("http://localhost/location2");
        }


        [Test]
        public void the_correct_values_replace_the_variable_declarations()
        {
            given_uri_mapping("/test2/{variable2}", typeof(object), CultureInfo.CurrentCulture, null);
            given_uri_mapping("/test/{variable1}", typeof(object), CultureInfo.CurrentCulture, null);

            when_creating_uri<object>(new NameValueCollection { { "variable1", "injected1" } });

            Result.ToString().ShouldBe("http://localhost/test/injected1");
        }

        [Test]
        public void the_generated_uri_is_correct_for_generic_types()
        {
            given_uri_mapping("/test/{variable1}", typeof(IList<object>), CultureInfo.CurrentCulture, null);

            when_creating_uri<IList<object>>(new NameValueCollection { { "variable1", "injected1" } });

            Result.ToString().ShouldBe("http://localhost/test/injected1");
        }

        [Test]
        public void the_generated_uri_is_correct_when_injecting_in_query_string_variables()
        {
            given_uri_mapping("/test?query={variable1}",
                                    typeof(IList<object>),
                                    CultureInfo.CurrentCulture,
                                    null);

            when_creating_uri<IList<object>>(new NameValueCollection { { "variable1", "injected1" } });

            Result.ToString().ShouldBe("http://localhost/test?query=injected1");
        }

        [Test]
        public void uris_with_names_are_not_selected_by_default()
        {
            given_uri_mapping("/location1", typeof(IConvertible), CultureInfo.CurrentCulture, null);
            given_uri_mapping("/location2", typeof(IConvertible), CultureInfo.CurrentCulture, "location2");

            when_creating_uri<IConvertible>(null);

            Result.ShouldBe("http://localhost/location1");
        }

        [Test]
        public void uris_with_non_matching_templates_because_the_nvc_is_null_are_ignored()
        {
            given_uri_mapping("/theshire", typeof(Frodo), CultureInfo.InvariantCulture, null);
            given_uri_mapping("/theshire/{housename}", typeof(Frodo), CultureInfo.InvariantCulture, null);

            when_creating_uri<Frodo>(null);

            Result.ShouldBe("http://localhost/theshire");
        }

        [Test]
        public void uris_are_generated_correctly_when_base_uri_has_trailing_slash()
        {
            given_uri_mapping("/theshire", typeof(Frodo), CultureInfo.InvariantCulture, null);

            when_creating_uri<Frodo>("http://localhost/lotr/".ToUri(), null);

            Result.ShouldBe("http://localhost/lotr/theshire");
        }

        [Test]
        public void uris_are_generated_correctly_when_base_uri_hasnt_got_trailing_slash()
        {
            given_uri_mapping("/theshire", typeof(Frodo), CultureInfo.InvariantCulture, null);

            when_creating_uri<Frodo>("http://localhost/lotr".ToUri(), null);

            Result.ShouldBe("http://localhost/lotr/theshire");
        }

        [Test]
        public void uris_are_generated_correctly_for_minimum_query_fit()
        {
            given_uri_mapping("/theshire/{character}", typeof(Frodo), CultureInfo.InvariantCulture, null);
            given_uri_mapping("/theshire{character}?q={query}", typeof(Frodo), CultureInfo.InvariantCulture, null);

            when_creating_uri<Frodo>(new NameValueCollection{{"character", "frodo"}});

            Result.ShouldBe("http://localhost/theshire/frodo");
        }
    }
    namespace context
    {
        public class templated_uri_resolver_context : OpenRasta.Testing.context
        {

            protected TemplatedUriResolver Resolver;

            protected Uri Result;
            ITypeSystem TypeSystem;

            public templated_uri_resolver_context()
            {
                TypeSystem = TypeSystems.Default;
            }
            [SetUp]
            public void before_each_behavior()
            {
                Result = null;
                Resolver = new TemplatedUriResolver();
            }
            protected void when_creating_uri<T>(Uri baseUri, NameValueCollection templateParameters)
            {
                Result = Resolver.CreateUriFor(baseUri, typeof(T), templateParameters);
            }

            protected void given_uri_mapping(string uri, Type type, CultureInfo cultureInfo, string alias)
            {
                Resolver.Add(new UriRegistration(uri,  TypeSystem.FromClr(type), alias, cultureInfo));
            }

            protected void when_creating_uri<T1>(NameValueCollection nameValueCollection)
            {
                Result = Resolver.CreateUriFor(new Uri("http://localhost"), typeof(T1), nameValueCollection);
            }

            protected void when_creating_uri<T1>(string uriName, NameValueCollection nameValueCollection)
            {
                Result = Resolver.CreateUriFor(new Uri("http://localhost"), typeof(T1), uriName, nameValueCollection);
            }
        }
    }
}

#region Full license
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion
