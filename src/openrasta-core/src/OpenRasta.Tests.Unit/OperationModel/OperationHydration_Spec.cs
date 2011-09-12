using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using OpenRasta.DI;
using OpenRasta.OperationModel;
using OpenRasta.Pipeline.Contributors;
using OpenRasta.Testing;

namespace OperationHydration_Spec
{
    public class when_executing_operation_hydrators : context
    {
        [Test]
        public void the_hydrator_is_executed()
        {
            var processor = new Mock<IOperationHydrator>(MockBehavior.Strict);
            var operations = new[] { new Mock<IOperation>().Object };
            var resolver = new InternalDependencyResolver();
            processor.Expect(x => x.Process(It.IsAny<IEnumerable<IOperation>>())).Returns((IEnumerable<IOperation> op) => op);

            resolver.AddDependencyInstance<IOperationHydrator>(processor.Object);
            var contrib = new OperationHydratorContributor(resolver);
            contrib.ProcessOperations(operations);

            processor.VerifyAll();
        }
    }
}