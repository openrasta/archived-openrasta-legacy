using System;
using OpenRasta.TypeSystem;

namespace OpenRasta.Configuration.MetaModel.Handlers
{
    public class TypeRewriterMetaModelHandler : AbstractMetaModelHandler
    {
        readonly ITypeSystem _typeSystem;

        public TypeRewriterMetaModelHandler(ITypeSystem typeSystem)
        {
            _typeSystem = typeSystem;
        }

        public override void PreProcess(IMetaModelRepository repository)
        {
            foreach (var resource in repository.ResourceRegistrations)
                if (resource.ResourceKey is Type)
                    resource.ResourceKey = _typeSystem.FromClr((Type)resource.ResourceKey);
        }
    }
}