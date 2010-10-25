using Microsoft.Practices.ObjectBuilder2;

namespace OpenRasta.DI.Unity.Extensions.Policies
{
    /// <summary>
    /// Makes the current <see cref="TypeTracker"/> available to other policies and strategies.
    /// </summary>
    public class TypeTrackerPolicy : IBuilderPolicy
    {
        public TypeTracker TypeTracker { get; private set; }

        public TypeTrackerPolicy(TypeTracker typeTracker)
        {
            TypeTracker = typeTracker;
        }
    }
}
