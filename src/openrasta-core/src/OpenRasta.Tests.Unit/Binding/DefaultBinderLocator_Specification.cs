using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using OpenRasta.Binding;
using OpenRasta.Testing;
using OpenRasta.TypeSystem;

namespace DefaultBinderLocator_Specification
{
    [TestFixture]
    public class when_building_binders_from_static_methods
    {
        [Test]
        public void the_binder_is_created_correctly()
        {
            var type = TypeSystems.Default.FromClr(typeof(ClassWithStaticBinder));
            var binderLocator = new DefaultObjectBinderLocator();

            binderLocator.GetBinder(type).ShouldBe(ClassWithStaticBinder.StaticBinder);
        }
        private class ClassWithStaticBinder
        {
            public static IObjectBinder StaticBinder = new Mock<IObjectBinder>().Object;
            public static IObjectBinder GetBinder(ITypeSystem typeSystem, IMember member)
            {
                return StaticBinder;
            }
        }
    }
}
