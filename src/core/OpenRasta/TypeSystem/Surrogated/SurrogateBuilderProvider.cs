using System;
using System.Linq;
using OpenRasta.DI;
using OpenRasta.TypeSystem.Surrogates;

namespace OpenRasta.TypeSystem.Surrogated
{
    public class SurrogateBuilderProvider : ISurrogateProvider
    {
        readonly ISurrogateBuilder[] _builders;

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
            var alienTypes = _builders.Where(x => x.CanCreateFor(property)).Select(x => x.Create(property)).ToList();
            if (alienTypes.Count > 0)
            {
                return new PropertyWithSurrogates(property, alienTypes);
            }

            return property;
        }

        IType FindTypeSurrogate(IType type)
        {
            var surrogates = _builders.Where(x => x.CanCreateFor(type)).Select(x => x.Create(type)).ToList();
            if (surrogates.Count > 0)
            {
                return new TypeWithSurrogates(type, surrogates);
            }

            return type;
        }
    }
}