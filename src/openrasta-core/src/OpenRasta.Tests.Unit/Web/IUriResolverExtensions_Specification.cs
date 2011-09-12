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
using System.Collections.Specialized;
using System.Globalization;
using Moq;
using NUnit.Framework;
using OpenRasta.DI;
using OpenRasta.Testing;
using OpenRasta.Tests.Unit.Fakes;
using OpenRasta.Web;

namespace IUriResolverExtensions_Specification
{
    public class templated_uri_resolver : context
    {
        protected Uri ThenTheUri;
        IUriResolver UriResolver;

        public Mock<ICommunicationContext> GivenContext()
        {
            var contextMock = new Mock<ICommunicationContext>();
            DependencyManager.GetService<IDependencyResolver>().AddDependencyInstance(typeof(ICommunicationContext),
                                                                                      contextMock.Object);

            return contextMock;
        }

        public void GivenUriMapping(object key, string uri, CultureInfo culture, string uriName)
        {
            UriResolver.AddUriMapping(uri, key, culture, uriName);
        }

        public void WhenCreatingUriFor<TResource>()
        {
            ThenTheUri = UriResolver.CreateUriFor(typeof(TResource));
        }

        protected override void SetUp()
        {
            base.SetUp();
            var resolver = new InternalDependencyResolver();
            DependencyManager.SetResolver(resolver);
            resolver.AddDependency<IUriResolver, TemplatedUriResolver>();
            resolver.AddDependencyInstance<IDependencyResolver>(resolver);
            UriResolver = DependencyManager.GetService<IUriResolver>();
        }
    }

    public class when_creatig_uris_for_resource_strings
    {
    }

    [TestFixture]
    public class when_creating_uris_for_objects : templated_uri_resolver
    {
        [Test]
        public void the_uri_cannot_be_created_from_a_null_object_instance()
        {
            GivenContext();
            GivenUriMapping<Customer>("/customer", null, null);
            Executing(() =>
                      ((Customer)null).CreateUri(null, null)
                )
                .ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void the_uri_is_created_based_on_the_named_values_and_the_runtime_type()
        {
            GivenContext();
            GivenUriMapping<Customer>("/customer/{firstname}", null, null);

            new Customer().CreateUri(new Uri("http://localhost"),
                                     new NameValueCollection { { "firstname", "John" } })
                .ShouldBe(new Uri("http://localhost/customer/John"));
        }

        [Test]
        public void the_uri_is_created_based_on_the_public_properties_and_the_runtime_type()
        {
            GivenContext();
            GivenUriMapping<Customer>("/customer/{firstname}", null, null);

            new Customer { FirstName = "John" }.CreateUri(new Uri("http://localhost"))
                .ShouldBe(new Uri("http://localhost/customer/John"));
        }

        [Test]
        public void the_uri_is_created_based_on_the_runtime_type_of_the_object()
        {
            GivenContext();
            GivenUriMapping<Customer>("/customer", null, null);

            var customer = new Customer();

            customer.CreateUri(new Uri("http://localhost"), null)
                .ShouldBe(new Uri("http://localhost/customer"));
        }
        [Test]
        public void the_correct_uri_is_selected_for_a_type()
        {
            GivenContext();
            GivenUriMapping<Frodo>("/theshrine", null, null);
            GivenUriMapping<Sauron>("/mordor", null, null);

            typeof(Frodo).CreateUri(new Uri("http://localhost")).ShouldBe("http://localhost/theshrine");
            typeof(Sauron).CreateUri(new Uri("http://localhost")).ShouldBe("http://localhost/mordor");
        }
        [Test]
        public void the_uri_is_created_using_contextual_base_address_from_the_context()
        {
            GivenContext()
                .ExpectGet(c => c.ApplicationBaseUri)
                .Returns(new Uri("http://tempserver"));

            GivenUriMapping<string>("/test", null, null);

            WhenCreatingUriFor<string>();

            ThenTheUri.ShouldBe(new Uri("http://tempserver/test"));
        }

        void GivenUriMapping<T>(string uri, CultureInfo culture, string uriName)
        {
            GivenUriMapping(typeof(T), uri, culture, uriName);
        }

        public class Customer
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
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