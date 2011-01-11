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
using System.Diagnostics;
using System.Linq;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ComponentActivator;
using Castle.Windsor;
using OpenRasta.DI.Internal;
using OpenRasta.Pipeline;
#if CASTLE_20
using Castle.MicroKernel.Registration;
#endif

namespace OpenRasta.DI.Windsor
{
    public class WindsorDependencyResolver : DependencyResolverCore, IDependencyResolver
    {
        private static readonly object _syncLock = new object();

        readonly IWindsorContainer _windsorContainer;

        public WindsorDependencyResolver(IWindsorContainer container)
        {
            _windsorContainer = container;
        }

        public bool HasDependency(Type serviceType)
        {
            if (serviceType == null) return false;
            return AvailableHandlers(_windsorContainer.Kernel.GetHandlers(serviceType)).Any();
        }

        public bool HasDependencyImplementation(Type serviceType, Type concreteType)
        {
            return
                AvailableHandlers(_windsorContainer.Kernel.GetHandlers(serviceType))
                    .Any(h => h.ComponentModel.Implementation == concreteType);
        }

        public void HandleIncomingRequestProcessed()
        {
            var store = _windsorContainer.Resolve<IContextStore>();

            store.Destruct();
        }

        protected override object ResolveCore(Type serviceType)
        {
            // IHandler[] handlers = _windsorContainer.Kernel.GetHandlers(serviceType);

            // IHandler firstAvailHandler = AvailableHandlers(handlers).First();
            // return _windsorContainer.Resolve(firstAvailHandler.ComponentModel.Name);
            return _windsorContainer.Resolve(serviceType);
        }

        protected override IEnumerable<TService> ResolveAllCore<TService>()
        {
            // return _windsorContainer.ResolveAll<TService>();
            var handlers = _windsorContainer.Kernel.GetAssignableHandlers(typeof (TService));
            var resolved = new List<TService>();
            foreach (var handler in AvailableHandlers(handlers))
                try
                {
                    resolved.Add((TService) _windsorContainer.Resolve(handler.ComponentModel.Name));
                }
                catch
                {
                    continue;
                }
            return resolved;
        }

        protected override void AddDependencyCore(Type dependent, Type concrete, DependencyLifetime lifetime)
        {
            lock (_syncLock)
            {
                string componentName = Guid.NewGuid().ToString();
                if (lifetime != DependencyLifetime.PerRequest)
                {
#if CASTLE_20
                    _windsorContainer.AddComponentLifeStyle(componentName,
                                                            dependent,
                                                            concrete,
                                                            ConvertLifestyles.ToLifestyleType(lifetime));
#elif CASTLE_10
                 _windsorContainer.AddComponentWithLifestyle(componentName, dependent, concrete, ConvertLifestyles.ToLifestyleType(lifetime));
#endif
                }
                else
                {
#if CASTLE_20
                    _windsorContainer.Register(
                        Component.For(dependent).Named(componentName).ImplementedBy(concrete).LifeStyle.Custom(typeof(ContextStoreLifetime)));
#elif CASTLE_10
                                ComponentModel component = _windsorContainer.Kernel.ComponentModelBuilder.BuildModel(componentName, dependent, concrete, null);
                component.LifestyleType = ConvertLifestyles.ToLifestyleType(lifetime);
                component.CustomLifestyle = typeof (ContextStoreLifetime);
                _windsorContainer.Kernel.AddCustomComponent(component);
#endif
                }
            }
        }

        protected override void AddDependencyInstanceCore(Type serviceType, object instance, DependencyLifetime lifetime)
        {
            lock (_syncLock)
            {
                string key = Guid.NewGuid().ToString();
                if (lifetime == DependencyLifetime.PerRequest)
                {
                    // try to see if we have a registration already
                    var store = (IContextStore)Resolve(typeof(IContextStore));
                    if (_windsorContainer.Kernel.HasComponent(serviceType))
                    {
                        var handler = _windsorContainer.Kernel.GetHandler(serviceType);
                        if (handler.ComponentModel.ExtendedProperties[Constants.REG_IS_INSTANCE_KEY] != null)
                        {
                            // if there's already an instance registration we update the store with the correct reg.
                            store[handler.ComponentModel.Name] = instance;
                        }
                        else
                        {
                            throw new DependencyResolutionException("Cannot register an instance for a type already registered");
                        }
                    }
                    else
                    {
                        var component = new ComponentModel(key, serviceType, instance.GetType());
                        var customLifestyle = typeof(ContextStoreLifetime);
                        component.LifestyleType = LifestyleType.Custom;
                        component.CustomLifestyle = customLifestyle;
                        component.CustomComponentActivator = typeof(ContextStoreInstanceActivator);
                        component.ExtendedProperties[Constants.REG_IS_INSTANCE_KEY] = true;
                        component.Name = component.Name;

                        _windsorContainer.Kernel.AddCustomComponent(component);
                        store[component.Name] = instance;
                    }
                }
                else if (lifetime == DependencyLifetime.Singleton)
                {
                    _windsorContainer.Kernel.AddComponentInstance(key, serviceType, instance);
                }
            }
        }

        protected override void AddDependencyCore(Type handlerType, DependencyLifetime lifetime)
        {
            AddDependencyCore(handlerType, handlerType, lifetime);
        }

        IEnumerable<IHandler> AvailableHandlers(IEnumerable<IHandler> handlers)
        {
            return from handler in handlers
                   where handler.CurrentState == HandlerState.Valid
                         && IsAvailable(handler.ComponentModel)
                   select handler;
        }

        bool IsAvailable(ComponentModel component)
        {
            bool isWebInstance = IsWebInstance(component);
            if (isWebInstance)
            {
                if (component.Name == null || !HasDependency(typeof (IContextStore))) return false;
                var store = _windsorContainer.Resolve<IContextStore>();
                bool isInstanceAvailable = store[component.Name] != null;
                return isInstanceAvailable;
            }
            return true;
        }

        static bool IsWebInstance(ComponentModel component)
        {
            return typeof (ContextStoreLifetime).IsAssignableFrom(component.CustomLifestyle)
                   && component.ExtendedProperties[Constants.REG_IS_INSTANCE_KEY] != null;
        }
    }

    public class ContextStoreInstanceActivator : AbstractComponentActivator
    {
        string storeKey;

        public ContextStoreInstanceActivator(ComponentModel model, IKernel kernel, ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction)
            : base(model, kernel, onCreation, onDestruction)
        {
            storeKey = model.Name;
        }

        protected override object InternalCreate(CreationContext context)
        {
            var store = (IContextStore) Kernel.Resolve(typeof (IContextStore));
            if (store[storeKey] == null)
            {
                Debug.WriteLine("The instance is not present in the context store");
                return null;
            }

            return store[storeKey];
        }

        protected override void InternalDestroy(object instance)
        {
            var store = (IContextStore) Kernel.Resolve(typeof (IContextStore));

            store[storeKey] = null;
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