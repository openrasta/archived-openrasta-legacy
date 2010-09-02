using Microsoft.Practices.Unity;
using OpenRasta.DI.Unity.Extensions;

namespace OpenRasta.DI.Unity
{
    static class UnityContainerExtensions
    {
        /// <summary>
        /// Fetches the <see cref="TypeTracker"/> extension for a particular container.
        /// </summary>
        public static TypeTracker TypeTracker(this IUnityContainer container)
        {
            return container.Configure<TypeTracker>();
        }
    }
}
