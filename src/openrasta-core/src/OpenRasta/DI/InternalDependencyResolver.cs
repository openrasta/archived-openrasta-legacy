#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using OpenRasta.DI.Internal;
using OpenRasta.Diagnostics;
using OpenRasta.Pipeline;

namespace OpenRasta.DI
{
    public abstract class DependencyLifetimeManager
    {
        protected DependencyLifetimeManager(InternalDependencyResolver resolver)
        {
            Resolver = resolver;
        }

        protected InternalDependencyResolver Resolver { get; private set; }

        public virtual bool IsRegistrationAvailable(DependencyRegistration registration)
        {
            return true;
        }

        public virtual object Resolve(ResolveContext context, DependencyRegistration registration)
        {
            return context.Builder.CreateObject(registration);
        }

        public virtual void VerifyRegistration(DependencyRegistration registration)
        {
        }
    }

    public class InternalDependencyResolver : DependencyResolverCore, IDependencyResolver
    {
        readonly Dictionary<DependencyLifetime, DependencyLifetimeManager> _lifetimeManagers;
        ILogger _log = new TraceSourceLogger();

        public InternalDependencyResolver()
        {
            Registrations = new DependencyRegistrationCollection();
            _lifetimeManagers = new Dictionary<DependencyLifetime, DependencyLifetimeManager>
            {
                { DependencyLifetime.Transient, new TransientLifetimeManager(this) }, 
                { DependencyLifetime.Singleton, new SingletonLifetimeManager(this) }, 
                { DependencyLifetime.PerRequest, new PerRequestLifetimeManager(this) }
            };
        }

        public ILogger Log
        {
            get { return _log; }
            set { _log = value; }
        }

        public DependencyRegistrationCollection Registrations { get; private set; }

        protected override void AddDependencyCore(Type serviceType, Type concreteType, DependencyLifetime lifetime)
        {
            Registrations.Add(new DependencyRegistration(serviceType, concreteType, _lifetimeManagers[lifetime]));
        }

        protected override void AddDependencyCore(Type concreteType, DependencyLifetime lifetime)
        {
            AddDependencyCore(concreteType, concreteType, lifetime);
        }

        protected override void AddDependencyInstanceCore(Type serviceType, object instance, DependencyLifetime lifetime)
        {
            var instanceType = instance.GetType();

            var registration = new DependencyRegistration(serviceType, instanceType, _lifetimeManagers[lifetime], instance);

            Registrations.Add(registration);
        }

        protected override IEnumerable<TService> ResolveAllCore<TService>()
        {
            return from dependency in Registrations[typeof(TService)]
                   where dependency.LifetimeManager.IsRegistrationAvailable(dependency)
                   select (TService)Resolve(dependency);
        }

        protected override object ResolveCore(Type serviceType)
        {
            if (!Registrations.HasRegistrationForService(serviceType))
                throw new DependencyResolutionException("No type registered for {0}".With(serviceType.Name));

            return Resolve(Registrations.GetRegistrationForService(serviceType));
        }

        public void HandleIncomingRequestProcessed()
        {
            var store = (IContextStore)Resolve(typeof(IContextStore));
            store.Destruct();
        }

        public bool HasDependency(Type serviceType)
        {
            if (serviceType == null)
                return false;
            return Registrations.HasRegistrationForService(serviceType);
        }

        public bool HasDependencyImplementation(Type serviceType, Type concreteType)
        {
            return Registrations.HasRegistrationForService(serviceType) && Registrations[serviceType].Count(r => r.ConcreteType == concreteType) >= 1;
        }

        object Resolve(DependencyRegistration dependency)
        {
            var context = new ResolveContext(Registrations, Log);
            return context.Resolve(dependency);
        }
    }
}

#region Full license
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion