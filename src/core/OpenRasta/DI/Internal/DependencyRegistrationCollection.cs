using System;
using System.Collections.Generic;
using System.Linq;
using OpenRasta.Pipeline;

namespace OpenRasta.DI.Internal
{
    public class DependencyRegistrationCollection : IContextStoreDependencyCleaner
    {
        readonly Dictionary<Type, List<DependencyRegistration>> _registrations = new Dictionary<Type, List<DependencyRegistration>>();

        public IEnumerable<DependencyRegistration> this[Type serviceType]
        {
            get
            {
                lock (_registrations)
                {
                    return GetSvcRegistrations(serviceType);
                }
            }
        }

        public void Add(DependencyRegistration registration)
        {
            registration.LifetimeManager.VerifyRegistration(registration);
            lock (_registrations)
            {
                GetSvcRegistrations(registration.ServiceType).Add(registration);
            }
        }

        public DependencyRegistration GetRegistrationForService(Type type)
        {
            lock (_registrations)
            {
                return GetSvcRegistrations(type).LastOrDefault(x => x.LifetimeManager.IsRegistrationAvailable(x));
            }
        }

        public bool HasRegistrationForService(Type type)
        {
            lock (_registrations)
            {
                return _registrations.ContainsKey(type) && GetSvcRegistrations(type).ToList().Any(x => x.LifetimeManager.IsRegistrationAvailable(x));
            }
        }

        public void Destruct(string key, object instance)
        {
            lock (_registrations)
            {
                foreach (var reg in _registrations)
                {
                    var toRemove = reg.Value.Where(x => x.Key == key).ToList();

                    toRemove.ForEach(x => reg.Value.Remove(x));
                }
            }
        }

        // Not thread safe
        List<DependencyRegistration> GetSvcRegistrations(Type serviceType)
        {
            List<DependencyRegistration> svcRegistrations;
            if (!_registrations.TryGetValue(serviceType, out svcRegistrations))
                _registrations.Add(serviceType, svcRegistrations = new List<DependencyRegistration>());

            return svcRegistrations;
        }
    }
}