using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormElement_Specification;
using NUnit.Framework;
using OpenRasta.Testing;
using OpenRasta.Web.Markup;
using OpenRasta.Web.Markup.Modules;

namespace IIframeModule_Specification
{
    public class when_setting_frameborder : markup_element_context<IIFrameElement>
    {
        [Test]
        public void the_border_is_set_to_0_when_the_attribute_value_is_false()
        {
            WhenCreatingElement(()=>Document.CreateElement<IIFrameElement>().FrameBorder(false));
            ThenTheElementAsString.ShouldContain("frameborder=\"0\"");
        }
        [Test]
        public void the_border_is_set_to_1_when_the_attribute_value_is_true()
        {
            WhenCreatingElement(() => Document.CreateElement<IIFrameElement>().FrameBorder(true));
            ThenTheElementAsString.ShouldContain("frameborder=\"1\"");
        }
        [Test]
        public void the_boreder_is_not_set_by_default()
        {
            WhenCreatingElement(()=>Document.CreateElement<IIFrameElement>());
            ThenTheElementAsString.ShouldNotContain("frameborder=");
        }
    }
    public class when_setting_scrolling : markup_element_context<IIFrameElement>
    {
        [Test]
        public void the_scrolling_is_not_rendered_by_default()
        {
            WhenCreatingElement(() => Document.CreateElement<IIFrameElement>());
            ThenTheElementAsString.ShouldNotContain("scrolling=");
        }

        [Test]
        public void the_scrolling_is_rendered_as_yes_when_the_yes_value_is_used()
        {
            WhenCreatingElement(() => Document.CreateElement<IIFrameElement>().Scrolling(Scrolling.Yes));
            ThenTheElementAsString.ShouldContain(@"scrolling=""yes""");
        }
    }
}
