using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using OpenRasta.Binding;
using OpenRasta.TypeSystem.ReflectionBased.Surrogates;

namespace OpenRasta.TypeSystem.ReflectionBased
{
    public abstract class ReflectionBasedMember<T> : IMember, INativeMember
        where T : IMemberBuilder
    {
        public IPathManager PathManager { get; set; }

        readonly Dictionary<string, IProperty> _propertiesCachedByPath =
            new Dictionary<string, IProperty>(StringComparer.OrdinalIgnoreCase);

        readonly object _syncRoot = new object();

        IType _memberType;

        ILookup<string, IMethod> _methodsCache;

        protected ReflectionBasedMember(Type targetType)
        {
            TypeSystem = new ReflectionBasedTypeSystem();
            TargetType = targetType;
            SurrogateFactory = new DefaultSurrogateFactory();
            PathManager = new PathManager();
        }
        public virtual bool IsCollection
        {
            get
            {
                return TargetType.IsArray || TargetType.Implements(typeof(IEnumerable<>));
            }
        }
        public virtual string Name
        {
            get { return TargetType.Name; }
        }

        public ISurrogateFactory SurrogateFactory { get; set; }

        public Type TargetType { get; set; }

        public IType Type
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

        public abstract T CreateBuilder();

        public virtual int CompareTo(IMember parent)
        {
            ReflectionBasedMember<T> otherReflection;
            if (parent == null || (otherReflection = parent as ReflectionBasedMember<T>) == null)
                return -1;
            return TargetType.GetInheritanceDistance(otherReflection.TargetType);
        }

        public virtual bool CanSetValue(object value)
        {
            return
                (TargetType.IsValueType && value != null && TargetType.IsAssignableFrom(value.GetType()))
                || (!TargetType.IsValueType && (value == null || TargetType.IsAssignableFrom(value.GetType())));
        }

        public TAttribute FindAttribute<TAttribute>() where TAttribute : class
        {
            return FindAttributes<TAttribute>().FirstOrDefault();
        }

        public IEnumerable<TAttribute> FindAttributes<TAttribute>() where TAttribute : class
        {
            return Attribute.GetCustomAttributes(TargetType, true).OfType<TAttribute>();
        }

        public virtual IProperty GetLocalIndexer(string indexerParameter)
        {
            KeyValuePair<PropertyInfo, object[]>? indexer = TargetType.FindIndexers(1).FindIndexer(indexerParameter);

            return indexer != null ? SurrogateIfNeeded(new ReflectionBasedProperty(this, indexer.Value.Key, indexer.Value.Value)) : null;
        }

        public virtual IProperty GetLocalProperty(string propertyName)
        {
            lock (_syncRoot)
            {
                if (_propertiesCachedByPath.ContainsKey(propertyName))
                    return _propertiesCachedByPath[propertyName];
                PropertyInfo pi = TargetType.FindPropertyCaseInvariant(propertyName);
                if (pi == null)
                    return null;

                IProperty pa = SurrogateIfNeeded(new ReflectionBasedProperty(this, pi, null));
                _propertiesCachedByPath.Add(propertyName, pa);
                return pa;
            }
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
            IMember sourceMember = this;
            IProperty property = null;
            foreach (var parseResult in PathManager.ReadComponents(propertyName).Where(x => x.Type != PathComponentType.None))
            {
                if (parseResult.Type == PathComponentType.Indexer)
                    property = sourceMember.GetLocalIndexer(parseResult.ParsedValue);
                else if (parseResult.Type == PathComponentType.Member)
                    property = sourceMember.GetLocalProperty(parseResult.ParsedValue);

                if (property == null)
                    return null;

                sourceMember = property;
            }

            return property;
        }

        public bool IsAssignableTo(IMember member)
        {
            return member != null && CompareTo(member) >= 0;
        }

        IMemberBuilder IMember.CreateBuilder()
        {
            return CreateBuilder();
        }

        IProperty SurrogateIfNeeded(IProperty property)
        {
            return SurrogateFactory != null ? SurrogateFactory.FindSurrogate(property) ?? property : property;
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

        public Type NativeType
        {
            get { return TargetType; }
        }
        public override string ToString()
        {
            return TargetType.ToString();
        }
    }
}