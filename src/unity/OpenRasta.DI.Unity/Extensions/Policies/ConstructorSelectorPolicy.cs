using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;

namespace OpenRasta.DI.Unity.Extensions.Policies
{
    /// <summary>
    /// Selects the constructor with the most parameters we can actually provide.
    /// </summary>
    /// <remarks>
    /// Normally Unity prefers the constructor with the most parameters, even if they can't be
    /// provided.
    /// </remarks>
    class ConstructorSelectorPolicy : IConstructorSelectorPolicy
    {
        /// <summary>
        /// Exposes part of Unity's default policy which is marked protected.
        /// </summary>
        class ParameterResolver : DefaultUnityConstructorSelectorPolicy
        {
            public IDependencyResolverPolicy GetResolver(ParameterInfo parameterInfo)
            {
                return CreateResolver(parameterInfo);
            }
        }

        readonly ParameterResolver parameterResolver = new ParameterResolver();

        public SelectedConstructor SelectConstructor(IBuilderContext context)
        {
            var target = BuildKey.GetType(context.BuildKey);
            var typeTracker = context.Policies.Get<TypeTrackerPolicy>(context.BuildKey).TypeTracker;
            var constructor = SelectConstructor(target, typeTracker);

            if (constructor == null)
                return null;

            // Unity includes a policy used when explicitly configuring injection.  Here we borrow
            // it to specify the constructor we want to user.  Normally the user specifies a specific
            // constructor and the policies for filling its parameters, here we do it automatically.
            var parameters = new List<InjectionParameterValue>();

            foreach (var parameter in constructor.GetParameters())
            {
                parameters.Add(new TypeToBeResolved(
                    parameter.ParameterType,
                    parameterResolver.GetResolver(parameter)));
            }

            return new SpecifiedConstructorSelectorPolicy(constructor, parameters.ToArray())
                .SelectConstructor(context);
        }

        static ConstructorInfo SelectConstructor(Type target, TypeTracker typeTracker)
        {
            ConstructorInfo best = null;
            var bestScore = -1;

            foreach (var constructor in target.GetConstructors())
            {
                var score = Rate(constructor, typeTracker);
                
                if(score > bestScore)
                {
                    best = constructor;
                    bestScore = score;
                }
            }

            return best;
        }

        /// <summary>
        /// Scores the given constructor based on the number of dependencies we can fill.
        /// </summary>
        static int Rate(MethodBase constructor, TypeTracker typeTracker)
        {
            // Preserve the default behaviour of preferring explicitly marked constructors.
            if (constructor.IsDefined(typeof(InjectionConstructorAttribute), false))
                return int.MaxValue;

            var score = 0;

            foreach (var parameter in constructor.GetParameters())
            {
                if(parameter.IsOut || parameter.IsRetval)
                    return -1;

                if (typeTracker.HasDependency(parameter.ParameterType))
                {
                    score++;
                }
                else
                {
                    // We don't know how to fill this parameter so try a different constructor
                    return -1;
                }
            }

            return score;
        }
    }
}