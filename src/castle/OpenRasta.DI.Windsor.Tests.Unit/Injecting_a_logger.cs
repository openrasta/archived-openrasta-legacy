using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;
using OpenRasta.Configuration;
using OpenRasta.DI;
using OpenRasta.DI.Windsor;
using OpenRasta.Diagnostics;

namespace WindsorDependencyResolver_Specification
{
	public class Injecting_a_logger
	{
		[Test]
		public void Provided_logger_is_not_overridden_by_defaults()
		{
			var container = new WindsorContainer();
			container.Register(Component.For<ILogger>().ImplementedBy<MyLogger>());
			var dependencyResolver = new WindsorDependencyResolver(container);
			var registrar = new TestDependencyRegistrar();
			registrar.RegisterLogging(dependencyResolver);

			var logger = container.Resolve(typeof(ILogger));

			Assert.That(logger, Is.InstanceOf(typeof(MyLogger)));
		}

		private class TestDependencyRegistrar : DefaultDependencyRegistrar
		{
			public new void RegisterLogging(IDependencyResolver resolver)
			{
				base.RegisterLogging(resolver);
			}
		}

		private class MyLogger : ILogger
		{
			public IDisposable Operation(object source, string name)
			{
				return new OperationCookie();
			}

			public void WriteDebug(string message, params object[] format) { }

			public void WriteWarning(string message, params object[] format) { }

			public void WriteError(string message, params object[] format) { }

			public void WriteInfo(string message, params object[] format) { }

			public void WriteException(Exception e) { }

			private class OperationCookie : IDisposable
			{
				public void Dispose() { }
			}
		}
	}
}