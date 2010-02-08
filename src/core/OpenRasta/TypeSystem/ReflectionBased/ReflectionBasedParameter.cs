using System;
using System.Reflection;

namespace OpenRasta.TypeSystem.ReflectionBased
{
    public class ReflectionBasedParameter : ReflectionBasedMember<IParameterBuilder>, IParameter
    {
        readonly ReflectionBasedMethod _ownerMethod;
        readonly ParameterInfo _parameterInfo;

        public ReflectionBasedParameter(ReflectionBasedMethod ownerMethod, ParameterInfo parameterInfo)
            : base(ownerMethod.TypeSystem, parameterInfo.ParameterType)
        {
            _ownerMethod = ownerMethod;
            _parameterInfo = parameterInfo;
        }

        public object DefaultValue
        {
            get
            {
                return _parameterInfo.DefaultValue == Missing.Value
                           ? _parameterInfo.ParameterType.GetDefaultValue()
                           : _parameterInfo.DefaultValue;
            }
        }

        public bool IsOutput
        {
            get { return _parameterInfo.IsOut; }
        }

        public bool IsOptional
        {
            get { return _parameterInfo.IsOptional; }
        }

        public override string Name
        {
            get { return _parameterInfo.Name; }
        }

        public IMethod Owner
        {
            get { return _ownerMethod; }
        }
    }
}