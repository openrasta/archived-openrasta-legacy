using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;

namespace OpenRasta.DI.StructureMap
{
	public class StructureMapDependencyResolver : DependencyResolverCore, IDependencyResolver
	{
		private readonly IContainer _container;

		public StructureMapDependencyResolver()
			: this(ObjectFactory.Container)
		{
		}

		public StructureMapDependencyResolver(IContainer container)
		{
			_container = container;
		}

		protected override void AddDependencyCore(Type serviceType, Type concreteType, DependencyLifetime lifetime)
		{
			_container.Configure(cfg => cfg.For(serviceType).LifecycleIs(GetLifecycle(lifetime)).Use(concreteType));
		}

		private static InstanceScope GetLifecycle(DependencyLifetime lifetime)
		{
			switch (lifetime)
			{
				case DependencyLifetime.PerRequest:
					return InstanceScope.HttpContext;
				case DependencyLifetime.Singleton:
					return InstanceScope.Singleton;
				case DependencyLifetime.Transient:
					return InstanceScope.Transient;
				default:
					return InstanceScope.HttpContext;
			}
		}

		protected override void AddDependencyCore(Type concreteType, DependencyLifetime lifetime)
		{
			_container.Configure(cfg => cfg.For(concreteType).LifecycleIs(GetLifecycle(lifetime)).Use(concreteType));
		}

		protected override void AddDependencyInstanceCore(Type serviceType, object instance, DependencyLifetime lifetime)
		{
			_container.Configure(cfg => cfg.For(serviceType).LifecycleIs(GetLifecycle(lifetime)).Use(instance));
		}

		protected override IEnumerable<TService> ResolveAllCore<TService>()
		{
			return _container.GetAllInstances<TService>();
		}

		protected override object ResolveCore(Type serviceType)
		{
			return _container.GetInstance(serviceType);
		}

		public bool HasDependency(Type serviceType)
		{
			return _container.TryGetInstance(serviceType) != null;
		}

		public bool HasDependencyImplementation(Type serviceType, Type concreteType)
		{
			if (!_container.Model.HasImplementationsFor(serviceType))
				return false;

			return _container.Model.For(serviceType).Instances.Any(i => i.ConcreteType == concreteType);
		}

		public void HandleIncomingRequestProcessed()
		{
			// meh
		}
	}
}