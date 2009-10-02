using System;
using OpenRasta.DI;

namespace OpenRasta.Configuration.MetaModel
{
    public class DependencyRegistrationModel
    {
        public DependencyRegistrationModel(Type serviceType, Type concreteType, DependencyLifetime lifetime)
        {
            if (serviceType == null) throw new ArgumentNullException("serviceType");
            if (concreteType == null) throw new ArgumentNullException("concreteType");
            ServiceType = serviceType;
            ConcreteType = concreteType;
            Lifetime = lifetime;
        }

        public Type ConcreteType { get; private set; }
        public DependencyLifetime Lifetime { get; private set; }
        public Type ServiceType { get; private set; }
    }
}