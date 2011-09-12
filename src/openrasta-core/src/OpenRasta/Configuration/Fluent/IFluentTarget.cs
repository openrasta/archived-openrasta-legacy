using OpenRasta.Configuration.MetaModel;
using OpenRasta.TypeSystem;

namespace OpenRasta.Configuration.Fluent
{
    public interface IFluentTarget : INoIzObject
    {
        IMetaModelRepository Repository { get; }
        ITypeSystem TypeSystem { get; }
    }
}