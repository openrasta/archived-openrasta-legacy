using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenRasta.Binding;
using OpenRasta.Collections;
using OpenRasta.DI;
using OpenRasta.TypeSystem;

namespace OpenRasta.OperationModel.MethodBased
{
    public class MethodBasedOperation : IOperation
    {
        readonly IMethod _method;
        readonly IType _ownerType;

        readonly Dictionary<IParameter, IObjectBinder> _parameterBinders;

        public MethodBasedOperation(IObjectBinderLocator binderLocator, IType ownerType, IMethod method)
        {
            _method = method;
            _ownerType = ownerType;
            _parameterBinders = method.InputMembers.ToDictionary(x => x, x => binderLocator.GetBinder(x));
            Inputs = _parameterBinders.Select(x => new InputMember(x.Key, x.Value, x.Key.IsOptional));
            ExtendedProperties = new NullBehaviorDictionary<object, object>();
        }

        public IDictionary ExtendedProperties { get; private set; }
        public IEnumerable<InputMember> Inputs { get; private set; }

        public string Name
        {
            get { return _method.Name; }
        }

        public IDependencyResolver Resolver { get; set; }

        public override string ToString()
        {
            return _method.ToString();
        }

        public T FindAttribute<T>() where T : class
        {
            return _method.FindAttribute<T>() ?? _ownerType.FindAttribute<T>();
        }

        public IEnumerable<T> FindAttributes<T>() where T : class
        {
            return _ownerType.FindAttributes<T>().Concat(_method.FindAttributes<T>());
        }

        public IEnumerable<OutputMember> Invoke()
        {
            if (!Inputs.AllReady())
                throw new InvalidOperationException("The operation is not ready for invocation.");

            var handler = CreateInstance(_ownerType, Resolver);

            var bindingResults = from kv in _parameterBinders
                                 let param = kv.Key
                                 let binder = kv.Value
                                 select binder.IsEmpty
                                            ? BindingResult.Success(param.DefaultValue)
                                            : binder.BuildObject();

            var parameters = GetParameters(bindingResults);

            var result = _method.Invoke(handler, parameters.ToArray());

            // note this is only temporary until we implement out and ref support...
            if (_method.OutputMembers.Any())
            {
                return new[]
                {
                    new OutputMember
                    {
                        Member = _method.OutputMembers.Single(), 
                        Value = result.Single()
                    }
                };
            }
            return new OutputMember[0];
        }

        /// <summary>
        /// Returns an instance of the type, optionally through the container if it is supported.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="resolver"></param>
        /// <returns></returns>
        static object CreateInstance(IType type, IDependencyResolver resolver)
        {
            var typeForResolver = type as IResolverAwareType;
            return resolver == null || typeForResolver == null ? type.CreateInstance() : typeForResolver.CreateInstance(resolver);
        }

        IEnumerable<object> GetParameters(IEnumerable<BindingResult> results)
        {
            foreach (var result in results)
                if (!result.Successful)
                    throw new InvalidOperationException("A parameter wasn't successfully created.");
                else
                    yield return result.Instance;
        }
    }
}