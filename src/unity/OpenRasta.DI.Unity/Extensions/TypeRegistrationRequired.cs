using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;
using OpenRasta.DI.Unity.Extensions.Strategies;

namespace OpenRasta.DI.Unity.Extensions
{
    /// <summary>
    /// Changes the default Unity behaviour so that types must be registered to be created.
    /// </summary>
    class TypeRegistrationRequired : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Context.Strategies.Add(new TypeRegistrationRequiredStrategy(), UnityBuildStage.PreCreation);
        }
    }
}
