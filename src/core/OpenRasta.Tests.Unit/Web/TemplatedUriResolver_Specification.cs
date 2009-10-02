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
using NUnit.Framework;
using OpenRasta.Testing;
using OpenRasta.Tests.Unit.Fakes;
using OpenRasta.Web;

namespace TemplatedUriResolver_Specification
{
    [TestFixture]
    public class when_binding_uris_by_type : context
    {
        [SetUp]
        public void before_each_behavior()
        {
            ThenTheResult = null;
            Resolver = new TemplatedUriResolver();
        }
        TemplatedUriResolver Resolver;

        Uri ThenTheResult;

        void WhenResolvingVariablesFor<T1>(NameValueCollection nameValueCollection)
        {
            ThenTheResult = Resolver.CreateUriFor(new Uri("http://localhost"), typeof (T1), nameValueCollection);
        }
        void WhenResolvingVariablesFor<T1>(string uriName, NameValueCollection nameValueCollection)
        {
            ThenTheResult = Resolver.CreateUriFor(new Uri("http://localhost"), typeof(T1), uriName,nameValueCollection);
        }
        void GivenThatTheResolverHas(string uri, Type type, CultureInfo cultureInfo, string alias) { Resolver.AddUriMapping(uri, type, cultureInfo, alias); }

        [Test]
        public void the_correct_values_replace_the_variable_declarations()
        {
            GivenThatTheResolverHas("/test2/{variable2}", typeof(object), CultureInfo.CurrentCulture, null);
            GivenThatTheResolverHas("/test/{variable1}", typeof(object), CultureInfo.CurrentCulture, null);

            WhenResolvingVariablesFor<object>(new NameValueCollection {{"variable1", "injected1"}});

            ThenTheResult.ToString().ShouldBe("http://localhost/test/injected1");
        }

        [Test]
        public void the_generated_uri_is_correct_for_generic_types()
        {
            GivenThatTheResolverHas("/test/{variable1}", typeof (IList<object>), CultureInfo.CurrentCulture, null);

            WhenResolvingVariablesFor<IList<object>>(new NameValueCollection {{"variable1", "injected1"}});

            ThenTheResult.ToString().ShouldBe("http://localhost/test/injected1");
        }

        [Test]
        public void the_generated_uri_is_correct_when_injecting_in_query_string_variables()
        {
            GivenThatTheResolverHas("/test?query={variable1}", typeof (IList<object>), CultureInfo.CurrentCulture,
                                    null);

            WhenResolvingVariablesFor<IList<object>>(new NameValueCollection {{"variable1", "injected1"}});

            ThenTheResult.ToString().ShouldBe("http://localhost/test?query=injected1");
        }
        [Test]
        public void a_uri_by_name_is_found()
        {
            GivenThatTheResolverHas("/location1", typeof(IConvertible), CultureInfo.CurrentCulture, null);
            GivenThatTheResolverHas("/location2", typeof(IConvertible), CultureInfo.CurrentCulture, "location2");

            WhenResolvingVariablesFor<IConvertible>("location2",null);

            ThenTheResult.ShouldBe("http://localhost/location2");
        }

        [Test]
        public void uris_with_names_are_not_selected_by_default()
        {
            GivenThatTheResolverHas("/location1", typeof(IConvertible), CultureInfo.CurrentCulture, null);
            GivenThatTheResolverHas("/location2", typeof(IConvertible), CultureInfo.CurrentCulture, "location2");

            WhenResolvingVariablesFor<IConvertible>(null);

            ThenTheResult.ShouldBe("http://localhost/location1");
        }
        [Test]
        public void uris_with_non_matching_templates_because_the_nvc_is_null_are_ignored()
        {
            GivenThatTheResolverHas("/theshire", typeof(Frodo), CultureInfo.InvariantCulture, null);
            GivenThatTheResolverHas("/theshire/{housename}", typeof(Frodo), CultureInfo.InvariantCulture, null);

            WhenResolvingVariablesFor<Frodo>(null);

            ThenTheResult.ShouldBe("http://localhost/theshire");
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