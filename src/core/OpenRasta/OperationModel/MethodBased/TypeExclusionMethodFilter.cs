using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using OpenRasta.TypeSystem;
using OpenRasta.TypeSystem.ReflectionBased;

namespace OpenRasta.OperationModel.MethodBased
{
    public class TypeExclusionMethodFilter<T> : IMethodFilter
    {
        public ITypeSystem TypeSystem { get; set; }

        public TypeExclusionMethodFilter()
        {
            TypeSystem = new ReflectionBasedTypeSystem();
        }
        public IEnumerable<IMethod> Filter(IEnumerable<IMethod> methods)
        {
            var type = TypeSystem.FromClr<T>();

            return from method in methods
                   where method.Owner.Type.CompareTo(type) != 0
                   select method;
        }
    }
}
