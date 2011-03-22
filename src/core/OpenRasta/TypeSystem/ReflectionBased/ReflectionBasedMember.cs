using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace OpenRasta.TypeSystem.ReflectionBased
{
    public abstract class ReflectionBasedMember<T> : IMember
        where T : IMemberBuilder
    {
        readonly Dictionary<string, IProperty> _propertiesCachedByPath =
            new Dictionary<string, IProperty>(StringComparer.OrdinalIgnoreCase);

        readonly object _syncRoot = new object();

        IType _memberType;

        ILookup<string, IMethod> _methodsCache;

        protected ReflectionBasedMember(ITypeSystem typeSystem, Type targetType)
        {
            TypeSystem = typeSystem;
            SurrogateProvider = typeSystem.SurrogateProvider;
            PathManager = typeSystem.PathManager;
            TargetType = targetType;
        }

        public virtual bool IsEnumerable
        {
            get { return TargetType.IsArray || (TargetType.Implements(typeof(IEnumerable<>)) && !TargetType.Implements(typeof(IDictionary<,>))); }
        }

        public virtual string Name
        {
            get { return TargetType.Name; }
        }

        public Type StaticType
        {
            get { return TargetType; }
        }

        public IPathManager PathManager { get; set; }

        public ISurrogateProvider SurrogateProvider { get; set; }

        public Type TargetType { get; set; }

        public virtual IType Type
        {
            get
            {
                if (_memberType == null)
                {
                    lock (_syncRoot)
                    {
                        Thread.MemoryBarrier();
                        if (_memberType == null)
                            _memberType = TypeSystem.FromClr(TargetType);
                    }
                }

                return _memberType;
            }
        }

        public virtual string TypeName
        {
            get { return TargetType.Name; }
        }

        public ITypeSystem TypeSystem { get; set; }

        public TAttribute FindAttribute<TAttribute>() where TAttribute : class
        {
            return FindAttributes<TAttribute>().FirstOrDefault();
        }

        public virtual IEnumerable<TAttribute> FindAttributes<TAttribute>() where TAttribute : class
        {
            return Attribute.GetCustomAttributes(TargetType, true).OfType<TAttribute>();
        }

        public virtual bool CanSetValue(object value)
        {
            return
                (TargetType.IsValueType && value != null && TargetType.IsAssignableFrom(value.GetType()))
                || (!TargetType.IsValueType && (value == null || TargetType.IsAssignableFrom(value.GetType())));
        }

        public virtual IProperty GetIndexer(string indexerParameter)
        {
            var indexer = TargetType.FindIndexers(1).FindIndexer(indexerParameter);

            return indexer != null ? SurrogateProperty(new ReflectionBasedProperty(TypeSystem, this, indexer.Value.Key, indexer.Value.Value)) : null;
        }

        public IMethod GetMethod(string methodName)
        {
            VerifyMethodsInitialized();

            return _methodsCache.Contains(methodName) ? _methodsCache[methodName].FirstOrDefault() : null;
        }

        public IList<IMethod> GetMethods()
        {
            VerifyMethodsInitialized();
            return _methodsCache.SelectMany(x => x).ToList().AsReadOnly();
        }

        public virtual IProperty GetProperty(string propertyName)
        {
            lock (_syncRoot)
            {
                if (_propertiesCachedByPath.ContainsKey(propertyName))
                    return _propertiesCachedByPath[propertyName];
                var pi = TargetType.FindPropertyCaseInvariant(propertyName);
                if (pi == null)
                    return null;

                var pa = SurrogateProperty(new ReflectionBasedProperty(TypeSystem, this, pi, null));
                _propertiesCachedByPath.Add(propertyName, pa);
                return pa;
            }
        }

        IProperty SurrogateProperty(IProperty property)
        {
            if (TypeSystem.SurrogateProvider == null) return property;
            return TypeSystem.SurrogateProvider.FindSurrogate(property);
        }

        void VerifyMethodsInitialized()
        {
            if (_methodsCache == null)
            {
                lock (_syncRoot)
                {
                    Thread.MemoryBarrier();
                    if (_methodsCache == null)
                    {
                        var allProperties = TargetType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                        _methodsCache = (from method in TargetType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                                         where !allProperties.Any(x => x.GetGetMethod() == method || x.GetSetMethod() == method)
                                         select new ReflectionBasedMethod(TypeSystem.FromClr(method.DeclaringType), method) as IMethod)
                            .ToLookup(x => x.Name, StringComparer.OrdinalIgnoreCase);
                    }
                }
            }
        }
    }
}