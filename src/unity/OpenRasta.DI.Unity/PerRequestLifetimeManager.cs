using System;
using Microsoft.Practices.Unity;
using OpenRasta.Pipeline;

namespace OpenRasta.DI.Unity
{
    /// <summary>
    /// This, combined with the context container management in <see cref="UnityDependencyResolver"/>,
    /// implements OpenRastas per request lifetime requirements.  One instance of this class is used
    /// per registered type.
    /// </summary>
    class PerRequestLifetimeManager : LifetimeManager
    {
        readonly IDependencyResolver resolver;
        readonly string key = Guid.NewGuid().ToString();

        IContextStore ContextStore
        {
            get { return resolver.Resolve<IContextStore>(); }
        }

        public PerRequestLifetimeManager(IDependencyResolver resolver)
        {
            this.resolver = resolver;
        }

        /// <summary>
        /// Fetch the current instance of this type from the current request.
        /// </summary>
        public override object GetValue()
        {
            if(!resolver.HasDependency<IContextStore>())
                throw new DependencyResolutionException("Context store not initialised.");

            return ContextStore[key];
        }

        /// <summary>
        /// Add the current instance of this type to the current request.
        /// </summary>
        public override void SetValue(object newValue)
        {
            ContextStore[key] = newValue;
        }

        public override void RemoveValue()
        {
            // Strangely required, but not used by Unity, go Microsoft!
            throw new NotSupportedException();
        }
    }
}
