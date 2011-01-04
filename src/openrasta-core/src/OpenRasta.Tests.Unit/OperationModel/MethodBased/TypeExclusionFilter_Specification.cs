using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenRasta.Testing;
using OpenRasta.TypeSystem;
using OpenRasta.OperationModel.MethodBased;

namespace OpenRasta.Tests.Unit.OperationModel.MethodBased
{
    public class when_filtering_methods_by_type : openrasta_context
    {
        [Test]
        public void the_methods_from_the_type_are_excluded()
        {
            var filteredMethods = new TypeExclusionMethodFilter<when_filtering_methods_by_type>()
                .Filter(TypeSystem.FromClr<when_filtering_methods_by_type>().GetMethods());
            filteredMethods.SingleOrDefault(x => x.Name == "the_methods_from_the_type_are_excluded")
                .ShouldBeNull();
        }
    }
}
