using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;
using OpenRasta.DI.Unity.Extensions.Strategies;

namespace OpenRasta.DI.Unity.Extensions
{
    /// <summary>
    /// Adds early detection of circular references to prevent <see cref="StackOverflowException"/>s.
    /// </summary>
    class CycleDetector : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Context.Strategies.AddNew<CycleDetectionStrategy>(UnityBuildStage.PreCreation);
        }
    }
}

