using System;
using System.Collections.Generic;
using System.Linq;
using OpenRasta.TypeSystem;

namespace OpenRasta.OperationModel.MethodBased
{
    public class TypeExclusionMethodFilter<T> : IMethodFilter
    {
        public TypeExclusionMethodFilter()
        {
            TypeSystem = TypeSystems.Default;
        }

        public ITypeSystem TypeSystem { get; set; }

        public IEnumerable<IMethod> Filter(IEnumerable<IMethod> methods)
        {
            var type = TypeSystem.FromClr<T>();

            return from method in methods
                   where method.Owner.Type.CompareTo(type) != 0
                   select method;
        }
    }
}