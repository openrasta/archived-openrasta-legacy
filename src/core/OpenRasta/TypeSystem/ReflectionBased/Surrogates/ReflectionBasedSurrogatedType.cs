using System;

namespace OpenRasta.TypeSystem.ReflectionBased.Surrogates
{
    public class ReflectionBasedSurrogatedType : ReflectionBasedType
    {
        readonly SurrogatedPropertyProvider _propertyProvider;

        public ReflectionBasedSurrogatedType(Type wrapperType, ReflectionBasedMember<ITypeBuilder> wrapped)
            : base(wrapped.TargetType)
        {
            if (wrapperType == null) throw new ArgumentNullException("wrapperType");
            if (wrapped == null) throw new ArgumentNullException("wrapped");
            _propertyProvider = new SurrogatedPropertyProvider(wrapperType, wrapped, this);
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