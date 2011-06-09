using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenRasta.DI.StructureMap
{
    public class StructureMapDependencyResolverFactory : IDependencyResolverAccessor
    {
        public IDependencyResolver Resolver
        {
            get { return new StructureMapDependencyResolver(); }
        }
    }
}
