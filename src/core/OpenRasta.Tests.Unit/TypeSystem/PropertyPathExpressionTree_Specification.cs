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
using NUnit.Framework;
using OpenRasta;
using OpenRasta.Reflection;
using OpenRasta.Testing;
using OpenRasta.Tests.Unit.Fakes;

namespace PropertyPathExpressionTree_Specification
{
    public class when_generating_paths_from_fields : context
    {
        [Test]
        public void nested_properties_are_generated()
        {
            var customer = new Customer { DateOfBirth = DateTime.Parse("14 Nov 2000") };
            var pp = new PropertyPathForInstance<int>(() => customer.DateOfBirth.Day);

            pp.Path.TypePrefix.ShouldBe("Customer");
            pp.Path.TypeSuffix.ShouldBe("DateOfBirth.Day");
            pp.PropertyType.ShouldBe<int>();
            pp.Value.ShouldBe(14);
        }

        [Test]
        public void the_class_without_namespace_is_generated()
        {
            var customer = new Customer { Username = "johndoe" };
            var pp = new PropertyPathForInstance<object>(
                () => customer.Username);

            pp.Path.TypePrefix.ShouldBe("Customer");
            pp.Path.TypeSuffix.ShouldBe("Username");
            pp.PropertyType.ShouldBe<string>();
            pp.Value.ShouldBe("johndoe");
        }

        [Test]
        public void the_indexers_are_generated()
        {
        }
    }

    public class when_generating_paths_for_instances : context
    {
        public when_generating_paths_for_instances()
        {
            Customer = new Customer { FirstName = "John" };
        }

        Customer Customer { get; set; }

        [Test]
        public void an_instance_part_of_the_path_is_replaced_with_the_prefix_in_object_paths()
        {
            try
            {
                ObjectPaths.Add(Customer, new PropertyPath { TypePrefix = "Customer", TypeSuffix = "TheFirst" });

                var pp = new PropertyPathForInstance<object>(() => Customer.FirstName);
                pp.Path.TypePrefix.ShouldBe("Customer");
                pp.Path.TypeSuffix.ShouldBe("TheFirst.FirstName");
            }
            finally
            {
                ObjectPaths.Remove(Customer);
            }
        }

        [Test]
        public void the_root_of_the_path_is_the_property_type()
        {
            var pp = new PropertyPathForInstance<object>(
                () => Customer.FirstName);

            pp.Path.TypePrefix.ShouldBe("Customer");
            pp.Path.TypeSuffix.ShouldBe("FirstName");
            pp.Value.ShouldBe("John");
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