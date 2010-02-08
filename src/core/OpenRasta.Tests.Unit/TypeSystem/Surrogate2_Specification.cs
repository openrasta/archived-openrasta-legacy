using System;
using NUnit.Framework;
using OpenRasta.Testing;
using OpenRasta.TypeSystem;
using OpenRasta.TypeSystem.ReflectionBased;
using OpenRasta.TypeSystem.Surrogated;
using OpenRasta.TypeSystem.Surrogates;
using OpenRasta.TypeSystem.Surrogates.Static;

namespace OpenRasta.Tests.Unit.TypeSystem
{
    public class when_using_type_properties : context
    {
        readonly ITypeSystem _ts;

        public when_using_type_properties()
        {
            _ts = new ReflectionBasedTypeSystem(new SurrogateBuilderProvider(new ISurrogateBuilder[] { new Saruman() }), new PathManager());
        }

        [Test]
        public void can_assign_property_on_alien_type_instance()
        {
            var frodoType = _ts.FromClr<Frodo>();

            var property = frodoType.FindPropertyByPath("IsEvil");
            property.ShouldNotBeNull();

            var saruman = new Saruman();
            property.TrySetValue(saruman, false).ShouldBeTrue();

            saruman.IsEvil.ShouldBeFalse();
        }

        [Test]
        public void can_assign_property_on_original_type_instance()
        {
            var frodoType = _ts.FromClr<Frodo>();

            var property = frodoType.FindPropertyByPath("IsEvil");
            property.ShouldNotBeNull();

            var frodo = new Frodo();
            property.TrySetValue(frodo, true).ShouldBeTrue();

            frodo.IsGood.ShouldBeFalse();
        }

        [Test]
        public void owner_type_on_alien_property_is_correct()
        {
            var frodoType = _ts.FromClr<Frodo>();

            frodoType.FindPropertyByPath("IsEvil").Owner.ShouldBe(frodoType);
        }

        [Test]
        public void owner_type_on_real_property_is_correct()
        {
            var frodoType = _ts.FromClr<Frodo>();

            frodoType.FindPropertyByPath("IsGood").Owner.ShouldBe(frodoType);
        }

        [Test]
        public void real_properties_can_be_set()
        {
            var frodoType = _ts.FromClr<Frodo>();
            var naughty = new Frodo();
            frodoType.FindPropertyByPath("IsGood").TrySetValue(naughty, false)
                .ShouldBeTrue();

            naughty.IsGood.ShouldBeFalse();
        }
    }

    public class when_using_type_builders : context
    {
        readonly ReflectionBasedTypeSystem _ts;

        public when_using_type_builders()
        {
            _ts = new ReflectionBasedTypeSystem(new SurrogateBuilderProvider(new ISurrogateBuilder[] { new Saruman() }), new PathManager());
        }

        [Test]
        public void multiple_calls_to_alien_property_executed_on_same_instance_of_surrogate()
        {
            var frodo = _ts.FromClr<Frodo>().CreateBuilder();

            bool isEvil = frodo.GetProperty("IsEvil")
                .ShouldNotBeNull()
                .TrySetValue(true);
            bool isMoreEvil = frodo.GetProperty("IsMoreEvil")
                .ShouldNotBeNull()
                .TrySetValue(true);

            var builtFrodo = (Frodo)frodo.Create();
            builtFrodo.SarumanMessing.ShouldBe(2);
        }
    }


    public class Frodo
    {
        public Frodo()
        {
            IsGood = true;
        }

        public bool IsGood { get; set; }
        public int SarumanMessing { get; set; }
    }

    public class Saruman : AbstractStaticSurrogate<Frodo>, ISurrogate
    {
        bool _isEvil = true;
        Frodo frodo;

        public bool IsEvil
        {
            get { return _isEvil; }
            set
            {
                MoodJumps++;
                if (frodo != null)
                {
                    frodo.IsGood = !value;
                    frodo.SarumanMessing = MoodJumps;
                }

                _isEvil = value;
            }
        }

        public bool IsMoreEvil
        {
            get { return IsEvil; }
            set { IsEvil = value; }
        }

        public int MoodJumps { get; set; }

        object ISurrogate.Value
        {
            get { return frodo; }
            set { frodo = (Frodo)value; }
        }
    }
}