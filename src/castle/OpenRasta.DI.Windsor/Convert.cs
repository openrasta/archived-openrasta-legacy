using System;
using Castle.Core;

namespace OpenRasta.DI.Windsor
{
    public static class ConvertLifestyles
    {
        public static LifestyleType ToLifestyleType(DependencyLifetime lifetime)
        {
            if (lifetime == DependencyLifetime.Singleton)
                return LifestyleType.Singleton;
            if (lifetime == DependencyLifetime.PerRequest)
                return LifestyleType.Custom;
            if (lifetime == DependencyLifetime.Transient)
                return LifestyleType.Transient;
            throw new ArgumentOutOfRangeException("lifetime", "The provided lifetime is not recognized.");
        }
    }
}