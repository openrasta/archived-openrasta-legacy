using System;
using System.Collections.Generic;
using System.Linq;
using OpenRasta.Collections;
using OpenRasta.TypeSystem;

namespace OpenRasta.Handlers
{
    public class HandlerRepository : IHandlerRepository
    {
        readonly IDictionary<object, HashSet<IType>> _resourceHandlers = new NullBehaviorDictionary<object, HashSet<IType>>();

        public IEnumerable<IType> GetHandlerTypes()
        {
            return _resourceHandlers.SelectMany(x => x.Value).Distinct();
        }

        /// <exception cref="ArgumentException">The provided handler is already registered.</exception>
        public void AddResourceHandler(object resourceKey, IType handlerType)
        {
            if (resourceKey == null) throw new ArgumentNullException("resourceKey");
            if (resourceKey is Type) throw new ArgumentException("Cannot register a type as the key. Use an IType instead.", "resourceKey");
            var list = GetOrCreate(resourceKey);
            if (list.Contains(handlerType))
                throw new ArgumentException("The provided handler is already registered.", "handlerType");
            list.Add(handlerType);
        }

        public void Clear()
        {
            _resourceHandlers.Clear();
        }

        public IEnumerable<IType> GetHandlerTypesFor(object resourceKey)
        {
            if (resourceKey is Type)
                throw new ArgumentException("Type keys are not allowed. Use an IType instead.");
            return GetOrCreate(resourceKey);
        }


        HashSet<IType> GetOrCreate(object resourceKey)
        {
            HashSet<IType> handlerTypes;
            if (!_resourceHandlers.TryGetValue(resourceKey, out handlerTypes))
            {
                _resourceHandlers.Add(resourceKey, handlerTypes = new HashSet<IType>());
            }
            return handlerTypes;
        }
    }
}