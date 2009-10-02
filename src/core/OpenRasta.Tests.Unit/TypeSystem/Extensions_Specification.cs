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
using System.ComponentModel;
using NUnit.Framework;
using OpenRasta.Testing;
using OpenRasta.TypeSystem.ReflectionBased;

namespace Extensions_Specification
{
    public class when_creating_instances_from_strings : context
    {
        [Test]
        public void array_types_are_parsed()
        {
            typeof (int[]).CreateInstanceFrom(new[] {"1"})
                .ShouldBeOfType<int[]>()
                .ShouldContain(1);
        }

        [Test]
        public void arrays_of_strings_return_the_provided_values()
        {
            typeof (string[]).CreateInstanceFrom(new[] {"one", "two", "three"})
                .ShouldBeOfType<string[]>()
                .ShouldHaveSameElementsAs(new[] {"one", "two", "three"});
        }

        [Test]
        public void lists_are_parsed()
        {
            typeof (List<string>).CreateInstanceFrom(new[] {"one", "two"})
                .ShouldBeOfType<List<string>>()
                .ShouldContain("one")
                .ShouldContain("two");
        }

        [Test]
        public void non_array_types_are_not_parsed_if_there_are_multiple_values()
        {
            Executing(() => typeof (int).CreateInstanceFrom(new[] {"1", "2"}))
                .ShouldThrow<NotSupportedException>();
        }

        [Test]
        public void non_array_types_are_parsed_if_theres_one_value()
        {
            typeof (int).CreateInstanceFrom(new[] {"1"})
                .ShouldBeOfType<int>()
                .ShouldBe(1);
        }

        [Test]
        public void types_implementing_ICollection_of_T_are_parsed()
        {
            typeof (LinkedList<string>).CreateInstanceFrom(new[] {"one", "two"})
                .ShouldBeOfType<LinkedList<string>>()
                .ShouldContain("one")
                .ShouldContain("two");
        }
    }

    public class when_creating_type_string_for_simple_types : context
    {
        [Test]
        public void instance_results_in_typestring_for_the_instance_type()
        {
            new SimpleType().GetTypeString()
                .ShouldBe("SimpleType");
        }

        [Test]
        public void nested_types_use_the_dot_syntax()
        {
            typeof (SimpleType.NestedType).GetTypeString()
                .ShouldBe("SimpleType.NestedType");
        }

        [Test]
        public void null_types_result_in_an_error()
        {
            Executing(() => ((Type) null).GetTypeString())
                .ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void the_name_without_the_namespace_is_returned()
        {
            typeof (SimpleType).GetTypeString()
                .ShouldBe("SimpleType");
        }
    }

    public class when_creating_type_strings_for_generic_types : context
    {
        [Test]
        public void generic_type_strings_are_defined_with_parenthesis()
        {
            typeof (GenericType<string>).GetTypeString()
                .ShouldBe("GenericType(String)");
        }

        [Test]
        public void generic_types_that_are_not_constructed_do_not_have_a_typestring()
        {
            Executing(() => typeof (GenericType<>).GetTypeString())
                .ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void nested_generic_type_uses_the_generics_syntax()
        {
            typeof (SimpleType.NestedGenericType<string>).GetTypeString()
                .ShouldBe("SimpleType.NestedGenericType(String)");
        }

        [Test, Ignore("Need to understand how the generics reflection api works first.")]
        public void recursive_generic_types_use_the_generic_syntax()
        {
            typeof (GenericType<GenericType<string>>.NestedGenericType<string>).GetTypeString()
                .ShouldBe("GenericType(GenericType(string)).NestedGenericType(String)");
        }
    }

    public class when_calculating_inheritance_distances : context
    {
        [Test]
        public void a_type_implementing_an_interface_has_a_distance_of_0_to_that_interface()
        {
            typeof (IList<string>).GetInheritanceDistance(typeof (IEnumerable))
                .ShouldBe(0);
        }

        [Test]
        public void an_interface_has_a_distance_of_minus_one_to_a_concrete_type()
        {
            typeof (IList).GetInheritanceDistance(typeof (string))
                .ShouldBe(-1);
        }

        [Test]
        public void an_interface_has_a_distance_of_minus_one_to_an_interface_it_doesnt_implement()
        {
            typeof (IList<string>).GetInheritanceDistance(typeof (IList))
                .ShouldBe(-1);
        }

        [Test]
        public void an_interface_has_a_distance_of_one_to_object()
        {
            typeof (IList).GetInheritanceDistance(typeof (object))
                .ShouldBe(1);
        }

        [Test]
        public void any_type_has_a_distance_of_zero_to_itself()
        {
            typeof (int).GetInheritanceDistance(typeof (int))
                .ShouldBe(0);
        }

        [Test]
        public void comparing_to_a_type_not_in_the_inheritance_tree_returns_minus_one()
        {
            typeof (int).GetInheritanceDistance(typeof (string))
                .ShouldBe(-1);
        }

        [Test]
        public void primitive_types_return_one()
        {
            typeof (int).GetInheritanceDistance(typeof (ValueType))
                .ShouldBe(1);
        }
        [Test]
        public void a_parent_type_has_an_inheritance_distance_of_minus_one_to_a_child_type()
        {
            typeof(ValueType).GetInheritanceDistance(typeof(int)).ShouldBe(-1);
        }
    }

    public class when_generating_default_values : context
    {
        [Test]
        public void reference_types_return_null()
        {
            typeof (SimpleType).GetDefaultValue()
                .ShouldBeNull();
        }

        [Test]
        public void value_types_return_a_default_instance()
        {
            typeof (int).GetDefaultValue()
                .ShouldBe(0);
        }
    }

    public class when_converting_to_string : context
    {
        class Converter : TypeConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture,
                                             object value, Type destinationType)
            {
                if (destinationType == typeof (string))
                    return "ValueFromConverter";
                return null;
            }
        }

        [TypeConverter(typeof (Converter))]
        class TypeWithConverter
        {
        }

        [Test]
        public void non_primitive_types_without_converters_return_the_result_of_ToString()
        {
            var simpleType = new SimpleType();
            simpleType.ConvertToString()
                .ShouldBe(simpleType.ToString());
        }

        [Test]
        public void primitive_types_return_the_converted_value()
        {
            3.ConvertToString()
                .ShouldBe("3");
        }

        [Test]
        public void types_with_converters_return_the_coverter_value()
        {
            new TypeWithConverter().ConvertToString()
                .ShouldBe("ValueFromConverter");
        }
    }

    public class SimpleType
    {
        public override string ToString() { return "ToString()"; }

        public class NestedGenericType<T>
        {
        }

        public class NestedType
        {
        }
    }

    public class GenericType<T>
    {
        public class NestedGenericType<U>
        {
        }

        public class NestedType
        {
        }
    }
}

#region Full license
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion