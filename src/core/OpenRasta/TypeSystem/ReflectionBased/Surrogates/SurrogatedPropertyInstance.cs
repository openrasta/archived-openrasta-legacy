using System;
using System.Collections.Generic;
using OpenRasta.Binding;

namespace OpenRasta.TypeSystem.ReflectionBased.Surrogates
{
    public class SurrogatedPropertyInstance : PropertyBuilder
    {
        readonly ISurrogate _surrogate;

        public SurrogatedPropertyInstance(Type surrogateType, ReflectionBasedProperty property)
            : base(property)
        {
            _surrogate = (ISurrogate)Activator.CreateInstance(surrogateType);
        }
        
        protected override bool SetValueOnParent(object value)
        {
            _surrogate.Value = Owner.Value ?? Owner.Member.Type.CreateInstance();
            if (Property.TrySetValue(_surrogate, value))
                return Owner.TrySetValue(_surrogate.Value);
            return false;
        }
        public override object Value
        {
            get
            {
                if (Owner != null)
                {
                    _surrogate.Value = Owner.Value;
                    return Property.GetValue(_surrogate);
                }
                return base.Value;
            }
        }
    }
}