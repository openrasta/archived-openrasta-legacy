using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormElement_Specification;
using NUnit.Framework;
using OpenRasta.Testing;
using OpenRasta.Web.Markup;
using OpenRasta.Web.Markup.Modules;

namespace StyleSheetModule_Specifications
{
    public class when_setting_media : markup_element_context<IStyleElement>
    {
        [Test]
        public void the_media_is_not_defined_by_default()
        {
            WhenCreatingElement(Document.CreateElement<IStyleElement>);
            ThenTheElementAsString.ShouldNotContain("media=");
        }
        [Test]
        public void the_media_attribute_is_generated()
        {
            WhenCreatingElement(()=>Document.CreateElement<IStyleElement>().Media("screen"));
            ThenTheElementAsString.ShouldContain(@"media=""screen""");
        }
        [Test]
        public void two_media_attributes_are_generated()
        {
            WhenCreatingElement(() => Document.CreateElement<IStyleElement>().Media("screen").Media("alternate"));
            ThenTheElementAsString.ShouldContain(@"media=""screen,alternate""");
        }
        [Test]
        public void adding_a_media_with_a_comma_results_in_two_media_values()
        {
            WhenCreatingElement(()=>Document.CreateElement<IStyleElement>().Media("screen,alternate"));
            ThenTheElement.Media.Count.ShouldBe(2);
            ThenTheElement.Media.ShouldContain("screen");
            ThenTheElement.Media.ShouldContain("alternate");
        }
    }
}
