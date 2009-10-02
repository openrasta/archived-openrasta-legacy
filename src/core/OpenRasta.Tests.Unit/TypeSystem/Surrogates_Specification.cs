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
using System.Collections.Generic;
using Instances_Specification;
using NUnit.Framework;
using OpenRasta.Testing;
using OpenRasta.Tests.Unit.Fakes;
using OpenRasta.TypeSystem;
using OpenRasta.TypeSystem.ReflectionBased;

namespace Surrogates_Specification
{
    public class when_using_ListOfT : context
    {
        ITypeBuilder _theBuilder;

        [Test]
        public void indexer_values_are_ignored_and_values_are_appended()
        {
            GivenTypeInstance();

            _theBuilder.GetProperty(":1").TrySetValue("hello").ShouldBeTrue();
            _theBuilder.GetProperty(":0").TrySetValue("hello2").ShouldBeTrue();

            var theList = (List<string>) _theBuilder.Create();
            theList[0].ShouldBe("hello");
            theList[1].ShouldBe("hello2");
        }
        [Test]
        public void the_indexer_is_surrogated()
        {
            GivenTypeInstance();

            _theBuilder.GetProperty(":0").TrySetValue("hello")
                .ShouldBeTrue();

            var theList = (List<string>) _theBuilder.Create();
            theList[0].ShouldBe("hello");
        }

        void GivenTypeInstance()
        {
            _theBuilder = new ReflectionBasedTypeSystem().FromClr(typeof(List<string>)).CreateBuilder();
        }
    }

    public class when_using_ListOfT_as_a_nested_property : context
    {
        ITypeBuilder _theBuilder;

        [Test]
        public void a_nested_indexer_is_surrogated()
        {
            GivenTypeInstance();

            _theBuilder.GetProperty("ListOfStrings:0").TrySetValue("hello")
                .ShouldBeTrue();

            var theList = (ListContainer) _theBuilder.Create();
            theList.ListOfStrings[0].ShouldBe("hello");
        }
        [Test]
        public void indexer_value_is_ignored_when_surrogate_is_an_intermediary()
        {
            var instance = new ReflectionBasedTypeSystem().FromClr(typeof(House)).CreateBuilder();

            instance.GetProperty("Customers:4.FirstName").TrySetValue("Anakin");
            instance.GetProperty("Customers:0.FirstName").TrySetValue("Skywalker");

            var result = (House)instance.Create();
            result.Customers[0].FirstName.ShouldBe("Anakin");
        }
        void GivenTypeInstance()
        {
            _theBuilder = new ReflectionBasedTypeSystem().FromClr(typeof(ListContainer)).CreateBuilder();
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
            GivenTypeInstance<Customer>();
            GivenProperty("DateOfBirth.Day", 14);
            GivenProperty("DateOfBirth.Month", 12);

            WhenCreatingTheObject();

            ThenTheObject<Customer>().DateOfBirth.Day.ShouldBe(14);
            ThenTheObject<Customer>().DateOfBirth.Month.ShouldBe(12);
        }

        [Test]
        public void surrogate_types_are_used_for_read_only_properties()
        {
            GivenTypeInstance<DateTime>();
            GivenProperty("Day", 14);

            WhenCreatingTheObject();

            ThenTheObject<DateTime>().Day.ShouldBe(14);
        }

        T ThenTheObject<T>()
        {
            return (T) _result;
        }

        void WhenCreatingTheObject()
        {
            _result = ThenTypeBuilder.Create();
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