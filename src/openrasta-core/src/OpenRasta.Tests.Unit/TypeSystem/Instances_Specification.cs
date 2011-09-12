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
using System.Runtime.InteropServices;
using NUnit.Framework;
using OpenRasta.Binding;
using OpenRasta.Testing;
using OpenRasta.Tests.Unit.Fakes;
using OpenRasta.Tests.Unit.TypeSystem;
using OpenRasta.TypeSystem;
using Frodo=OpenRasta.Tests.Unit.Fakes.Frodo;

namespace Instances_Specification
{
    public class when_assigning_properties_to_an_existing_object : instance_context
    {
        [Test]
        public void cannot_assign_properties_to_a_null_object()
        {
            given_builder_for<Customer>();

            Executing(() => TypeBuilder.Update(null)).ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void the_properties_are_assigned()
        {
            given_builder_for<Customer>();
            given_property("username", "johndoe");

            var newCustomer = new Customer {FirstName = "John"};

            TypeBuilder.Update(newCustomer);

            newCustomer.Username.ShouldBe("johndoe");
            newCustomer.FirstName.ShouldBe("John");

            given_property("lastname", "doe");
            newCustomer = new Customer {FirstName = "John"};
            TypeBuilder.Update(newCustomer);

            newCustomer.Username.ShouldBe("johndoe");
            newCustomer.FirstName.ShouldBe("John");
            newCustomer.LastName.ShouldBe("doe");
        }
    }

    public class when_getting_properties_by_name : instance_context
    {
        [Test]
        public void a_property_retrieved_twice_retrieves_the_same_object()
        {
            given_builder_for<string>();

            TypeBuilder.GetProperty("Length").ShouldBeTheSameInstanceAs(TypeBuilder.GetProperty("Length"));
        }

        [Test]
        public void a_readonly_property_is_returned_as_non_writable()
        {
            given_builder_for<string>();

            TypeBuilder.GetProperty("Length").CanWrite.ShouldBeFalse();
        }

        [Test]
        public void an_unknown_property_returns_null()
        {
            given_builder_for<string>();

            TypeBuilder.GetProperty("unknown").ShouldBeNull();
        }
    }

    public class when_getting_list_of_changes : instance_context
    {
        [Test]
        public void a_property_retrieved_but_wihtout_a_value_is_not_present_in_the_changes()
        {
            given_builder_for<Customer>();

            TypeBuilder.GetProperty("FirstName")
                .TrySetValue(2).ShouldBeFalse();

            TypeBuilder.Changes.Count.ShouldBe(0);
        }

        [Test]
        public void a_successfully_assigned_property_is_present_in_the_list_of_changes()
        {
            given_builder_for<Customer>();

            TypeBuilder.GetProperty("FirstName").TrySetValue("Frodo");

            TypeBuilder.Changes.Count.ShouldBe(1);
            TypeBuilder.Changes.ContainsKey("FirstName").ShouldBeTrue();
        }

        [Test]
        public void accessing_the_indexer_with_an_unknown_key_throws()
        {
            given_builder_for<Customer>();
            Executing(() => { IPropertyBuilder value = TypeBuilder.Changes["firstname"]; })
                .ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Test]
        public void the_change_list_is_readonly()
        {
            given_builder_for<Customer>();

            TypeBuilder.Changes.IsReadOnly.ShouldBeTrue();
            Executing(() => TypeBuilder.Changes.Add(null, null))
                .ShouldThrow<NotSupportedException>();

            Executing(() => TypeBuilder.Changes.Remove(null))
                .ShouldThrow<NotSupportedException>();

            Executing(() => TypeBuilder.Changes["bla"] = null)
                .ShouldThrow<NotSupportedException>();
        }

        [Test]
        public void the_keys_are_case_insensitive()
        {
            given_builder_for<Customer>();
            TypeBuilder.GetProperty("FirstName")
                .TrySetValue("Frodo");
            TypeBuilder.Changes["firstname"].Value.ShouldBe("Frodo");
        }
    }

    public class when_writing_propertie_values : instance_context
    {
        [Test]
        public void a_property_is_not_set_when_the_type_is_incompatible()
        {
            given_builder_for<Customer>();

            TypeBuilder.GetProperty("Username").TrySetValue(3).ShouldBeFalse();
        }

        [Test]
        public void a_writable_property_is_set_and_its_value_can_be_retrieved()
        {
            given_builder_for<Customer>();

            TypeBuilder.GetProperty("Username").TrySetValue("hello")
                .ShouldBeTrue();

            TypeBuilder.GetProperty("Username")
                .Value.ShouldBe("hello");
        }
    }

    public class when_writing_property_values_on_live_object : instance_context
    {
        BindingResult IdentityConverter(string str, Type destinationType)
        {
            return BindingResult.Success(str);
        }

        [Test]
        public void a_new_instance_is_created_when_using_create()
        {
            given_builder_for<Customer>();
            Customer customer = TypeBuilder.Create().ShouldBeOfType<Customer>().ShouldNotBeNull();

            TypeBuilder.GetProperty("FirstName").TrySetValue("Frodo")
                .ShouldBeTrue();

            customer.LastName = "Baggins";
            customer = TypeBuilder.Create() as Customer;

            customer.FirstName.ShouldBe("Frodo");
            customer.LastName.ShouldBeNull();
        }

        [Test]
        public void assigning_properties_after_the_property_got_assigned_a_parent_updates_the_parent()
        {
            given_builder_for<Customer>();

            var customerInstance = new Customer();
            TypeBuilder.TrySetValue(customerInstance);

            IPropertyBuilder firstName = TypeBuilder.GetProperty("FirstName");
            //firstName.SetOwner(TypeBuilder);

            firstName.TrySetValue(new[] {"Smeagol"}, IdentityConverter);

            TypeBuilder.Update(customerInstance);
            customerInstance.FirstName.ShouldBe("Smeagol");
        }

        [Test]
        public void no_reference_is_kept_after_a_call_to_apply()
        {
            given_builder_for<Customer>();
            var customer = new Customer();
            TypeBuilder.Update(customer);

            TypeBuilder.GetProperty("FirstName").TrySetValue("Frodo")
                .ShouldBeTrue();

            object newCustomer = TypeBuilder.Create();
            newCustomer.ShouldNotBeTheSameInstanceAs(customer);
            customer.FirstName.ShouldBe(null);
        }
    }

    public class when_creating_an_object : instance_context
    {
        object _result;

        T ThenTheObject<T>()
        {
            return (T) _result;
        }

        void WhenCreatingTheObject()
        {
            _result = TypeBuilder.Create();
        }

        [Test]
        public void a_type_instance_can_be_applied_twice()
        {
            given_builder_for<Customer>();
            given_property("Username", "johndoe");

            WhenCreatingTheObject();

            given_property("FirstName", "John");
            WhenCreatingTheObject();

            ThenTheObject<Customer>().FirstName.ShouldBe("John");
            ThenTheObject<Customer>().Username.ShouldBe("johndoe");
        }

        [Test]
        public void a_value_can_be_assigned_to_the_type_instance()
        {
            given_builder_for<int>();

            TypeBuilder.TrySetValue(3)
                .ShouldBeTrue();
            TypeBuilder.HasValue.ShouldBeTrue();
            TypeBuilder.Value.ShouldBe(3);
        }

        [Test]
        public void a_value_of_the_incorrect_type_cannot_be_assinged()
        {
            given_builder_for<int>();

            TypeBuilder.TrySetValue("hello")
                .ShouldBeFalse();

            TypeBuilder.HasValue.ShouldBeFalse();
        }
        [Test]
        public void nested_properties_are_assigned()
        {
            given_builder_for<Customer>();
            given_property("Address.Line1", "Cadbury Street");
            given_property("Address.City", "London");

            WhenCreatingTheObject();

            ThenTheObject<Customer>().Address.Line1.ShouldBe("Cadbury Street");
            ThenTheObject<Customer>().Address.City.ShouldBe("London");
        }

        [Test]
        public void setting_properties_on_children_of_indexers_works_as_expected()
        {
            given_builder_for<House>();

            given_property("CustomersByName:john.FirstName", "John");
            given_property("CustomersByName:john.LastName", "Doe");
            WhenCreatingTheObject();
            ThenTheObject<House>().CustomersByName["john"].FirstName.ShouldBe("John");
            ThenTheObject<House>().CustomersByName["john"].LastName.ShouldBe("Doe");
        }

        [Test]
        public void the_properties_are_assigned()
        {
            given_builder_for<Customer>();
            given_property("Username", "johndoe");
            given_property("FirstName", "john");

            WhenCreatingTheObject();
            ThenTheObject<Customer>().Username.ShouldBe("johndoe");
            ThenTheObject<Customer>().FirstName.ShouldBe("john");
        }
    }


    public abstract class instance_context : context
    {
        protected static ITypeSystem _ts = TypeSystems.Default;
        public instance_context()
        {
        }
        // ITypeSystem _typeSystem = new 
        public void given_builder_for<T1>()
        {
            TypeBuilder = _ts.FromClr(typeof(T1)).CreateBuilder();
        }

        protected ITypeBuilder TypeBuilder;

        protected void given_property(string prop, object value)
        {
            TypeBuilder.GetProperty(prop).TrySetValue(value).ShouldBeTrue();
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