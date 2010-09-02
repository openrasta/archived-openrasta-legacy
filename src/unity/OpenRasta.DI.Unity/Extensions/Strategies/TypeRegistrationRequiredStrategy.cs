using Microsoft.Practices.ObjectBuilder2;
using OpenRasta.DI.Unity.Extensions.Policies;

namespace OpenRasta.DI.Unity.Extensions.Strategies
{
    /// <summary>
    /// Makes sure that any types being built are actually registered in the container.
    /// </summary>
    class TypeRegistrationRequiredStrategy : BuilderStrategy
    {
        public override void PreBuildUp(IBuilderContext context)
        {
            // If we belong to a parent container we'll be registered first in the strategy chain
            // but we don't know about types that the child container has registered so skip the
            // check and let the child deal with it.  Ideally we shouldn't have been registered but
            // I couldn't figure out how to do that check in the extension initializtion.
            if (WeAreNotTheLast(context))
                return;

            var requestedType = BuildKey.GetType(context.OriginalBuildKey);
            var typeTracker = context.Policies.Get<TypeTrackerPolicy>(context.BuildKey).TypeTracker;

            if(!typeTracker.HasDependency(requestedType))
                throw new DependencyResolutionException("Couldn't find type " + requestedType.FullName);
        }

        bool WeAreNotTheLast(IBuilderContext context)
        {
            var foundUs = false;

            foreach (var strategy in context.Strategies)
            {
                if(!(strategy is TypeRegistrationRequiredStrategy))
                    continue;

                if (foundUs)
                    return true;

                if(strategy == this)
                    foundUs = true;
            }

            return false;
        }
    }
}
