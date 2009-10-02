using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using OpenRasta.Codecs.WebForms;
using OpenRasta.Testing;

namespace PageParser_Specification
{
    public class when_parsing_aspx_page_inherit_attributes : context
    {
        [TestCase("MasterView(HomeResource)", typeof (MasterPageView<HomeResource>))]
        [TestCase("ResourceView(HomeResource)", typeof (ResourceView<HomeResource>))]
        [TestCase("ResourceSubView(HomeResource)", typeof (ResourceSubView<HomeResource>))]
        [TestCase("ResourceSubView(IList(HomeResource))", typeof (ResourceSubView<IList<HomeResource>>))]
        [TestCase("Unknown(HomeResource)", null, TestName = "Unknown friendly names are not recognized")]
        public void friendlyname_masterview_with_generic_type_is_rewritten(string typeName, Type expectedType)
        {
            OpenRastaPageParserFilter.GetTypeFromFriendlyType(typeName, new[] {typeof (HomeResource).Namespace, typeof (IList<>).Namespace})
                .ShouldBe(expectedType);
        }

        [TestCase("MasterView", typeof (MasterPageView))]
        [TestCase("ResourceView", typeof (ResourceView))]
        [TestCase("ResourceSubView", typeof (ResourceSubView))]
        public void friendlyname_without_generic_type_is_rewritten(string typeName, Type expectedType)
        {
            OpenRastaPageParserFilter.GetTypeFromFriendlyType(typeName, new[] {typeof (HomeResource).Namespace})
                .ShouldBe(expectedType);
        }

        [TestCase("OpenRasta.Codecs.WebForms.MasterPageView<HomeResource>", typeof (MasterPageView<HomeResource>))]
        [TestCase("OpenRasta.Codecs.WebForms.ResourceView<HomeResource>", typeof (ResourceView<HomeResource>))]
        [TestCase("OpenRasta.Codecs.WebForms.ResourceSubView<HomeResource>", typeof (ResourceSubView<HomeResource>))]
        [TestCase("IList<HomeResource>", typeof (IList<HomeResource>))]
        [TestCase("DarthVador(Of StarWars)", null, TestName = "Unknown names return null")]
        public void csharp_names_are_rewritten(string typeName, Type expectedType)
        {
            OpenRastaPageParserFilter.GetTypeFromCSharpType(typeName, new[] {typeof (HomeResource).Namespace, typeof (IList<>).Namespace})
                .ShouldBe(expectedType);
        }

        [TestCase("MasterView", typeof (MasterPageView))]
        [TestCase("ResourceView", typeof (ResourceView))]
        [TestCase("ResourceSubView", typeof (ResourceSubView))]
        [TestCase("MasterView(HomeResource)", typeof (MasterPageView<HomeResource>))]
        [TestCase("ResourceView(HomeResource)", typeof (ResourceView<HomeResource>))]
        [TestCase("ResourceSubView(HomeResource)", typeof (ResourceSubView<HomeResource>))]
        [TestCase("ResourceSubView(IList(HomeResource))", typeof (ResourceSubView<IList<HomeResource>>))]
        [TestCase("Unknown(HomeResource)", null,
            TestName = "Unknown friendly names are not recognized",
            ExpectedException = typeof (TypeLoadException))]
        [TestCase("OpenRasta.Codecs.WebForms.MasterPageView<HomeResource>", typeof (MasterPageView<HomeResource>))]
        [TestCase("OpenRasta.Codecs.WebForms.ResourceView<HomeResource>", typeof (ResourceView<HomeResource>))]
        [TestCase("OpenRasta.Codecs.WebForms.ResourceSubView<HomeResource>", typeof (ResourceSubView<HomeResource>))]
        [TestCase("IList<HomeResource>", typeof (IList<HomeResource>))]
        [TestCase("DarthVador(Of StarWars)", null,
            TestName = "Unknown names return null",
            ExpectedException = typeof (TypeLoadException))]
        public void the_inherits_attribute_is_rewritten(string typeName, Type expectedType)
        {
            var filter = new OpenRastaPageParserFilter();
            filter.PreprocessDirective("import", new Hashtable {{"namespace", typeof (HomeResource).Namespace}});
            filter.PreprocessDirective("import", new Hashtable {{"namespace", typeof (IList<>).Namespace}});
            string rewritten = filter.ParseInheritsAttribute(typeName);

            rewritten.ShouldBe(expectedType.AssemblyQualifiedName);
        }
    }
    public class when_parsing_the_page_title_attribute:context
    {
        [Test]
        public void the_title_is_renamed_PageTitle()
        {
            var filter = new OpenRastaPageParserFilter();
            var attributes = new Hashtable {{"Title", "page's title"},{"Inherits","MasterView"}};
            filter.PreprocessDirective("Page",attributes);
            attributes.ContainsKey("Title")
                .ShouldBeFalse();
            attributes["PageTitle"].ShouldBe("page's title");
        }
    }

    public class HomeResource
    {
    }
}