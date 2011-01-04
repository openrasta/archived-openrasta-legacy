using System;
using System.Collections.Generic;
using OpenRasta.Collections;

namespace OpenRasta.DI.Internal
{
    public class SingletonLifetimeManager : DependencyLifetimeManager
    {
        readonly IDictionary<string, object> _instances = new NullBehaviorDictionary<string, object>();

        public SingletonLifetimeManager(InternalDependencyResolver builder)
            : base(builder)
        {
        }

        public override object Resolve(ResolveContext context, DependencyRegistration registration)
        {
            object instance;

            if (!_instances.TryGetValue(registration.Key, out instance))
                lock (_instances)
                {
                    if (!_instances.TryGetValue(registration.Key, out instance))
                        _instances.Add(registration.Key, instance = base.Resolve(context, registration));
                }
            return instance;
        }

        public override void VerifyRegistration(DependencyRegistration registration)
        {
            if (registration.IsInstanceRegistration)
            {
                if (_instances[registration.Key] != null)
                    throw new InvalidOperationException("Trying to register an instance for a registration that already has one.");
                lock (_instances)
                {
                    _instances[registration.Key] = registration.Instance;
                }
                registration.Instance = null;
            }
        }
    }
}