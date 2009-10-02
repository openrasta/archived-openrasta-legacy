using System.Reflection;
using OpenRasta.TypeSystem.ReflectionBased;

namespace OpenRasta.TypeSystem
{
    public class ReflectionParameter : ReflectionBasedType
    {
        public ParameterInfo ParameterInfo { get; set; }

        public object DefaultValue { get { return ParameterInfo.DefaultValue; } }
        public bool IsOptional { get { return ParameterInfo.IsOptional; } }

        public ReflectionParameter(ParameterInfo info)
            : base(info.ParameterType)
        {
            ParameterInfo = info;
        }
    }
}