using System;
using System.Collections.Generic;
using OpenRasta.TypeSystem.ReflectionBased.Surrogates;

namespace OpenRasta.TypeSystem.ReflectionBased
{
    public class ReflectionBasedTypeSystem : ITypeSystem
    {
        static readonly IDictionary<Type, IType> _cache = new Dictionary<Type, IType>();

        public ReflectionBasedTypeSystem()
        {
            SurrogateFactory = new DefaultSurrogateFactory();
        }

        public ISurrogateFactory SurrogateFactory { get; set; }

        public IType FromClr(Type t)
        {
            if (t == null) throw new ArgumentNullException("t");
            IType result;
            if (!_cache.TryGetValue(t, out result))
            {
                lock (_cache)
                {
                    if (!_cache.TryGetValue(t, out result))
                    {
                        var typeAccessor = new ReflectionBasedType(t);
                        _cache.Add(t, result = (SurrogateFactory != null ? (SurrogateFactory.FindSurrogate(typeAccessor) ?? typeAccessor) : typeAccessor));
                    }
                }
            }
            return result;
        }


        public IType FromInstance(object instance)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            return FromClr(instance.GetType());
        }
    }
}