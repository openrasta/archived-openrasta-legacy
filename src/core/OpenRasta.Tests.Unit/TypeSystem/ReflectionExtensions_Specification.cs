using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenRasta.Testing;
using OpenRasta.TypeSystem.ReflectionBased;

namespace ReflectionExtensions_Specification
{
    public class when_finding_interfaces : context
    {
        [Test]
        public void an_interface_on_a_type_is_discovered()
        {
            typeof(List<string>).FindInterface(typeof(IList<>))
                .ShouldBe<IList<string>>();
        }
        [Test]
        public void an_interface_on_an_interface_is_discovered()
        {
            typeof(IList<string>).FindInterface(typeof(IEnumerable<>))
                .ShouldBe<IEnumerable<string>>();
        }
        [Test]
        public void an_interface_that_is_the_provided_interface_is_discovered()
        {
            typeof(IList<string>).FindInterface(typeof(IList<>))
                .ShouldBe <IList<string>>();
        }
    }
}
