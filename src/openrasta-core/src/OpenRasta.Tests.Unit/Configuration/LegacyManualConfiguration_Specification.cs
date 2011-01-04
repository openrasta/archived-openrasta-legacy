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
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using OpenRasta.Codecs;
using OpenRasta.Configuration;
using OpenRasta.DI;
using OpenRasta.Testing;
using OpenRasta.Tests.Unit.Configuration;
using OpenRasta.TypeSystem;
using OpenRasta.Web;

namespace LegacyManualConfiguration_Specification
{
    public class when_adding_uris_to_a_resource : configuration_context
    {
        void ThenTheUriHasTheResource<TResource>(string uri, CultureInfo language, string name)
        {
            var match = DependencyManager.Uris.Match(new Uri(new Uri("http://localhost/", UriKind.Absolute), uri));
            match.ShouldNotBeNull();
            match.UriCulture.ShouldBe(language);
            match.ResourceKey.ShouldBe(TypeSystems.Default.FromClr(typeof(TResource)));
            match.UriName.ShouldBe(name);
        }

        [Test]
        public void language_and_names_are_properly_registered()
        {
            GivenAResourceRegistrationFor<Customer>("/customer")
                .InLanguage("fr").Named("French");

            WhenTheConfigurationIsFinished();

            ThenTheUriHasTheResource<Customer>("/customer", CultureInfo.GetCultureInfo("fr"), "French");
        }

        [Test]
        public void registering_two_urls_works()
        {
            GivenAResourceRegistrationFor<Customer>("/customer/{id}")
                .InLanguage("en-CA")
                .AndAt("/privileged/customer/{id}").Named("Privileged");

            WhenTheConfigurationIsFinished();

            ThenTheUriHasTheResource<Customer>("/customer/{id}", CultureInfo.GetCultureInfo("en-CA"), null);
            ThenTheUriHasTheResource<Customer>("/privileged/customer/{id}", null, "Privileged");
        }

        [Test]
        public void the_base_uri_is_registered_for_that_resource()
        {
            GivenAResourceRegistrationFor<Customer>("/customer");

            WhenTheConfigurationIsFinished();

            ThenTheUriHasTheResource<Customer>("/customer", null, null);
        }
    }

    public class when_configuring_codecs : configuration_context
    {
        CodecRegistration ThenTheCodecFor<TResource, TCodec>(string mediaType)
        {
            return
                DependencyManager.Codecs.Where(codec => codec.ResourceType.CompareTo(TypeSystems.Default.FromClr(typeof (TResource)))==0 && codec.CodecType == typeof (TCodec) && codec.MediaType.MediaType == mediaType).
                    Distinct().SingleOrDefault();
        }

        [MediaType("application/vnd.rasta.test")]
        class Codec : NakedCodec
        {
        }

        [MediaType("application/vnd.rasta.test1")]
        [MediaType("application/vnd.rasta.test2")]
        class MultiCodec : NakedCodec
        {
        }

        class NakedCodec : ICodec
        {
            public object Configuration { get; set; }
        }

        [Test]
        public void a_codec_registered_with_configuration_media_type_doesnt_have_the_attribute_media_type_registered()
        {
            GivenAResourceRegistrationFor<Customer>("/customer")
                .HandledBy<CustomerHandler>()
                .AndTranscodedBy<Codec>()
                .ForMediaType("application/vnd.rasta.custom");

            WhenTheConfigurationIsFinished();

            ThenTheCodecFor<Customer, Codec>("application/vnd.rasta.test")
                .ShouldBeNull();
            ThenTheCodecFor<Customer, Codec>("application/vnd.rasta.custom")
                .ShouldNotBeNull();
        }

        [Test]
        public void a_codec_registered_with_two_media_type_attributes_is_registered_twice()
        {
            GivenAResourceRegistrationFor<Customer>("/customer")
                .HandledBy<CustomerHandler>()
                .AndTranscodedBy<MultiCodec>();

            WhenTheConfigurationIsFinished();

            ThenTheCodecFor<Customer, MultiCodec>("application/vnd.rasta.test1")
                .ShouldNotBeNull();
            ThenTheCodecFor<Customer, MultiCodec>("application/vnd.rasta.test2")
                .ShouldNotBeNull();
        }

        [Test]
        public void a_codec_registered_with_two_media_types_in_configuration_is_registered_twice()
        {
            GivenAResourceRegistrationFor<Customer>("/customer")
                .HandledBy<CustomerHandler>()
                .AndTranscodedBy<Codec>()
                .ForMediaType("application/vnd.rasta.config1")
                .AndForMediaType("application/vnd.rasta.config2");

            WhenTheConfigurationIsFinished();

            ThenTheCodecFor<Customer, Codec>("application/vnd.rasta.config1")
                .ShouldNotBeNull();
            ThenTheCodecFor<Customer, Codec>("application/vnd.rasta.config2")
                .ShouldNotBeNull();
        }

        [Test]
        public void a_codec_registered_without_media_types_is_registered_with_the_default_attributed_media_types()
        {
            GivenAResourceRegistrationFor<Customer>("/customer")
                .HandledBy<CustomerHandler>()
                .AndTranscodedBy<Codec>();

            WhenTheConfigurationIsFinished();

            ThenTheCodecFor<Customer, Codec>("application/vnd.rasta.test")
                .ShouldNotBeNull();
        }

        [Test]
        public void registering_a_codec_without_media_type_in_config_or_in_attributes_raises_an_error()
        {
            GivenAResourceRegistrationFor<Customer>("/customer")
                .HandledBy<CustomerHandler>()
                .AndTranscodedBy<NakedCodec>();

            Executing(WhenTheConfigurationIsFinished)
                .ShouldThrow<OpenRastaConfigurationException>();
        }
    }

    public class when_adding_handlers : configuration_context
    {
        IType ThenTheUriHasTheHandler<THandler>(string uri)
        {
            var urimatch = DependencyManager.Uris.Match(new Uri(new Uri("http://localhost/", UriKind.Absolute), uri));
            urimatch.ShouldNotBeNull();

            var handlerMatch = DependencyManager.Handlers.GetHandlerTypesFor(urimatch.ResourceKey).FirstOrDefault();
            handlerMatch.ShouldNotBeNull();
            handlerMatch.ShouldBe(TypeSystems.Default.FromClr(typeof(THandler)));
            return handlerMatch;
        }

        [Test]
        public void the_handler_is_registered()
        {
            GivenAResourceRegistrationFor<Customer>("/customer")
                .HandledBy<CustomerHandler>();

            WhenTheConfigurationIsFinished();

            ThenTheUriHasTheHandler<CustomerHandler>("/customer");
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