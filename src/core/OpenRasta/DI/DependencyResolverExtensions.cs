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

namespace OpenRasta.DI
{
    public static class DependencyResolverExtensions
    {
        /// <summary>
        /// Adds a concrete dependency to the resolver.
        /// </summary>
        /// <typeparam name="TConcrete">The concrete type to register.</typeparam>
        /// <param name="resolver"></param>
        public static void AddDependency<TConcrete>(this IDependencyResolver resolver)
            where TConcrete : class
        {
            AddDependency<TConcrete>(resolver, DependencyLifetime.Transient);
        }

        /// <summary>
        /// Adds a concrete dependency with the specified lifetime.
        /// </summary>
        /// <typeparam name="TConcrete">The concrete type to register.</typeparam>
        /// <param name="resolver"></param>
        /// <param name="lifetime">The lifetime of the type.</param>
        public static void AddDependency<TConcrete>(this IDependencyResolver resolver, DependencyLifetime lifetime)
            where TConcrete : class
        {
            resolver.AddDependency(typeof(TConcrete), lifetime);
        }

        /// <summary>
        /// Adds a dependency of type <typeparamref name="TService"/>, implemented by the type <typeparamref name="TConcrete"/>.
        /// </summary>
        /// <typeparam name="TService">The type to register.</typeparam>
        /// <typeparam name="TConcrete">The type of the concrete implementation.</typeparam>
        /// <param name="resolver">The resolver.</param>
        public static void AddDependency<TService, TConcrete>(this IDependencyResolver resolver)
            where TService : class
            where TConcrete : class, TService
        {
            AddDependency<TService, TConcrete>(resolver, DependencyLifetime.Singleton);
        }

        /// <summary>
        /// Adds a dependency of type <typeparamref name="TService"/>, implemented by the type <typeparamref name="TConcrete"/>, with the specified dependency lifetime.
        /// </summary>
        /// <typeparam name="TService">The type to register.</typeparam>
        /// <typeparam name="TConcrete">The type of the concrete implementation.</typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <param name="lifetime">The lifetime of the type.</param>
        public static void AddDependency<TService, TConcrete>(this IDependencyResolver resolver, 
                                                              DependencyLifetime lifetime)
            where TService : class
            where TConcrete : class, TService
        {
            resolver.AddDependency(typeof(TService), typeof(TConcrete), lifetime);
        }

        public static void AddDependencyInstance(this IDependencyResolver resolver, Type serviceType, object instance)
        {
            resolver.AddDependencyInstance(serviceType, instance, DependencyLifetime.Singleton);
        }

        public static void AddDependencyInstance<TService>(this IDependencyResolver resolver, object instance)
        {
            resolver.AddDependencyInstance(typeof(TService), instance);
        }

        public static void AddDependencyInstance<TService>(this IDependencyResolver resolver, object instance, DependencyLifetime lifetime)
        {
            resolver.AddDependencyInstance(typeof(TService), instance, lifetime);
        }

        public static bool HasDependency<T>(this IDependencyResolver resolver)
        {
            return resolver.HasDependency(typeof(T));
        }

        /// <summary>
        /// Returns an instance of a registered dependency of type T.
        /// </summary>
        /// <typeparam name="T">The dependency type.</typeparam>
        /// <param name="resolver"></param>
        /// <returns>An instance of T.</returns>
        /// <exception cref="DependencyResolutionException">The resolver couldn't resolve the exception.</exception>
        public static T Resolve<T>(this IDependencyResolver resolver)
            where T : class
        {
            return resolver.Resolve(typeof(T)) as T;
        }

        public static T Resolve<T>(this IDependencyResolver resolver, UnregisteredAction unregistered)
            where T : class
        {
            return (T)resolver.Resolve(typeof(T), unregistered);
        }

        public static object Resolve(this IDependencyResolver resolver, Type type, UnregisteredAction unregisteredBehavior)
        {
            if (unregisteredBehavior == UnregisteredAction.AddAsTransient && !resolver.HasDependency(type))
                resolver.AddDependency(type, DependencyLifetime.Transient);
            return resolver.Resolve(type);
        }

        public static T ResolveWithDefault<T>(this IDependencyResolver resolver, Func<T> defaultValue)
            where T : class
        {
            return resolver.HasDependency(typeof(T)) ? resolver.Resolve<T>() : defaultValue();
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