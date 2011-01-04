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
using Instances_Specification;
using NUnit.Framework;
using OpenRasta.Testing;
using OpenRasta.Tests.Unit.Fakes;
using OpenRasta.Tests.Unit.TypeSystem;
using OpenRasta.TypeSystem;
using Frodo = OpenRasta.Tests.Unit.Fakes.Frodo;

namespace Surrogates_Specification
{

    public class when_using_ListOfT : instance_context
    {
        ITypeBuilder _theBuilder;

        [Test]
        public void indexer_values_are_ignored_and_values_are_appended()
        {
            given_builder();

            _theBuilder.GetProperty(":1").TrySetValue("hello").ShouldBeTrue();
            _theBuilder.GetProperty(":0").TrySetValue("hello2").ShouldBeTrue();

            var theList = (List<string>)_theBuilder.Create();
            theList[0].ShouldBe("hello");
            theList[1].ShouldBe("hello2");
        }
        [Test]
        public void the_indexer_is_surrogated()
        {
            given_builder();

            _theBuilder.GetProperty(":0").TrySetValue("hello")
                .ShouldBeTrue();

            var theList = (List<string>)_theBuilder.Create();
            theList[0].ShouldBe("hello");
        }

        void given_builder()
        {
            _theBuilder = _ts.FromClr(typeof(List<string>)).CreateBuilder();
        }
    }

    public class when_using_ListOfT_as_a_nested_property : instance_context
    {
        ITypeBuilder _theBuilder;

        [Test]
        public void a_nested_indexer_is_surrogated()
        {
            GivenTypeInstance();

            _theBuilder.GetProperty("ListOfStrings:0").TrySetValue("hello")
                .ShouldBeTrue();

            var theList = (ListContainer)_theBuilder.Create();
            theList.ListOfStrings[0].ShouldBe("hello");
        }
        [Test]
        public void indexer_value_is_ignored_when_surrogate_is_an_intermediary()
        {
            var instance = _ts.FromClr(typeof(House)).CreateBuilder();

            instance.GetProperty("Customers:4.FirstName").TrySetValue("Anakin");
            instance.GetProperty("Customers:0.FirstName").TrySetValue("Skywalker");

            var result = (House)instance.Create();
            result.Customers[0].FirstName.ShouldBe("Anakin");
        }
        void GivenTypeInstance()
        {
            _theBuilder = _ts.FromClr(typeof(ListContainer)).CreateBuilder();
        }

        public class ListContainer
        {
            public List<string> ListOfStrings { get; set; }
        }
    }
    public class when_using_surrogated_property : instance_context
    {

    }
    public class when_using_DateTime_surrogate : instance_context
    {
        object _result;

        [Test]
        public void nested_surrogate_types_are_used_for_read_only_properties()
        {
            given_builder_for<Customer>();
            given_property("DateOfBirth.Day", 14);
            given_property("DateOfBirth.Month", 12);

            when_creating_object();

            result_as<Customer>().DateOfBirth.Day.ShouldBe(14);
            result_as<Customer>().DateOfBirth.Month.ShouldBe(12);
        }

        [Test]
        public void surrogate_types_are_used_for_read_only_properties()
        {
            given_builder_for<DateTime>();
            given_property("Day", 14);

            when_creating_object();

            result_as<DateTime>().Day.ShouldBe(14);
        }

        T result_as<T>()
        {
            return (T)_result;
        }

        void when_creating_object()
        {
            _result = TypeBuilder.Create();
        }

    }
    public class indexer_for_enumerates : context.indexer_context<IEnumerable<string>,string>
    {
        [Test]
        public void values_are_generated()
        {
            given_builder();
            given_successful_property(":0", "zero");
            given_successful_property(":1", "one");
            given_successful_property(":3", "three");

            when_object_built();

            then_values_should_be("zero","one","three");
        }
    }

    public class collection_for_enumerates : context.indexer_context<ICollection<string>, string>
    {
        [Test]
        public void values_are_generated()
        {
            given_builder();
            given_successful_property(":0", "zero");
            given_successful_property(":1", "one");
            given_successful_property(":3", "three");

            when_object_built();

            then_values_should_be("zero", "one", "three");
        }
    }

    public class list_for_enumerates : context.indexer_context<IList<string>, string>
    {
        [Test]
        public void values_are_generated()
        {
            given_builder();
            given_successful_property(":0", "zero");
            given_successful_property(":1", "one");
            given_successful_property(":3", "three");

            when_object_built();

            then_values_should_be("zero", "one", "three");
        }
    }
    public class Replicator : List<Frodo>{}
    public class using_enumerable_types_with_indexer_surrogates : context.indexer_context<Replicator,Frodo>
    {
        public void multiple_values_are_added_to_the_same_object()
        {
            given_builder();
            given_successful_property(":0.FirstName", "Frodo");
            given_successful_property(":0.LastName", "Baggins");

            when_object_built();

            result.First().FirstName.ShouldBe("Frodo");
            result.First().LastName.ShouldBe("Baggins");
        }
    }
    namespace context
    {
        public class indexer_context<T,TValue> : OpenRasta.Testing.context
            where T:IEnumerable<TValue>
        {
            protected static ITypeSystem TypeSystem = TypeSystems.Default;
            protected ITypeBuilder builder;
            protected T result;

            protected void given_builder()
            {
                builder = TypeSystem.FromClr<T>().CreateBuilder();
            }
            protected void given_successful_property(string key, object value)
            {
                builder.GetProperty(key).TrySetValue(value).ShouldBeTrue();
            }
            protected void when_object_built()
            {
                result = (T)builder.Create();
            }
            protected void then_values_should_be(params object[] values)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    result.Skip(i).FirstOrDefault().ShouldBe(values[i]);
                }
            }
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