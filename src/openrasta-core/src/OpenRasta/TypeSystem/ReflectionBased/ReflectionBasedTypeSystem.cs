using System;
using System.Collections.Generic;
using System.Threading;

namespace OpenRasta.TypeSystem.ReflectionBased
{
    public class ReflectionBasedTypeSystem : ITypeSystem
    {
        static readonly IDictionary<Type, IType> _cache = new Dictionary<Type, IType>();
        readonly Stack<Type> _recursionDefender = new Stack<Type>();

        public ReflectionBasedTypeSystem()
        {
            SurrogateProvider = null;
            PathManager = new PathManager();
        }

        public ReflectionBasedTypeSystem(ISurrogateProvider surrogateProvider, IPathManager pathManager)
        {
            SurrogateProvider = surrogateProvider;
            PathManager = pathManager;
        }

        public IPathManager PathManager { get; private set; }
        public ISurrogateProvider SurrogateProvider { get; private set; }

        public IType FromClr(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            IType result;
            if (!_cache.TryGetValue(type, out result))
            {
                lock (_cache)
                {
                    // if (_recursionDefender.Contains(type))
                    // throw new RecursionException();
                    try
                    {
                        _recursionDefender.Push(type);
                        Thread.MemoryBarrier();
                        if (!_cache.TryGetValue(type, out result))
                        {
                            var typeAccessor = new ReflectionBasedType(this, type);

                            // write the temporary type in the cache to avoid recursion
                            _cache[type] = typeAccessor;
                            result = SurrogateProvider != null
                                         ? (SurrogateProvider.FindSurrogate((IType)typeAccessor) ?? typeAccessor)
                                         : typeAccessor;

                            // and update
                            _cache[type] = result;
                        }
                    }
                    finally
                    {
                        _recursionDefender.Pop();
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