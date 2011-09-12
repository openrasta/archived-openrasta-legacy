using System;
using OpenRasta.Codecs;
using OpenRasta.Configuration.Fluent;
using OpenRasta.Configuration.Fluent.Implementation;
using OpenRasta.Configuration.MetaModel;
using OpenRasta.TypeSystem;

namespace OpenRasta.Configuration
{
    public static class HasExtensions
    {
        public static IResourceDefinition ResourcesNamed(this IHas has, string name)
        {
            return has.ResourcesWithKey(name);
        }

        public static IResourceDefinition ResourcesOfType<T>(this IHas has)
        {
            return has.ResourcesWithKey(typeof(T));
        }

        public static IResourceDefinition ResourcesOfType(this IHas has, Type clrType)
        {
            return has.ResourcesWithKey(clrType);
        }

        public static IResourceDefinition ResourcesOfType(this IHas has, IType type)
        {
            return has.ResourcesWithKey(type);
        }

        /// <exception cref="ArgumentNullException"><c>has</c> is null.</exception>
        public static IResourceDefinition ResourcesWithKey(this IHas has, object resourceKey)
        {
            if (has == null) throw new ArgumentNullException("has");
            if (resourceKey == null) throw new ArgumentNullException("resourceKey");

            var resourceKeyAsType = resourceKey as Type;
            bool isStrictRegistration = false;
            if (resourceKeyAsType != null && CodecRegistration.IsStrictRegistration(resourceKeyAsType))
            {
                resourceKey = CodecRegistration.GetStrictType(resourceKeyAsType);
                isStrictRegistration = true;
            }
            var registration = new ResourceModel
            {
                ResourceKey = resourceKey, 
                IsStrictRegistration = isStrictRegistration
            };

            var hasBuilder = (IFluentTarget)has;
            hasBuilder.Repository.ResourceRegistrations.Add(registration);
            return new ResourceDefinition(hasBuilder.TypeSystem, registration);
        }
    }
}