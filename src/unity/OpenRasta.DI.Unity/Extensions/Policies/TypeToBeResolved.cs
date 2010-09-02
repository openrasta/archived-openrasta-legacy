using System;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace OpenRasta.DI.Unity.Extensions.Policies
{
    /// <summary>
    /// Used by the injection policies to indicate how further dependencies will be resolved.
    /// </summary>
    class TypeToBeResolved : TypedInjectionValue
    {
        readonly IDependencyResolverPolicy resolver;

        public TypeToBeResolved(Type parameterType, IDependencyResolverPolicy resolver)
            : base(parameterType)
        {
            this.resolver = resolver;
        }

        public override IDependencyResolverPolicy GetResolverPolicy(Type typeToBuild)
        {
            return resolver;
        }
    }
}
