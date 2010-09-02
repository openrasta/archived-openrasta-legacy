using System.Collections;
using System.Linq;
using InternalDependencyResolver_Specification;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using OpenRasta.DI;
using OpenRasta.DI.Unity;
using OpenRasta.DI.Unity.Extensions;
using OpenRasta.Testing;

namespace UnityDependencyResolver_Specification
{   
    [TestFixture]
    public class when_resolving_instances_with_the_unity_resolver : when_resolving_instances
    {
        public override IDependencyResolver CreateResolver()
        {
            return new UnityDependencyResolver();
        }

        [Test]
        public void all_registered_dependencies_can_be_resolved()
        {
            Resolver.AddDependency<IFoo, AFoo>();
            Resolver.AddDependency<IFoo, BFoo>();
            Resolver.AddDependency<IFoo, FooBar>();

            var results = Resolver.ResolveAll<IFoo>();

            results.ShouldHaveCountOf(3);
            results.ShouldContainInstanceOf<AFoo>();
            results.ShouldContainInstanceOf<BFoo>();
            results.ShouldContainInstanceOf<FooBar>();
        }

        [Test]
        public void only_matching_dependencies_are_resolved()
        {
            Resolver.AddDependency<IFoo, AFoo>();
            Resolver.AddDependency<IBar, ABar>();

            var results = Resolver.ResolveAll<IFoo>();

            results.ShouldHaveCountOf(1);
            results.ShouldContainInstanceOf<AFoo>();
        }

        [Test]
        public void complex_cycles_generate_an_error()
        {
            Resolver.AddDependency<TriCycleA>();
            Resolver.AddDependency<TriCycleB>();
            Resolver.AddDependency<TriCycleC>();

            Executing(() => Resolver.Resolve<TriCycleA>())
                .ShouldThrow<DependencyResolutionException>();
        }

        [Test]
        public void dependencies_used_multiple_times_do_not_cause_cyclic_errors()
        {
            Resolver.AddDependency<IFoo, AFoo>();
            Resolver.AddDependency<ComplexChild>();
            Resolver.AddDependency<ComplexParent>();

            Resolver.Resolve<ComplexParent>()
                .ShouldNotBeNull();
        }

        [Test]
        public void unregistered_nested_dependencies_cause_an_error()
        {
            Resolver.AddDependency<IFoo, AFoo>();
            Resolver.AddDependency<ComplexParent>();

            Executing(() => Resolver.Resolve<ComplexParent>())
                .ShouldThrow<DependencyResolutionException>();
        }

        [Test]
        public void can_resolve_multiple_times()
        {
            Resolver.AddDependency<AFoo>();
            Resolver.Resolve<AFoo>().ShouldNotBeNull();
            Resolver.Resolve<AFoo>().ShouldNotBeNull();
            Resolver.Resolve<AFoo>().ShouldNotBeNull();
        }

        [Test]
        public void explicit_injection_constructors_are_used()
        {
            Resolver.AddDependency<IFoo, AFoo>();
            Resolver.AddDependency<IBar, ABar>();
            Resolver.AddDependency<MarkedConstructor>();
            Resolver.Resolve<MarkedConstructor>().Constructor.ShouldBe(2);
        }
    }

    public static class MoreSpecExtensions
    {
        public static bool ShouldContainInstanceOf<TService>(this IEnumerable instances)
        {
            return instances
                .AsQueryable()
                .OfType<object>()
                .Any(x => x.GetType() == typeof(TService));
        }
    }

    [TestFixture]
    public class when_registering_dependencies_with_the_unity_resolver : when_registering_dependencies
    {
        public override IDependencyResolver CreateResolver()
        {
            return new UnityDependencyResolver();
        }

        [Test]
        public void unregistered_dependencies_can_not_be_found()
        {
            Resolver.HasDependency<IFoo>().ShouldBe(false);
            Resolver.HasDependency<AFoo>().ShouldBe(false);
        }

        [Test]
        public void abstract_dependencies_can_be_found_by_either_type()
        {
            Resolver.AddDependency<IFoo, AFoo>();
            Resolver.HasDependency<IFoo>().ShouldBe(true);
            Resolver.HasDependency<AFoo>().ShouldBe(true);
        }

        [Test]
        public void concrete_dependencies_can_only_be_found_by_their_concrete_type()
        {
            Resolver.AddDependency<AFoo>();
            Resolver.HasDependency<AFoo>().ShouldBe(true);
            Resolver.HasDependency<IFoo>().ShouldBe(false);
        }

        [Test]
        public void multiple_abstract_dependencies_can_resolve_to_the_same_type()
        {
            Resolver.AddDependency<IFoo, FooBar>();
            Resolver.AddDependency<IBar, FooBar>();
            Resolver.HasDependency<IFoo>().ShouldBe(true);
            Resolver.HasDependency<IFoo>().ShouldBe(true);
        }

        [Test]
        public void only_the_specified_abstract_type_is_registered()
        {
            Resolver.AddDependency<IFoo, FooBar>();
            Resolver.HasDependency<IFoo>().ShouldBe(true);
            Resolver.HasDependency<IBar>().ShouldBe(false);
        }
    }

    [TestFixture]
    public class when_registering_for_per_request_lifetime_with_unity_dependency_resolver :
        when_registering_for_per_request_lifetime
    {
        public override IDependencyResolver CreateResolver()
        {
            return new UnityDependencyResolver();
        }
    }

    public class when_using_a_parent_container_with_unity_dependency_resolver : dependency_resolver_context
    {
        IUnityContainer parent;

        public override IDependencyResolver CreateResolver()
        {
            parent = new UnityContainer();
            parent.AddNewExtension<TypeTracker>();

            return new UnityDependencyResolver(parent);
        }

        [Test]
        public void dependencies_registered_in_parent_are_available_to_child()
        {
            parent.RegisterType<IFoo, AFoo>();
            Resolver.HasDependency<IFoo>().ShouldBe(true);
        }

        [Test]
        public void dependencies_registered_in_child_are_available_to_child()
        {
            Resolver.AddDependency<IFoo, AFoo>();
            Resolver.HasDependency<IFoo>().ShouldBe(true);
        }

        [Test]
        public void dependencies_registered_in_child_are_not_available_to_parent()
        {
            Resolver.AddDependency<IFoo, AFoo>();

            Executing(() => parent.Resolve<IFoo>())
                .ShouldThrow<ResolutionFailedException>();
        }

        [Test]
        public void can_resolve_dependencies_from_both_parent_and_child()
        {
            parent.RegisterType<IFoo, AFoo>();
            Resolver.AddDependency<IFoo, BFoo>();

            var results = Resolver.ResolveAll<IFoo>();

            results.ShouldHaveCountOf(2);
            results.ShouldContainInstanceOf<AFoo>();
            results.ShouldContainInstanceOf<BFoo>();
        }

        [Test]
        public void properties_on_types_in_child_container_are_injected()
        {
            parent.RegisterType<Simple>();
            Resolver.AddDependency<WithProperty>();

            Resolver.Resolve<WithProperty>().Simple.ShouldNotBeNull();
        }

        [Test]
        public void properties_on_types_in_parent_container_are_not_injected()
        {
            parent.RegisterType<Simple>();
            parent.RegisterType<WithProperty>();

            Resolver.Resolve<WithProperty>().Simple.ShouldBeNull();
        }

        [Test]
        public void unregistered_dependencies_cause_error_in_child_container()
        {
            parent.RegisterType<IFoo, AFoo>();
            Resolver.AddDependency<ComplexParent>();

            Executing(() => Resolver.Resolve<ComplexParent>())
                .ShouldThrow<DependencyResolutionException>();
        }

        [Test]
        public void unregistered_dependencies_from_parent_cause_error_child_container()
        {
            parent.RegisterType<IFoo, AFoo>();
            parent.RegisterType<ComplexParent>();

            Executing(() => Resolver.Resolve<ComplexParent>())
                .ShouldThrow<DependencyResolutionException>();
        }

        [Test]
        public void unregistered_dependencies_from_parent_are_resolved_from_parent_container()
        {
            parent.RegisterType<IFoo, AFoo>();
            parent.RegisterType<ComplexParent>();

            parent.Resolve<ComplexParent>().ShouldNotBeNull();
        }

        [Test]
        public void parent_container_uses_longest_constructor()
        {
            parent.RegisterType<IFoo, AFoo>();
            parent.RegisterType<MultiConstructor>();

            Executing(() => parent.Resolve<MultiConstructor>())
                .ShouldThrow<ResolutionFailedException>();
        }

        [Test]
        public void child_container_uses_greediest_constructor()
        {
            parent.RegisterType<IFoo, AFoo>();
            Resolver.AddDependency<MultiConstructor>();

            Resolver.Resolve<MultiConstructor>().Dependencies.ShouldBe(1);
        }
    }

    public class TriCycleA { public TriCycleA(TriCycleB b) { } }
    public class TriCycleB { public TriCycleB(TriCycleC c) { } }
    public class TriCycleC { public TriCycleC(TriCycleA a) { } }

    public interface IFoo { }
    public interface IBar { }

    public class AFoo : IFoo { }
    public class BFoo : IFoo { }

    public class ABar : IBar { }
    public class BBar : IBar { }

    public class FooBar : IFoo, IBar { }

    public class ComplexChild { public ComplexChild(IFoo foo) { } }
    public class ComplexParent { public ComplexParent(ComplexChild a, IFoo foo) { } }

    public class Simple { }
    public class WithProperty { public Simple Simple { get; set; } }

    public class MultiConstructor
    {
        public int Dependencies { get; set; }

        public MultiConstructor(IFoo foo, IBar bar)
        {
            Dependencies = 2;
        }

        public MultiConstructor(IFoo foo)
        {
            Dependencies = 1;
        }
    }

    public class MarkedConstructor
    {
        public int Constructor { get; set; }

        public MarkedConstructor(IFoo foo, IBar bar)
        {
            Constructor = 1;
        }

        [InjectionConstructor]
        public MarkedConstructor(IBar bar, IFoo foo)
        {
            Constructor = 2;
        }
        
    }
}
