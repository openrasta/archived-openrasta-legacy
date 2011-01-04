using System.Collections.Generic;
using NUnit.Framework;
using OpenRasta.Collections;
using OpenRasta.Testing;

namespace OpenRasta.Tests.Unit.Collections
{
    public class when_resuming_enumerations : context
    {
        readonly List<int> _original = new List<int> { 1, 2, 3, 4, 5 };
        List<int> _resulting;
        [Test]
        public void enumerating_all_values_is_successfull()
        {
            var iterator = CreateIterator();
            when_enumerating_values(iterator);

            _resulting.ShouldHaveSameElementsAs(_original);
        }

        [Test]
        public void enumerating_until_suspension_should_enumerate_previous_items()
        {
            var iter = CreateIterator();
            iter.SuspendAfter(2);
            when_enumerating_values(iter);
            _resulting.ShouldHaveSameElementsAs(new[] { 1, 2 });
        }
        [Test]
        public void resuming_after_suspension_should_enumerate_further_items()
        {
            var iter = CreateIterator();
            iter.SuspendAfter(2);
            when_enumerating_values(iter);
            _resulting.ShouldHaveSameElementsAs(new[] { 1, 2 });
            when_enumerating_values(iter);
            _resulting.ShouldHaveSameElementsAs(new[] { 3, 4, 5 });
        }

        [Test]
        public void enumerating_values_from_a_starting_point_is_successfull()
        {
            var iterator = CreateIterator();
            iterator.ResumeFrom(2).ShouldBeTrue();
            when_enumerating_values(iterator);
            _resulting.ShouldHaveSameElementsAs(new[] { 2, 3, 4, 5 });
        }

        [Test]
        public void enumerating_values_from_unknown_starting_point_fails()
        {
            var iterator = CreateIterator();
            iterator.ResumeFrom(10).ShouldBeFalse();
            iterator.GetEnumerator().MoveNext().ShouldBeFalse();
        }

        ResumableIterator<int, int> CreateIterator()
        {
            return new ResumableIterator<int, int>(_original.GetEnumerator(), x => x, (a, b) => a == b);
        }

        void when_enumerating_values(ResumableIterator<int, int> iterator)
        {
            _resulting = new List<int>();
            _resulting.AddRange(iterator);
        }
    }
}