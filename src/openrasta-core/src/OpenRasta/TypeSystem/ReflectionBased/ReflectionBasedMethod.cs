using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenRasta.TypeSystem.ReflectionBased
{
    public class ReflectionBasedMethod : IMethod
    {
        readonly MethodInfo _methodInfo;
        readonly object _syncRoot = new object();

        internal ReflectionBasedMethod(IMember ownerType, MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
            Owner = ownerType;
            TypeSystem = TypeSystems.Default;
            EnsureInputMembersExist();
            EnsureOutputMembersExist();
        }

        public IEnumerable<IParameter> InputMembers { get; private set; }

        public string Name
        {
            get { return _methodInfo.Name; }
        }

        public IEnumerable<IMember> OutputMembers { get; private set; }

        public IMember Owner { get; set; }
        public ITypeSystem TypeSystem { get; set; }

        public override string ToString()
        {
            return "{0}::{1}({2})".With(Owner.TypeName, _methodInfo.Name, string.Join(", ", _methodInfo.GetParameters().Select(x => "{0} {1}".With(x.ParameterType.Name, x.Name)).ToArray()));
        }

        public T FindAttribute<T>() where T : class
        {
            return FindAttributes<T>().FirstOrDefault();
        }

        public IEnumerable<T> FindAttributes<T>() where T : class
        {
            return _methodInfo.GetCustomAttributes(typeof(T), true).Cast<T>();
        }

        public IEnumerable<object> Invoke(object target, params object[] members)
        {
            return new[]{ _methodInfo.Invoke(target, members) };
        }

        void EnsureInputMembersExist()
        {
            if (InputMembers == null)
            {
                InputMembers = _methodInfo.GetParameters()
                    .Where(x => !x.IsOut)
                    .Select(x => (IParameter)new ReflectionBasedParameter(this, x)).ToList().AsReadOnly();
            }
        }

        void EnsureOutputMembersExist()
        {
            if (OutputMembers == null)
            {
                var outputParameters = new List<IMember>();
                outputParameters.Add(TypeSystem.FromClr(_methodInfo.ReturnType));
                foreach (var outOrRefParameter in InputMembers.Where(x => x.IsOutput))
                    outputParameters.Add(outOrRefParameter);
                OutputMembers = outputParameters.AsReadOnly();
            }
        }
    }
}