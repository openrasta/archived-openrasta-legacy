using OpenRasta.Configuration.MetaModel;
using OpenRasta.TypeSystem;

namespace OpenRasta.Configuration.Fluent
{
    public interface IFluentTarget
    {
        IMetaModelRepository Repository { get; }
        ITypeSystem TypeSystem { get; }
    }
}