using System;
using System.Collections.Generic;
using System.Linq;
using OpenRasta.DI;
using OpenRasta.TypeSystem.Surrogates;

namespace OpenRasta.TypeSystem.Surrogated
{
    public class SurrogateBuilderProvider : ISurrogateProvider
    {
        readonly ISurrogateBuilder[] _builders;
        static Dictionary<IType, IType> _typeCache = new Dictionary<IType, IType>();
        static Dictionary<IProperty, IProperty> _propCache = new Dictionary<IProperty, IProperty>();
        // HACK: Waiting to push Func<IEnumerable<T>> resolution
        // in container. Remove when done.
        public SurrogateBuilderProvider(IDependencyResolver resolver)
            : this(resolver.ResolveAll<ISurrogateBuilder>().ToArray())
        {
        }

        public SurrogateBuilderProvider(ISurrogateBuilder[] builders)
        {
            if (builders == null) throw new ArgumentNullException("builders");
            _builders = builders;
        }

        public T FindSurrogate<T>(T member) where T : IMember
        {
            var t = member as IType;
            if (t != null) return (T)FindTypeSurrogate(t);
            var p = member as IProperty;
            if (p != null) return (T)FindPropertySurrogate(p);

            return member;
        }

        IProperty FindPropertySurrogate(IProperty property)
        {
            return Cached(_propCache,
                          property,
                          p =>
                          {
                              var alienTypes = _builders.Where(x => x.CanCreateFor(p)).Select(x => x.Create(p)).ToList();
                              return alienTypes.Count > 0 ? new PropertyWithSurrogates(property, alienTypes) : property;
                          });
        }

        IType FindTypeSurrogate(IType type)
        {
            return Cached(_typeCache,
                          type,
                          t =>
                          {
                              var surrogates = _builders.Where(x => x.CanCreateFor(t)).Select(x => x.Create(t)).ToList();
                              return surrogates.Count > 0 ? new TypeWithSurrogates(type, surrogates) : type;
                          });

        }
        T Cached<T>(Dictionary<T, T> cache, T value, Func<T, T> createCached)
        {
            T cachedValue;
            if (cache.TryGetValue(value, out cachedValue))
                return cachedValue;
            lock (cache)
            {
                cachedValue = createCached(value);
                cache.Add(value, cachedValue);
                return cachedValue;
            }
        }
    }
}