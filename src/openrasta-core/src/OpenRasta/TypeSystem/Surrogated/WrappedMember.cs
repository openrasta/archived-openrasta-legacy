using System;
using System.Collections.Generic;

namespace OpenRasta.TypeSystem.Surrogated
{
    public abstract class WrappedMember : IMember, IHasWrappedMember
    {
        readonly IMember _wrapped;
        Dictionary<string,IProperty> _cachedProperty = new Dictionary<string, IProperty>();

        protected WrappedMember(IMember member)
        {
            _wrapped = member;
        }

        public Type StaticType
        {
            get { return _wrapped.StaticType; }
        }

        public virtual bool IsEnumerable
        {
            get { return _wrapped.IsEnumerable; }
        }

        public virtual string Name
        {
            get { return _wrapped.Name; }
        }

        public virtual IType Type
        {
            get { return _wrapped.Type; }
        }

        public virtual string TypeName
        {
            get { return _wrapped.TypeName; }
        }

        public virtual ITypeSystem TypeSystem
        {
            get { return _wrapped.TypeSystem; }
            set { _wrapped.TypeSystem = value; }
        }

        IMember IHasWrappedMember.WrappedMember
        {
            get { return _wrapped; }
        }

        public virtual T FindAttribute<T>() where T : class
        {
            return _wrapped.FindAttribute<T>();
        }

        public virtual IEnumerable<T> FindAttributes<T>() where T : class
        {
            return _wrapped.FindAttributes<T>();
        }

        public virtual bool CanSetValue(object value)
        {
            return _wrapped.CanSetValue(value);
        }

        public virtual IProperty GetIndexer(string parameter)
        {
            return CachedProperty(parameter, ()=> Reroot(_wrapped.GetIndexer(parameter)));
        }

        public virtual IProperty GetProperty(string name)
        {
            return CachedProperty(name, ()=> Reroot(_wrapped.GetProperty(name)));
        }

        public virtual IMethod GetMethod(string methodName)
        {
            return _wrapped.GetMethod(methodName);
        }

        public virtual IList<IMethod> GetMethods()
        {
            return _wrapped.GetMethods();
        }

        protected WrappedProperty Reroot(IProperty root)
        {
            if (root == null) return null;
            return new WrappedProperty(this, root);
        }

        protected IProperty CachedProperty(string parameter, Func<IProperty> propertyCreator)
        {
            IProperty output;
            if (!_cachedProperty.TryGetValue(parameter, out output))
                lock (_cachedProperty)
                    if (!_cachedProperty.TryGetValue(parameter, out output))
                        _cachedProperty[parameter] = output = propertyCreator();
            return output;
        }
    }
}