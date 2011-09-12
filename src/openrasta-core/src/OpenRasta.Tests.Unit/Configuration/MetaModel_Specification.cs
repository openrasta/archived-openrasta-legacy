using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using OpenRasta.Codecs;
using OpenRasta.Configuration;
using OpenRasta.Configuration.Fluent;
using OpenRasta.Configuration.Fluent.Implementation;
using OpenRasta.Configuration.MetaModel;
using OpenRasta.DI;
using OpenRasta.Testing;
using OpenRasta.Tests.Unit.Fakes;
using OpenRasta.TypeSystem;
using OpenRasta.TypeSystem.ReflectionBased;
using OpenRasta.Web;
using OpenRasta.Web.UriDecorators;

namespace Configuration_Specification
{
    public class configuration_context : context
    {
        protected static IHas ResourceSpaceHas;
        protected static IUses ResourceSpaceUses;
        protected IMetaModelRepository MetaModel;
        protected IDependencyResolver Resolver;

        protected ResourceModel FirstRegistration
        {
            get { return MetaModel.ResourceRegistrations.First(); }
        }

        protected override void SetUp()
        {
            base.SetUp();
            Resolver = new InternalDependencyResolver();
            Resolver.AddDependency<ITypeSystem, ReflectionBasedTypeSystem>();
            MetaModel = new MetaModelRepository(Resolver);
            ResourceSpaceHas = new FluentTarget(Resolver, MetaModel);
            ResourceSpaceUses = new FluentTarget(Resolver, MetaModel);

            DependencyManager.SetResolver(Resolver);
        }

        protected override void TearDown()
        {
            base.TearDown();
            DependencyManager.UnsetResolver();
        }
    }

    public class when_registering_resources : configuration_context
    {
        [Test]
        public void a_resource_by_generic_type_is_registered()
        {
            ResourceSpaceHas.ResourcesOfType<Customer>();

            MetaModel.ResourceRegistrations.Count.ShouldBe(1);
            MetaModel.ResourceRegistrations[0].ResourceKey.ShouldBe(typeof(Customer));
        }

        [Test]
        public void a_resource_by_name_is_registered()
        {
            ResourceSpaceHas.ResourcesNamed("customers");

            MetaModel.ResourceRegistrations[0].ResourceKey.ShouldBe("customers");
        }

        [Test]
        public void a_resource_by_type_is_registered()
        {
            ResourceSpaceHas.ResourcesOfType(typeof(Customer));

            MetaModel.ResourceRegistrations[0].ResourceKey.ShouldBe(typeof(Customer));
        }

        [Test]
        public void a_resource_with_any_key_is_registered()
        {
            var key = new object();
            ResourceSpaceHas.ResourcesWithKey(key);

            MetaModel.ResourceRegistrations[0].ResourceKey.ShouldBeTheSameInstanceAs(key);
        }

        [Test]
        public void a_resource_with_IType_is_registered()
        {
            ResourceSpaceHas.ResourcesOfType(TypeSystems.Default.FromClr(typeof(Customer)));

            MetaModel.ResourceRegistrations[0].ResourceKey.ShouldBeOfType<IType>().Name.ShouldBe("Customer");
        }

        [Test]
        public void cannot_execute_registration_on_null_IHas()
        {
            Executing(() => ((IHas)null).ResourcesWithKey(new object()))
                .ShouldThrow<ArgumentNullException>();
        }
        [Test]
        public void a_resource_of_type_strict_is_registered_as_a_strict_type()
        {
            ResourceSpaceHas.ResourcesOfType<Strictly<Customer>>();

            MetaModel.ResourceRegistrations[0].ResourceKey.ShouldBeOfType<Type>().ShouldBe<Customer>();
            MetaModel.ResourceRegistrations[0].IsStrictRegistration.ShouldBeTrue();

        }
        [Test]
        public void cannot_register_a_resource_with_a_null_key()
        {
            Executing(() => ResourceSpaceHas.ResourcesWithKey(null))
                .ShouldThrow<ArgumentNullException>();
        }
    }

    public class when_registering_uris : configuration_context
    {
        IList<UriModel> TheUris
        {
            get { return MetaModel.ResourceRegistrations[0].Uris; }
        }

        [Test]
        public void a_null_language_defaults_to_the_inviariant_culture()
        {
            ResourceSpaceHas.ResourcesOfType<Customer>().AtUri("/customer").InLanguage(null);
            TheUris[0].Language.ShouldBe(CultureInfo.InvariantCulture);
        }

        [Test]
        public void a_resource_can_be_registered_with_no_uri()
        {
            ICodecParentDefinition reg = ResourceSpaceHas.ResourcesOfType<Customer>().WithoutUri;
            TheUris.Count.ShouldBe(0);
        }

        [Test]
        public void a_uri_is_registered()
        {
            ResourceSpaceHas.ResourcesOfType<Customer>().AtUri("/customer");

            TheUris.Count.ShouldBe(1);
            TheUris[0].Uri.ShouldBe("/customer");
        }

        [Test]
        public void a_uri_language_is_registered()
        {
            ResourceSpaceHas.ResourcesOfType<Customer>().AtUri("/customer").InLanguage("fr");
            TheUris[0].Language.Name.ShouldBe("fr");
        }

        [Test]
        public void a_uri_name_is_registered()
        {
            ResourceSpaceHas.ResourcesOfType<Customer>().AtUri("/customer").Named("default");

            TheUris[0].Name.ShouldBe("default");
        }

        [Test]
        public void can_register_multiple_uris_for_a_resource()
        {
            ResourceSpaceHas.ResourcesOfType<Frodo>()
                .AtUri("/theshire")
                .And
                .AtUri("/lothlorien");

            TheUris.Count.ShouldBe(2);
            TheUris[0].Uri.ShouldBe("/theshire");
            TheUris[1].Uri.ShouldBe("/lothlorien");
        }

        [Test]
        public void cannot_register_a_uri_then_without_a_uri()
        {
            Executing(() =>
                {
                    ICodecParentDefinition reg = ResourceSpaceHas.ResourcesOfType<Frodo>()
                        .AtUri("/theshrine")
                        .And.WithoutUri;
                }).ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void lcannot_register_a_null_uri_for_a_resource()
        {
            Executing(() => ResourceSpaceHas.ResourcesOfType<Customer>().AtUri(null))
                .ShouldThrow<ArgumentNullException>();
        }
    }

    public class when_registring_handlers_for_resources_with_URIs : configuration_context
    {
        [Test]
        public void a_handler_from_generic_type_is_registered()
        {
            ResourceSpaceHas.ResourcesOfType<Frodo>().AtUri("/theshrine")
                .HandledBy<CustomerHandler>();

            FirstRegistration.Handlers[0].Name.ShouldBe("CustomerHandler");
        }

        [Test]
        public void a_handler_from_itype_is_registered()
        {
            ResourceSpaceHas.ResourcesOfType(typeof(Frodo)).AtUri("/theshrine")
                .HandledBy(TypeSystems.Default.FromClr(typeof(CustomerHandler)));
            FirstRegistration.Handlers[0].Name.ShouldBe("CustomerHandler");
        }

        [Test]
        public void a_handler_from_type_instance_is_registered()
        {
            ResourceSpaceHas.ResourcesOfType(typeof(Frodo)).AtUri("/theshrine")
                .HandledBy(typeof(CustomerHandler));
            FirstRegistration.Handlers[0].Name.ShouldBe("CustomerHandler");
        }

        [Test]
        public void cannot_add_a_null_handler()
        {
            Executing(() => ResourceSpaceHas.ResourcesOfType<Frodo>().AtUri("/theshrine").HandledBy((IType)null))
                .ShouldThrow<ArgumentNullException>();
            Executing(() => ResourceSpaceHas.ResourcesOfType<Frodo>().AtUri("/theshrine").HandledBy((Type)null))
                .ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void two_handlers_can_be_added()
        {
            ResourceSpaceHas.ResourcesOfType(typeof(Frodo)).AtUri("/theshrine")
                .HandledBy<CustomerHandler>()
                .And
                .HandledBy<object>();

            FirstRegistration.Handlers[0].Name.ShouldBe("CustomerHandler");
            FirstRegistration.Handlers[1].Name.ShouldBe("Object");
        }
    }
    public class when_registering_uri_decorators : configuration_context
    {
        [Test]
        public void a_dependency_is_added_to_the_meta_model()
        {
            ResourceSpaceUses.UriDecorator<TestUriDecorator>();

            MetaModel.CustomRegistrations.OfType<DependencyRegistrationModel>().FirstOrDefault()
                .ShouldNotBeNull()
                .ConcreteType.ShouldBe<TestUriDecorator>();
        }
        public class TestUriDecorator : IUriDecorator
        {
            public bool Parse(Uri uri, out Uri processedUri)
            {
                processedUri = null; return false;
            }

            public void Apply()
            {
            }
        }
    }


    public class when_registering_codecs : configuration_context
    {
        [Test]
        public void can_add_a_codec_by_type()
        {
            ExecuteTest(parent =>
                {
                    parent.TranscodedBy<CustomerCodec>();

                    FirstRegistration.Codecs[0].CodecType.ShouldBe<CustomerCodec>();
                });
        }

        [Test]
        public void can_add_a_codec_by_type_instance()
        {
            ExecuteTest(parent =>
                {
                    parent.TranscodedBy(typeof(CustomerCodec));

                    FirstRegistration.Codecs[0].CodecType.ShouldBe<CustomerCodec>();
                });
        }
        [Test]
        public void can_add_a_codec_configuration()
        {
            ExecuteTest(parent =>
            {
                var configurationObject = new object();
                parent.TranscodedBy(typeof(CustomerCodec),configurationObject);

                FirstRegistration.Codecs[0].Configuration.ShouldBe(configurationObject);
            });
        }
        [Test]
        public void can_add_a_specific_media_type_for_a_codec()
        {
            ExecuteTest(parent =>
                {
                    parent.TranscodedBy<CustomerCodec>().ForMediaType(MediaType.Javascript.ToString());

                    FirstRegistration.Codecs[0].MediaTypes[0].MediaType.ShouldBe(MediaType.Javascript);
                });
        }

        [Test]
        public void can_register_two_media_types()
        {
            ExecuteTest(parent =>
                {
                    parent.TranscodedBy<CustomerCodec>().ForMediaType("application/xhtml+xml").ForMediaType("text/plain");

                    FirstRegistration.Codecs[0].MediaTypes[0].MediaType.ToString().ShouldBe("application/xhtml+xml");
                    FirstRegistration.Codecs[0].MediaTypes[1].MediaType.ToString().ShouldBe("text/plain");
                });
        }
        [Test]
        public void can_register_an_extension_on_mediatype()
        {
            ExecuteTest(parent =>
            {
                parent.TranscodedBy<CustomerCodec>().ForMediaType("application/xhtml+xml").ForExtension("xml");

                FirstRegistration.Codecs[0].MediaTypes[0]
                    .Extensions.ShouldContain("xml")
                    .Count().ShouldBe(1);
            });
        }

        [Test]
        public void can_register_multiple_extensions_on_multiple_mediatypes()
        {
            ExecuteTest(parent =>
            {
                parent.TranscodedBy<CustomerCodec>()
                    .ForMediaType(MediaType.Xhtml).ForExtension("xml").ForExtension("xhtml")
                    .ForMediaType("text/html").ForExtension("html");

                FirstRegistration.Codecs[0].MediaTypes[0]
                    .Extensions
                        .ShouldContain("xml")
                        .ShouldContain("xhtml")
                        .Count().ShouldBe(2);
                FirstRegistration.Codecs[0].MediaTypes[1]
                    .Extensions
                        .ShouldContain("html")
                        .Count().ShouldBe(1);
            });
        }
        [Test]
        public void can_register_multiple_codecs_with_multiple_media_types_and_multiple_extensions()
        {
            ExecuteTest(parent =>
            {
                parent
                    .TranscodedBy<CustomerReaderCodec>()
                    .And
                    .TranscodedBy<CustomerCodec>()
                    .ForMediaType("application/xhtml+xml").ForExtension("xml").ForExtension("xhtml")
                    .And
                    .TranscodedBy<CustomerWriterCodec>()
                    .ForMediaType("application/unknown");

                FirstRegistration.Codecs[0].CodecType.ShouldBe<CustomerReaderCodec>();

                FirstRegistration.Codecs[1].CodecType.ShouldBe<CustomerCodec>();
                FirstRegistration.Codecs[1].MediaTypes[0]
                    .Extensions
                        .ShouldContain("xml")
                        .ShouldContain("xhtml")
                        .Count().ShouldBe(2);

                FirstRegistration.Codecs[2].CodecType.ShouldBe<CustomerWriterCodec>();
                FirstRegistration.Codecs[2].MediaTypes[0]
                    .Extensions
                        .Count().ShouldBe(0);
            });
        }
        [Test]
        public void cannot_register_codec_not_implementing_icodec()
        {
            Executing(() => ResourceSpaceHas.ResourcesOfType<Frodo>().WithoutUri.TranscodedBy(typeof(string)))
                .ShouldThrow<ArgumentOutOfRangeException>();
        }
        void ExecuteTest(Action<ICodecParentDefinition> test)
        {
            test(ResourceSpaceHas.ResourcesOfType<Frodo>().AtUri("/theshrine").HandledBy<CustomerHandler>());
            MetaModel.ResourceRegistrations.Clear();
            test(ResourceSpaceHas.ResourcesOfType<Frodo>().WithoutUri);
        }
    }
    public class when_registering_custom_dependency : configuration_context
    {
        [Test]
        public void a_dependency_registration_is_added_to_the_metamodel()
        {
            ResourceSpaceUses.CustomDependency<IUriResolver, TemplatedUriResolver>(DependencyLifetime.Singleton);

            var first = MetaModel.CustomRegistrations.OfType<DependencyRegistrationModel>().ShouldHaveCountOf(1).First();
            first.ConcreteType.ShouldBe<TemplatedUriResolver>();
            first.ServiceType.ShouldBe<IUriResolver>();
            first.Lifetime.ShouldBe(DependencyLifetime.Singleton);

        }
    }
}