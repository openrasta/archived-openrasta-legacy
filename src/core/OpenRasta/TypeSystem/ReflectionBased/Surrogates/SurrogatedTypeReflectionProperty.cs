using System;
using System.Reflection;

namespace OpenRasta.TypeSystem.ReflectionBased.Surrogates
{
    public class SurrogatedTypeReflectionProperty : ReflectionBasedProperty
    {
        SurrogatedPropertyProvider _propertyProvider;

        public SurrogatedTypeReflectionProperty(Type surrogateType, ReflectionBasedProperty accessor)
            :base(accessor.Owner,accessor.Property,accessor.PropertyParameters)
        {
            _propertyProvider = new SurrogatedPropertyProvider(surrogateType, accessor, this);
        }



        public override IProperty GetLocalIndexer(string indexerParameter)
        {
            return _propertyProvider.GetLocalIndexer(indexerParameter);
        }

        public override IProperty GetLocalProperty(string propertyName)
        {
            return _propertyProvider.GetLocalProperty(propertyName);
        }
    }
}