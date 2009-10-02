using System;

namespace OpenRasta.TypeSystem.ReflectionBased.Surrogates
{
    public class SurrogatedPropertyProvider
    {
        readonly Type _surrogateType;
        readonly IMember _wrappedProperty;
        readonly IMember _owner;

        public SurrogatedPropertyProvider(Type surrogateType, IMember wrappedProperty, IMember owner)
        {
            if (surrogateType == null) throw new ArgumentNullException("surrogateType");
            if (wrappedProperty == null) throw new ArgumentNullException("wrappedProperty");
            if (owner == null) throw new ArgumentNullException("owner");
            _surrogateType = surrogateType;
            _owner = owner;
            _wrappedProperty = wrappedProperty;
        }

        public IProperty GetLocalIndexer(string indexerParameter)
        {
            if (string.IsNullOrEmpty(indexerParameter)) return null;
            var originalIndexer = _wrappedProperty.GetLocalIndexer(indexerParameter);
            var surrogateIndexer = _surrogateType.FindIndexers(1).FindIndexer(indexerParameter);
            if (surrogateIndexer == null)
            {
                return originalIndexer;
            }
            return new SurrogatedReflectionProperty(
                _surrogateType,
                surrogateIndexer.Value.Key,
                _owner,
                originalIndexer,
                surrogateIndexer.Value.Value);
        }
        public IProperty GetLocalProperty(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return null;
            var originalProperty = _wrappedProperty.GetLocalProperty(propertyName);
            var surrogateProperty = _surrogateType.FindPropertyCaseInvariant(propertyName);
            if (surrogateProperty == null)
                return originalProperty;
            return new SurrogatedReflectionProperty(
                _surrogateType,
                surrogateProperty,
                _owner,
                originalProperty,
                null);
            
        }
    }
}