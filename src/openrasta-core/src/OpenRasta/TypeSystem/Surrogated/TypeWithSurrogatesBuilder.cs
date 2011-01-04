using System.Collections.Generic;
using System.Linq;
using OpenRasta.TypeSystem.Surrogates;

namespace OpenRasta.TypeSystem.Surrogated
{
    public class TypeWithSurrogatesBuilder : TypeBuilder, IKeepSurrogateInstances
    {
        public TypeWithSurrogatesBuilder(IType typeWithSurrogates, IEnumerable<IType> alienTypes)
            : base(typeWithSurrogates)
        {
            Surrogates = alienTypes.ToDictionary(x => (IMember)x, x => (ISurrogate)x.CreateInstance());
        }

        public IDictionary<IMember, ISurrogate> Surrogates { get; private set; }
    }
}