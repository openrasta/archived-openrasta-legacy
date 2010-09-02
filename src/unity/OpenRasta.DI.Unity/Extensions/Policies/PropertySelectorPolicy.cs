using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;

namespace OpenRasta.DI.Unity.Extensions.Policies
{
    /// <summary>
    /// Injects values for all possible properties.
    /// </summary>
    /// <remarks>
    /// Normally Unity only injects values for properties explicitly marked with an attribute.
    /// </remarks>
    class PropertySelectorPolicy : IPropertySelectorPolicy
    {
        public IEnumerable<SelectedProperty> SelectProperties(IBuilderContext context)
        {
            var target = BuildKey.GetType(context.BuildKey);
            var typeTracker = context.Policies.Get<TypeTrackerPolicy>(context.BuildKey).TypeTracker;

            // Unity includes a policy used when explicitly configuring injection.  Here we borrow
            // it to specify the properties we want to fill.  Normally the user specifies specific
            // properties and the policies for filling them, here we do it automatically.
            var policy = new SpecifiedPropertiesSelectorPolicy();

            foreach (var property in target.GetProperties())
            {
                // Ignore indexed properties
                if(property.GetIndexParameters().Length > 0)
                    continue;

                // Ignore read only properties
                if(!property.CanWrite)
                    continue;

                // Ignore properties we can't fill anyway
                if(!typeTracker.HasDependency(property.PropertyType))
                    continue;

                // Ignore properties that would create obvious circular dependencies.  It is still
                // possible to create one if you have multiple classes referring to each other though.
                if (property.PropertyType == property.DeclaringType ||
                    property.DeclaringType.IsAssignableFrom(property.PropertyType))
                    continue;

                policy.AddPropertyAndValue(property, new TypeToBeResolved(
                    property.PropertyType,
                    GetResolver(property)));
            }

            return policy.SelectProperties(context);
        }

        static IDependencyResolverPolicy GetResolver(PropertyInfo property)
        {
            var resolverAttributes = property
                .GetCustomAttributes(typeof (DependencyResolutionAttribute), false)
                .OfType<DependencyResolutionAttribute>()
                .ToList();

            // Unity allows users to override DependencyResolutionAttribute and add it to their
            // properties to provide custom resolution of the properties dependency.  This preserves
            // that behaviour.
            if (resolverAttributes.Count() > 0)
            {
                return resolverAttributes[0].CreateResolver(property.PropertyType);
            }
            
            // If no custom logic was provided, this resolver will call back in to Unity.  Normally
            // Unity would only inject into properties if it had an attribute.
            return new NamedTypeDependencyResolverPolicy(property.PropertyType, null);
        }
    }
}