using System;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;
using OpenRasta.Binding;
using OpenRasta.DI;
using OpenRasta.OperationModel;
using OpenRasta.OperationModel.MethodBased;
using OpenRasta.Testing;
using OpenRasta.TypeSystem;

namespace OpenRasta.Tests.Unit.OperationModel.MethodBased
{
    public class when_using_optional_members : operation_context<MockOperationHandler>
    {
        [Test]
        public void the_operation_is_ready_for_invocation()
        {
            given_operation("Get", typeof(int));

            Operation.Inputs.AllReady().ShouldBeTrue();
        }
        [Test]
        public void all_parameters_are_satisfied()
        {
            given_operation("Get", typeof(int));

            Operation.Inputs.CountReady().ShouldBe(1);
        }
        [Test]
        public void a_default_parameter_value_is_supported()
        {
            given_operation("Search",typeof(string));

            Operation.Inputs.Optional().First().IsOptional.ShouldBeTrue();
            Operation.Inputs.Optional().First().Member.ShouldBeOfType<IParameter>().DefaultValue
                .ShouldBe("*");


        }
    }
    public class when_using_required_members : operation_context<MockOperationHandler>
    {
        [Test]
        public void the_operation_is_not_ready_for_invocation()
        {
            given_operation("Post", typeof(int), typeof(string));

            Operation.Inputs.AllReady().ShouldBeFalse();
        }
        [Test]
        public void no_parameter_is_satisfied()
        {
            given_operation("Post", typeof(int), typeof(string));

            Operation.Inputs.CountReady().ShouldBe(0);
        }
    }
    public class when_creating_operations : operation_context<MockOperationHandler>
    {
        [Test]
        public void the_operation_name_is_the_method_name()
        {
            given_operation("Get", typeof(int));

            Operation.Name.ShouldBe("Get");
        }
        [Test]
        public void the_operation_string_representation_is_the_method_signature()
        {
            given_operation("Get", typeof(int));

            Operation.ToString().ShouldBe("MockOperationHandler::Get(Int32 index)");
        }
        [Test]
        public void property_getters_are_not_selected()
        {
            Executing(()=>given_operation("get_Dependency"))
                .ShouldThrow<InvalidOperationException>();


        }
    }
    public class when_invoking_an_operation : operation_context<MockOperationHandler>
    {
        [Test]
        public void an_operation_not_ready_for_invocation_throws_an_exception()
        {
            given_operation("Post", typeof(int), typeof(string));

            Executing(() => Operation.Invoke())
                .ShouldThrow<InvalidOperationException>();
        }
        [Test]
        public void a_result_is_returned()
        {
            given_operation("Get", typeof(int));

            Operation.Invoke().Count().ShouldBe(1);
        }
    }
    public class when_reading_attributes_present_on_a_method : operation_context<OperationHandlerForAttributes>
    {
        [Test]
        public void a_single_attribute_is_found()
        {
            given_operation("GetHasOneAttribute", typeof(int));

            Operation.FindAttribute<DescriptionAttribute>()
                .ShouldNotBeNull()
                .Description.ShouldBe("Description");
        }
        [Test]
        public void multile_attributes_are_found()
        {
            given_operation("GetHasTwoAttributes",typeof(int));

            var attributes = Operation.FindAttributes<UselessAttribute>();
            attributes
                .FirstOrDefault(x => x.Name == "one").ShouldNotBeNull();
            attributes
                .FirstOrDefault(x => x.Name == "two").ShouldNotBeNull();
        }
        [Test]
        public void attributes_on_the_metod_owner_type_are_returned()
        {
            given_operation("GetHasTwoAttributes", typeof(int));

            var attributes = Operation.FindAttributes<UselessAttribute>();
            attributes
                .FirstOrDefault(x => x.Name == "type attribute").ShouldNotBeNull();
        }
        [Test]
        public void an_attribute_can_be_found_when_searching_by_interface()
        {
            given_operation("GetHasTwoAttributes", typeof(int));
            Operation.FindAttributes<IUseless>()
                .Count().ShouldBe(3);

        }
    }
    public class when_reading_attributes_not_present_on_a_method : operation_context<OperationHandlerForAttributes>
    {
        [Test]
        public void an_attribute_not_defined_returns_null()
        {
            given_operation("GetHasOneAttribute", typeof(int));

            Operation.FindAttribute<AttributeUsageAttribute>().ShouldBeNull();
        }
        [Test]
        public void an_attribute_not_defined_returns_an_empty_collection()
        {
            given_operation("GetHasOneAttribute", typeof(int));

            Operation.FindAttributes<AttributeUsageAttribute>().ShouldNotBeNull().ShouldBeEmpty();
        }
    }
    [Useless("type attribute")]
    public class OperationHandlerForAttributes
    {
        [Description("Description")]
        public void GetHasOneAttribute(int index){}
        [Useless("one")]
        [Useless("two")]
        public void GetHasTwoAttributes(int index){}
    }
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited=true)]
    public class UselessAttribute : Attribute, IUseless
    {
        public string Name { get; set; }

        public UselessAttribute(string name)
        {
            Name = name;
        }
    }
    public interface IUseless{}
    public class MockOperationHandler
    {
        public IDependencyResolver Dependency { get; set; }
        public int Get([Optional] int index)
        {
            return index;
        }
        public object Post(int index, string value)
        {
            return null;
        }
        public object Search([Optional, DefaultParameterValue("*")]string searchString)
        {
            return 0;
        }
    }

    public abstract class operation_context<THandler> : openrasta_context
    {
        protected operation_context()
        {
            Handler = TypeSystem.FromClr<THandler>();
        }

        protected IType Handler { get; set; }
        protected IOperation Operation { get; set; }

        protected void given_operation(string name, params Type[] parameters)
        {
            IMethod method = (from m in Handler.GetMethods()
                              where m.InputMembers.Count() == parameters.Length && m.Name.EqualsOrdinalIgnoreCase(name)
                              let matchingParams = 
                                  (from parameter in m.InputMembers
                                  from typeParameter in parameters
                                  where parameter.Type.CompareTo(parameter.TypeSystem.FromClr(typeParameter)) == 0
                                       select parameter).Count()
                              where parameters.Length == 0 || matchingParams == parameters.Length
                              select m).First();
            Operation = new MethodBasedOperation(new DefaultObjectBinderLocator(), Handler, method)
            {
                Resolver = Resolver
            };
        }

    }
}