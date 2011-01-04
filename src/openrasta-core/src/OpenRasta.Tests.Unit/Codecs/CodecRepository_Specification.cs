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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OpenRasta.Codecs;
using OpenRasta.IO;
using OpenRasta.Testing;
using OpenRasta.Tests.Unit.Fakes;
using OpenRasta.TypeSystem;
using OpenRasta.Web;

namespace CodecRepository_Specification
{
    public class when_searching_for__media_type_reader : codec_repository_context
    {
        [Test]
        public void a_codec_for_a_parent_resource_type_is_found()
        {
            GivenACodec<CustomerCodec, object>("application/xml");

            WhenFindingCodec("application/xml", typeof(Customer));

            ThenTheResult.CodecType
                .ShouldBe<CustomerCodec>();
        }

        [Test]
        public void a_codec_for_a_parent_resource_type_with_strict_marker_is_not_found()
        {
            GivenACodec<CustomerCodec, Strictly<object>>("application/xml");

            WhenFindingCodec("application/xml", typeof(Customer));

            ThenTheResult.ShouldBeNull();
        }

        [Test]
        public void a_codec_for_the_exact_resource_type_is_found()
        {
            GivenACodec<CustomerCodec, Strictly<Customer>>("application/xml");

            WhenFindingCodec("application/xml", typeof(Customer));

            ThenTheResult.ShouldNotBeNull()
                .CodecType
                .ShouldBe<CustomerCodec>();
        }

        [Test]
        public void a_codec_that_is_not_registered_for_all_resource_types_is_not_selected()
        {
            GivenACodec<CustomerCodec, Customer>("application/xml");

            WhenFindingCodec("application/xml", typeof(Customer), typeof(Frodo));

            ThenTheResult.ShouldBeNull();
        }

        [Test]
        public void a_wildcard_codec_is_not_selected_wheN_another_codec_has_matching_media_type()
        {
            GivenACodec<ApplicationOctetStreamCodec, IFile>("*/*;q=0.5");
            GivenACodec<MultipartFormDataObjectCodec, IFile>("multipart/form-data;q=0.5");

            WhenFindingCodec("multipart/form-data", typeof(IFile));

            ThenTheResult.CodecType.ShouldBe<MultipartFormDataObjectCodec>();
        }

        [Test]
        public void a_wildcard_codec_is_selected_if_the_destination_type_is_closest_to_the_param_type()
        {
            GivenACodec<ApplicationOctetStreamCodec, IFile>("*/*;q=0.4");
            GivenACodec<TextPlainCodec, string>("text/plain;q=0.5");

            WhenFindingCodec("text/plain", typeof(IFile));

            ThenTheResult.CodecType.ShouldBe<ApplicationOctetStreamCodec>();
        }

        [Test]
        public void a_wildcard_selects_the_codec_with_the_highest_quality()
        {
            GivenACodec<CustomerCodec, object>("application/json;q=0.4");
            GivenACodec<AnotherCustomerCodec, object>("application/xml;q=0.3");

            WhenFindingCodec("*/*", typeof(Customer));

            ThenTheResult.CodecType.ShouldBe<CustomerCodec>();
        }

        [Test]
        public void the_codec_with_the_highest_quality_is_selected()
        {
            GivenACodec<CustomerCodec, object>("application/xml;q=0.9", "nonspecific");
            GivenACodec<AnotherCustomerCodec, object>("application/xml", "specific");

            WhenFindingCodec("application/xml", typeof(Customer));

            ThenTheResult.CodecType
                .ShouldBe<AnotherCustomerCodec>();
            ThenTheResult.Configuration
                .ShouldBe("specific");
        }

        [Test]
        public void the_most_specific_codec_for_a_resource_type_is_found()
        {
            GivenACodec<CustomerCodec, object>("application/xml", "nonspecific");
            GivenACodec<CustomerCodec, Customer>("application/xml", "specific");

            WhenFindingCodec("application/xml", typeof(Customer));

            ThenTheResult.CodecType
                .ShouldBe<CustomerCodec>();
            ThenTheResult.Configuration
                .ShouldBe("specific");
        }

        [Test, Ignore]
        public void the_most_specific_codec_is_found_when_matching_against_several_resource_types()
        {
        }
    }

    public class when_registering_a_codec : codec_repository_context
    {
        [Test]
        public void the_same_codec_can_be_registered_for_several_content_types()
        {
        }

        [Test]
        public void the_same_codec_can_be_registered_for_several_resource_types()
        {
        }
    }

    public class when_searching_for_content_type_writers_for_a_media_type : codec_repository_context
    {
        readonly ITypeSystem typeSystem = TypeSystems.Default;
        new IList<CodecRegistration> ThenTheResult;

        [Test]
        public void generic_types_are_found()
        {
            GivenACodec<CustomerCodec, IList<Customer>>("application/xml");

            WhenFindingCodecsFor<IList<Customer>>("application/xml");

            ThenTheResult.Count.ShouldBe(1);
            ThenTheResult[0].CodecType
                .ShouldBe<CustomerCodec>();
        }

        [Test]
        public void inherited_types_are_found()
        {
            GivenACodec<CustomerCodec, object>("application/xml");

            WhenFindingCodecsFor<IList<Customer>>("application/xml");

            ThenTheResult.Count.ShouldBe(1);
            ThenTheResult[0].CodecType
                .ShouldBe<CustomerCodec>();
        }

        [Test]
        public void interface_types_are_found()
        {
            GivenACodec<CustomerCodec, IList<Customer>>("application/xml");

            WhenFindingCodecsFor<List<Customer>>("application/xml");

            ThenTheResult.Count.ShouldBe(1);
            ThenTheResult[0].CodecType
                .ShouldBe<CustomerCodec>();
        }

        [Test]
        public void matching_is_done_against_parent_interfaces()
        {
            GivenACodec<AnotherCustomerCodec, IEnumerable>("text/plain");

            WhenFindingCodecsFor<IList<Customer>>("text/plain");

            ThenTheResult.Count.ShouldBe(1);
            ThenTheResult[0].CodecType
                .ShouldBe<AnotherCustomerCodec>();
        }

        [Test]
        public void object_registrations_are_found_for_interfaces()
        {
            GivenACodec<AnotherCustomerCodec, object>("text/plain");

            WhenFindingCodecsFor<IList<Customer>>("text/plain");

            ThenTheResult.Count.ShouldBe(1);
            ThenTheResult[0].CodecType
                .ShouldBe<AnotherCustomerCodec>();
        }

        [Test]
        public void only_codecs_for_a_compatible_mime_type_are_selected()
        {
            GivenACodec<CustomerCodec, IList<Customer>>("application/xml");
            GivenACodec<AnotherCustomerCodec, IList<Customer>>("text/plain");

            WhenFindingCodecsFor<IList<Customer>>("text/plain");

            ThenTheResult.Count.ShouldBe(1);
            ThenTheResult[0].CodecType
                .ShouldBe<AnotherCustomerCodec>();
        }

        [Test]
        public void the_closest_matching_type_is_selected()
        {
            GivenACodec<CustomerCodec, object>("application/xml;q=0.5");
            GivenACodec<AnotherCustomerCodec, string>("text/plain;q=0.4");

            WhenFindingCodecsFor<string>("*/*");

            ThenTheResult.Count.ShouldBe(2);
            ThenTheResult[0].CodecType.ShouldBe<AnotherCustomerCodec>();
        }

        [Test]
        public void the_server_quality_is_used_in_prioritizing_the_negotiated_media_type()
        {
            GivenACodec<CustomerCodec, object>(
                "application/xhtml+xml;q=0.9,text/html,application/vnd.openrasta.htmlfragment+xml;q=0.5");
            WhenFindingCodecsFor<object>("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");

            ThenTheResult.Count.ShouldBeGreaterThan(0);
            ThenTheResult[0].MediaType.ToString().ShouldBe("text/html");
        }

        protected void WhenFindingCodecsFor<TResourceType>(params string[] contentTypes)
        {
            ThenTheResult =
                Codecs.FindMediaTypeWriter(typeSystem.FromClr(typeof(TResourceType)),
                                           MediaType.Parse(string.Join(",", contentTypes))).ToList();
        }
    }

    public class codec_repository_context : context
    {
        public ICodecRepository Codecs;
        public CodecRegistration ThenTheResult;
        public CodecMatch ThenTheResultScoring;
        public ITypeSystem TypeSystem = TypeSystems.Default;

        [SetUp]
        public void Setup()
        {
            Codecs = new CodecRepository();
            ThenTheResult = null;
        }

        protected void GivenACodec<TCodec, TResource>(string mediaTypes)
        {
            GivenACodec<TCodec, TResource>(mediaTypes, null);
        }

        protected void GivenACodec<TCodec, TResource>(string mediaTypes, string config)
        {
            foreach (var mediaType in MediaType.Parse(mediaTypes))
            {
                Type resourceType = typeof(TResource);

                Codecs.Add(CodecRegistration.FromResourceType(resourceType,
                                                         typeof(TCodec),
                                                         TypeSystems.Default,
                                                         mediaType,
                                                         null,
                                                         config, false));
            }
        }
        protected void WhenFindingCodec(string contentType, params Type[] resourcetypes)
        {
            ThenTheResult = null;
            ThenTheResultScoring = null;
            ThenTheResultScoring = Codecs.FindMediaTypeReader(new MediaType(contentType), resourcetypes.Select(x => (IMember)TypeSystem.FromClr(x)), null);
            if (ThenTheResultScoring == null)
                return;
            ThenTheResult = ThenTheResultScoring.CodecRegistration;
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