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

namespace OpenRasta.DI
{
    public abstract class DependencyResolverCore
    {
        public void AddDependency(Type serviceType, Type concreteType, DependencyLifetime lifetime)
        {
            CheckConcreteType(concreteType);
            CheckServiceType(serviceType, concreteType);
            CheckLifetime(lifetime);
            AddDependencyCore(serviceType, concreteType, lifetime);
        }

        public void AddDependency(Type concreteType, DependencyLifetime lifetime)
        {
            CheckConcreteType(concreteType);
            CheckLifetime(lifetime);

            AddDependencyCore(concreteType, lifetime);
        }

        protected abstract void AddDependencyCore(Type serviceType, Type concreteType, DependencyLifetime lifetime);
        protected abstract void AddDependencyCore(Type concreteType, DependencyLifetime lifetime);

        public void AddDependencyInstance(Type serviceType, object instance, DependencyLifetime lifetime)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");
            if (lifetime == DependencyLifetime.Transient)
                throw new ArgumentException("Cannot register an instance for Transient lifetimes.", "lifetime");

            CheckServiceType(serviceType, instance.GetType());
            AddDependencyInstanceCore(serviceType, instance, lifetime);
        }

        protected abstract void AddDependencyInstanceCore(Type serviceType, object instance, DependencyLifetime lifetime);

        public object Resolve(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");
            try
            {
                return ResolveCore(serviceType);
            }
            catch (Exception e)
            {
                if (e is DependencyResolutionException)
                    throw;
                throw new DependencyResolutionException("An error occurred while trying to resolve type {0}.".With(serviceType.Name), e);
            }
        }

        public IEnumerable<TService> ResolveAll<TService>()
        {
            try
            {
                return ResolveAllCore<TService>();
            }
            catch (Exception e)
            {
                if (e is DependencyResolutionException)
                    throw;
                throw new DependencyResolutionException("An error occurred while trying to resolve type {0}.".With(typeof(TService).Name), e);
            }
        }

        protected abstract IEnumerable<TService> ResolveAllCore<TService>();
        protected abstract object ResolveCore(Type serviceType);

        protected static void CheckConcreteType(Type concreteType)
        {
            if (concreteType == null)
                throw new ArgumentNullException("concreteType");
            if (concreteType.IsAbstract)
                throw new InvalidOperationException(
                    "The type {0} is abstract. You cannot register an abstract type for initialization.".With(
                        concreteType.FullName));
        }

        protected static void CheckLifetime(DependencyLifetime lifetime)
        {
            if (!Enum.IsDefined(typeof(DependencyLifetime), lifetime))
                throw new InvalidOperationException(
                    string.Format("Value {0} is unknown for enumeration DependencyLifetime.", lifetime));
        }

        protected static void CheckServiceType(Type serviceType, Type concreteType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");
            if (concreteType == null)
                throw new ArgumentNullException("concreteType");
            if (!serviceType.IsAssignableFrom(concreteType))
                throw new InvalidOperationException(
                    "The type {0} doesn't implement or inherit from {1}.".With(concreteType.Name, serviceType.Name));
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