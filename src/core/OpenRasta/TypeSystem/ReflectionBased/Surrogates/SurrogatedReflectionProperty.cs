using System;
using System.Reflection;

namespace OpenRasta.TypeSystem.ReflectionBased.Surrogates
{
    public class SurrogatedReflectionProperty : ReflectionBasedProperty
    {
        readonly Type _surrogateType;
        readonly IProperty _rewrittenProperty;

        public SurrogatedReflectionProperty(
            Type surrogateType, 
            PropertyInfo surrogateProperty,
            IMember owner, 
            IProperty rewrittenProperty, 
            object[] indexerParameters)
            : base(owner, surrogateProperty, indexerParameters)
        {
            _surrogateType = surrogateType;
            _rewrittenProperty = rewrittenProperty;
        }

        public override object GetValue(object target)
        {
            if (_surrogateType.IsAssignableFrom(target.GetType()))
                return base.GetValue(target);
            throw new InvalidOperationException();
        }
        public override IPropertyBuilder CreateBuilder()
        {
            return new SurrogatedPropertyInstance(_surrogateType, this);
        }
    }
}