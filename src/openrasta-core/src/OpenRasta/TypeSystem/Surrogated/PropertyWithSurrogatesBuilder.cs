using System.Collections.Generic;
using System.Linq;
using OpenRasta.TypeSystem.Surrogates;

namespace OpenRasta.TypeSystem.Surrogated
{
    public class PropertyWithSurrogatesBuilder : PropertyBuilder, IKeepSurrogateInstances
    {
        public PropertyWithSurrogatesBuilder(IProperty property, IMemberBuilder parent, IEnumerable<IType> alienTypes)
            : base(parent, property)
        {
            Surrogates = alienTypes.ToDictionary(x => (IMember)x, x => (ISurrogate)x.CreateInstance());
        }

        public IDictionary<IMember, ISurrogate> Surrogates { get; private set; }
    }
}