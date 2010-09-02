using System;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using OpenRasta.DI.Unity.Extensions.Policies;

namespace OpenRasta.DI.Unity.Extensions
{
    /// <summary>
    /// Customises the Unity containers injection behaviour to suit OpenRasta.  Refer to the
    /// individual policies for details.
    /// </summary>
    class InjectionPolicies : UnityContainerExtension
    {
        protected override void Initialize()
        {
            // When a new type is registered in the container, set up the injection policies
            Context.Registering += (sender, e) =>
            {
                ApplyPolicies(e.TypeTo);
                ApplyPolicies(e.TypeFrom);
            };

            Context.RegisteringInstance += (sender, e) =>
            {
                ApplyPolicies(e.RegisteredType);
                ApplyPolicies(e.Instance.GetType());
            };
        }

        void ApplyPolicies(Type type)
        {
            if(type == null)
                return;

            var key = new NamedTypeBuildKey(type);

            Context.Policies.Set<IConstructorSelectorPolicy>(new ConstructorSelectorPolicy(), key);
            Context.Policies.Set<IPropertySelectorPolicy>(new PropertySelectorPolicy(), key);
        }
    }
}