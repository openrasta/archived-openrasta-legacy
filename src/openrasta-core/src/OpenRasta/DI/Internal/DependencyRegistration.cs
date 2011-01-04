using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenRasta.DI.Internal
{
    public class DependencyRegistration
    {
        public DependencyRegistration(Type serviceType, Type concreteType, DependencyLifetimeManager lifetime)
            : this(serviceType, concreteType, lifetime, null)
        {
        }

        public DependencyRegistration(Type serviceType, Type concreteType, DependencyLifetimeManager lifetime, object instance)
        {
            Key = Guid.NewGuid().ToString();
            LifetimeManager = lifetime;
            ServiceType = serviceType;
            ConcreteType = concreteType;
            Constructors =
                new List<KeyValuePair<ConstructorInfo, ParameterInfo[]>>(
                    concreteType.GetConstructors().Select(ctor => new KeyValuePair<ConstructorInfo, ParameterInfo[]>(ctor, ctor.GetParameters())));
            Constructors.Sort((kv1, kv2) => kv1.Value.Length.CompareTo(kv2.Value.Length) * -1);
            Instance = instance;
            IsInstanceRegistration = instance != null;
        }

        public Type ConcreteType { get; set; }
        public List<KeyValuePair<ConstructorInfo, ParameterInfo[]>> Constructors { get; set; }
        public object Instance { get; set; }
        public bool IsInstanceRegistration { get; set; }
        public string Key { get; set; }
        public DependencyLifetimeManager LifetimeManager { get; set; }
        public Type ServiceType { get; set; }
    }
}