using System;
using OpenRasta.TypeSystem.Surrogates;

namespace OpenRasta.TypeSystem.Surrogated
{
    public class AlienPropertyBuilder : PropertyBuilder
    {
        readonly IKeepSurrogateInstances _parentBuilder;
        readonly ISurrogate _surrogatedTypeBuilder;

        public AlienPropertyBuilder(IMember owner, IMemberBuilder parentBuilder, IProperty alienProperty)
            : base(parentBuilder, alienProperty)
        {
            _parentBuilder = (IKeepSurrogateInstances)parentBuilder;
            _surrogatedTypeBuilder = _parentBuilder.Surrogates[owner];
        }

        public override object Apply(object target, out object assignedValue)
        {

            if (target == null)
                target = Owner.Member.Type.CreateInstance();

            _surrogatedTypeBuilder.Value = target;

            assignedValue = Value;
            if (Property.TrySetValue(_surrogatedTypeBuilder, assignedValue))
                return _surrogatedTypeBuilder.Value;

            throw new InvalidOperationException("An error has occurred while applying the changes.");
        }
    }
}