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
using OpenRasta.TypeSystem;
using OpenRasta.TypeSystem.ReflectionBased;

namespace Instances_Specification
{
    public class when_assigning_properties_to_an_existing_object : instance_context
    {
        [Test]
        public void cannot_assign_properties_to_a_null_object()
        {
            GivenTypeInstance<Customer>();

            Executing(() => ThenTypeBuilder.Apply(null)).ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void the_properties_are_assigned()
        {
            GivenTypeInstance<Customer>();
            GivenProperty("username", "johndoe");

            var newCustomer = new Customer {FirstName = "John"};

            ThenTypeBuilder.Apply(newCustomer);

            newCustomer.Username.ShouldBe("johndoe");
            newCustomer.FirstName.ShouldBe("John");

            GivenProperty("lastname", "doe");
            newCustomer = new Customer {FirstName = "John"};
            ThenTypeBuilder.Apply(newCustomer);

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
            GivenTypeInstance<string>();

            ThenTypeBuilder.GetProperty("Length").ShouldBeTheSameInstanceAs(ThenTypeBuilder.GetProperty("Length"));
        }

        [Test]
        public void a_readonly_property_is_returned_as_non_writable()
        {
            GivenTypeInstance<string>();

            ThenTypeBuilder.GetProperty("Length").CanWrite.ShouldBeFalse();
        }

        [Test]
        public void an_unknown_property_returns_null()
        {
            GivenTypeInstance<string>();

            ThenTypeBuilder.GetProperty("unknown").ShouldBeNull();
        }
    }

    public class when_getting_list_of_changes : instance_context
    {
        [Test]
        public void a_property_retrieved_but_wihtout_a_value_is_not_present_in_the_changes()
        {
            GivenTypeInstance<Customer>();

            ThenTypeBuilder.GetProperty("FirstName")
                .TrySetValue(2).ShouldBeFalse();

            ThenTypeBuilder.Changes.Count.ShouldBe(0);
        }

        [Test]
        public void a_successfully_assigned_property_is_present_in_the_list_of_changes()
        {
            GivenTypeInstance<Customer>();

            ThenTypeBuilder.GetProperty("FirstName").TrySetValue("Frodo");

            ThenTypeBuilder.Changes.Count.ShouldBe(1);
            ThenTypeBuilder.Changes.ContainsKey("FirstName").ShouldBeTrue();
        }

        [Test]
        public void accessing_the_indexer_with_an_unknown_key_throws()
        {
            GivenTypeInstance<Customer>();
            Executing(() => { IPropertyBuilder value = ThenTypeBuilder.Changes["firstname"]; })
                .ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Test]
        public void the_change_list_is_readonly()
        {
            GivenTypeInstance<Customer>();

            ThenTypeBuilder.Changes.IsReadOnly.ShouldBeTrue();
            Executing(() => ThenTypeBuilder.Changes.Add(null, null))
                .ShouldThrow<NotSupportedException>();

            Executing(() => ThenTypeBuilder.Changes.Remove(null))
                .ShouldThrow<NotSupportedException>();

            Executing(() => ThenTypeBuilder.Changes["bla"] = null)
                .ShouldThrow<NotSupportedException>();
        }

        [Test]
        public void the_keys_are_case_insensitive()
        {
            GivenTypeInstance<Customer>();
            ThenTypeBuilder.GetProperty("FirstName")
                .TrySetValue("Frodo");
            ThenTypeBuilder.Changes["firstname"].Value.ShouldBe("Frodo");
        }
    }

    public class when_writing_propertie_values : instance_context
    {
        [Test]
        public void a_property_is_not_set_when_the_type_is_incompatible()
        {
            GivenTypeInstance<Customer>();

            ThenTypeBuilder.GetProperty("Username").TrySetValue(3).ShouldBeFalse();
        }

        [Test]
        public void a_writable_property_is_set_and_its_value_can_be_retrieved()
        {
            GivenTypeInstance<Customer>();

            ThenTypeBuilder.GetProperty("Username").TrySetValue("hello")
                .ShouldBeTrue();

            ThenTypeBuilder.GetProperty("Username")
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
            GivenTypeInstance<Customer>();
            Customer customer = ThenTypeBuilder.Create().ShouldBeOfType<Customer>().ShouldNotBeNull();

            ThenTypeBuilder.GetProperty("FirstName").TrySetValue("Frodo")
                .ShouldBeTrue();

            customer.LastName = "Baggins";
            customer = ThenTypeBuilder.Create() as Customer;

            customer.FirstName.ShouldBe("Frodo");
            customer.LastName.ShouldBeNull();
        }

        [Test]
        public void assigning_properties_after_the_property_got_assigned_a_parent_updates_the_parent()
        {
            GivenTypeInstance<Customer>();
            var customerInstance = new Customer();
            ThenTypeBuilder.TrySetValue(customerInstance);

            IPropertyBuilder firstName = ThenTypeBuilder.GetProperty("FirstName");
            firstName.SetOwner(ThenTypeBuilder);

            firstName.TrySetValue(new[] {"Smeagol"}, IdentityConverter);

            customerInstance.FirstName.ShouldBe("Smeagol");
        }

        [Test]
        public void cannot_assign_a_null_parent_to_a_property()
        {
            GivenTypeInstance<Customer>();
            Executing(() => ThenTypeBuilder.GetProperty("FirstName").SetOwner(null))
                .ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void cannot_assign_a_parent_twice()
        {
            GivenTypeInstance<Customer>();
            ThenTypeBuilder.GetProperty("FirstName").SetOwner(ThenTypeBuilder);
            ITypeBuilder newBuilder = new ReflectionBasedType(typeof(Customer)).CreateBuilder();
            Executing(() => ThenTypeBuilder.GetProperty("FirstName").SetOwner(newBuilder))
                .ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void no_reference_is_kept_after_a_call_to_apply()
        {
            GivenTypeInstance<Customer>();
            var customer = new Customer();
            ThenTypeBuilder.Apply(customer);

            ThenTypeBuilder.GetProperty("FirstName").TrySetValue("Frodo")
                .ShouldBeTrue();

            object newCustomer = ThenTypeBuilder.Create();
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
            _result = ThenTypeBuilder.Create();
        }

        [Test]
        public void a_type_instance_can_be_applied_twice()
        {
            GivenTypeInstance<Customer>();
            GivenProperty("Username", "johndoe");

            WhenCreatingTheObject();

            GivenProperty("FirstName", "John");
            WhenCreatingTheObject();

            ThenTheObject<Customer>().FirstName.ShouldBe("John");
            ThenTheObject<Customer>().Username.ShouldBe("johndoe");
        }

        [Test]
        public void a_value_can_be_assigned_to_the_type_instance()
        {
            GivenTypeInstance<int>();

            ThenTypeBuilder.TrySetValue(3)
                .ShouldBeTrue();
            ThenTypeBuilder.HasValue.ShouldBeTrue();
            ThenTypeBuilder.Value.ShouldBe(3);
        }

        [Test]
        public void a_value_of_the_incorrect_type_cannot_be_assinged()
        {
            GivenTypeInstance<int>();

            ThenTypeBuilder.TrySetValue("hello")
                .ShouldBeFalse();

            ThenTypeBuilder.HasValue.ShouldBeFalse();
        }

        [Test]
        public void creating_an_object_twice_returns_the_same_object()
        {
            GivenTypeInstance<Customer>();
            ThenTypeBuilder.Create().ShouldBeTheSameInstanceAs(ThenTypeBuilder.Create());
        }

        [Test]
        public void nested_properties_are_assigned()
        {
            GivenTypeInstance<Customer>();
            GivenProperty("Address.Line1", "Cadbury Street");
            GivenProperty("Address.City", "London");

            WhenCreatingTheObject();

            ThenTheObject<Customer>().Address.Line1.ShouldBe("Cadbury Street");
            ThenTheObject<Customer>().Address.City.ShouldBe("London");
        }

        [Test]
        public void setting_properties_on_children_of_indexers_works_as_expected()
        {
            GivenTypeInstance<House>();

            GivenProperty("CustomersByName:john.FirstName", "John");
            GivenProperty("CustomersByName:john.LastName", "Doe");
            WhenCreatingTheObject();
            ThenTheObject<House>().CustomersByName["john"].FirstName.ShouldBe("John");
            ThenTheObject<House>().CustomersByName["john"].LastName.ShouldBe("Doe");
        }

        [Test]
        public void the_properties_are_assigned()
        {
            GivenTypeInstance<Customer>();
            GivenProperty("Username", "johndoe");
            GivenProperty("FirstName", "john");

            WhenCreatingTheObject();
            ThenTheObject<Customer>().Username.ShouldBe("johndoe");
            ThenTheObject<Customer>().FirstName.ShouldBe("john");
        }
    }

    public class when_assigning_a_value_to_a_parameter_binder : parameter_instance_context
    {
        [Test]
        public void the_parameter_has_a_value()
        {
            GivenParameterInstance((int a) => { });
            GivenBinderKey("a", 1);

            TheParameter.IsReadyForAssignment.ShouldBeTrue();
        }
    }

    public class when_not_assigning_a_value_to_a_parameter : parameter_instance_context
    {
        public void MethodWithOptional([Optional] int a)
        {
        }

        [Test]
        public void an_optional_value_gives_the_parameter_a_value()
        {
            GivenParameterInstance<int>(MethodWithOptional);

            TheParameter.IsReadyForAssignment.ShouldBeTrue();
        }

        [Test]
        public void an_unfilled_parameter_doesnt_have_value()
        {
            GivenParameterInstance((int a) => { });

            TheParameter.IsReadyForAssignment.ShouldBeFalse();
        }
    }

    public class parameter_instance_context : instance_context
    {
        protected ParameterInstance TheParameter;

        protected void GivenBinderKey<TValue>(string key, TValue value)
        {
            TheParameter.Binder.SetProperty(key, new[] {value}, (src, output) => BindingResult.Success(src))
                .ShouldBeTrue();
        }

        protected void GivenParameterInstance<T>(Action<T> action)
        {
            var parameterType = new ReflectionBasedTypeSystem().FromClr(action.Method.GetParameters()[0].ParameterType);
            TheParameter = new ParameterInstance(new ReflectionParameter(action.Method.GetParameters()[0]),
                                                 new KeyedValuesBinder(
                                                     parameterType,
                                                     action.Method.GetParameters()[0].Name));
        }
    }

    public class instance_context : context
    {
        // ITypeSystem _typeSystem = new 
        public void GivenTypeInstance<T1>()
        {
            ThenTypeBuilder = new ReflectionBasedTypeSystem().FromClr(typeof(T1)).CreateBuilder();
        }

        protected ITypeBuilder ThenTypeBuilder;

        protected void GivenProperty(string prop, object value)
        {
            ThenTypeBuilder.GetProperty(prop).TrySetValue(value).ShouldBeTrue();
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