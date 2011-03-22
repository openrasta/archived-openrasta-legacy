using System.Collections.Generic;
using Microsoft.Practices.ObjectBuilder2;
using OpenRasta.DI.Unity.Extensions.Policies;

namespace OpenRasta.DI.Unity.Extensions.Strategies
{
    /// <summary>
    /// Detects cycles in the current build operation by watching for repeated build keys.
    /// </summary>
    class CycleDetectionStrategy : BuilderStrategy
    {
        readonly HashSet<object> previousKeys = new HashSet<object>();

        bool cycleDetected;

        public override void PreBuildUp(IBuilderContext context)
        {
            // We use a temporary policy to mark that we've started cycle detection for this build.
            // The policy is local to this build only and if it's not present any keys we've
            // gathered were from a previous build.
            var policy = context.Policies.Get<CycleDetectionPolicy>(context.BuildKey);

            if (policy == null)
            {
                previousKeys.Clear();
                context.Policies.SetDefault(new CycleDetectionPolicy());
            }

            // If we are seeing the same build key twice it means we've followed a cycle
            if (previousKeys.Contains(context.OriginalBuildKey))
            {
                cycleDetected = true;
                context.BuildComplete = true;
            }

            previousKeys.Add(context.BuildKey);
        }

        public override void PostBuildUp(IBuilderContext context)
        {
            if (cycleDetected)
                throw new DependencyResolutionException("Circular dependency detected.");
        }
    }
}