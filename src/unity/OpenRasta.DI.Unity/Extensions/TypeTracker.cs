using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using OpenRasta.DI.Unity.Extensions.Policies;

namespace OpenRasta.DI.Unity.Extensions
{
    /// <summary>
    /// Adds queries regarding the currently registered type to the Unity DI container.  This
    /// extension must be installed in any instance of <see cref="IUnityContainer"/> you want to 
    /// use with OpenRasta.
    /// </summary>
    /// <remarks>
    /// Named components are entirely ignored by this extension.
    /// </remarks>
    public class TypeTracker : UnityContainerExtension
    {
        readonly Dictionary<Type, HashSet<Type>> mappings = new Dictionary<Type, HashSet<Type>>();

        protected override void Initialize()
        {
            // When a new type is registered in the container, save the details for later.
            Context.Registering +=
                (sender, e) => AddMapping(e.Name, e.TypeFrom, e.TypeTo);

            Context.RegisteringInstance +=
                (sender, e) => AddMapping(e.Name, e.RegisteredType, e.Instance.GetType());

            // Make this type tracker available to others
            Context.Policies.SetDefault(new TypeTrackerPolicy(this));
        }

        void AddMapping(string name, Type typeFrom, Type typeTo)
        {
            // Ignore named components
            if (!string.IsNullOrEmpty(name))
                return;

            if (typeTo == null)
                typeTo = typeFrom;

            if (!mappings.ContainsKey(typeFrom))
                mappings[typeFrom] = new HashSet<Type>();

            if(!mappings.ContainsKey(typeTo))
                mappings[typeTo] = new HashSet<Type>();

            mappings[typeFrom].Add(typeTo);
            mappings[typeTo].Add(typeTo);
        }

        /// <summary>
        /// Access the TypeTracker installed in our parent container.  This is necesary because we
        /// are only notified about types registered in our container specifically, not ones added
        /// to parent or child container.
        /// </summary>
        TypeTracker Parent
        {
            get
            {
                if (Container.Parent == null)
                    return null;

                return Container.Parent.TypeTracker();
            }
        }

        /// <summary>
        /// Do we know of any implementations for the given type at all?
        /// </summary>
        public bool HasDependency(Type serviceType)
        {
            if (mappings.ContainsKey(serviceType))
                return true;

            if(Parent == null)
                return false;

            return Parent.HasDependency(serviceType);
        }

        /// <summary>
        /// Do we know of a specific implementation of the given type?
        /// </summary>
        public bool HasDependencyImplementation(Type serviceType, Type concreteType)
        {
            if(mappings.ContainsKey(serviceType))
            {
                foreach (var type in mappings[serviceType])
                {
                    if(type == concreteType)
                        return true;
                }
            }

            if (Parent != null)
                return Parent.HasDependencyImplementation(serviceType, concreteType);

            return false;
        }

        /// <summary>
        /// Find all the possible implementations of the given type.
        /// </summary>
        public IEnumerable<Type> GetAllConcreteTypes(Type serviceType)
        {
            var types = new HashSet<Type>();

            if (mappings.ContainsKey(serviceType))
                types.UnionWith(mappings[serviceType]);

            if(Parent != null)
                types.UnionWith(Parent.GetAllConcreteTypes(serviceType));

            return types;
        }
    }
}