using System;
using System.Collections.Generic;
using OpenRasta.Binding;

namespace OpenRasta.TypeSystem
{
    public abstract class MemberBuilder : IMemberBuilder
    {
        protected readonly Dictionary<string, IPropertyBuilder> _propertiesCache =
            new Dictionary<string, IPropertyBuilder>(StringComparer.OrdinalIgnoreCase);

        protected MemberBuilder(IMember member)
        {
            Member = member;
        }

        public IMember Member { get; private set; }
        public abstract object Value
        {
            get;
        }

        public abstract bool HasValue { get; }

        public virtual bool CanWrite
        {
            get { return true; }
        }
        public virtual IPropertyBuilder GetProperty(string propertyPath)
        {
            if (_propertiesCache.ContainsKey(propertyPath))
                return _propertiesCache[propertyPath];

            lock (_propertiesCache)
            {
                IProperty property = Member.GetProperty(propertyPath);
                IPropertyBuilder propertyBuilder = property != null ? property.CreateBuilder() : null;
                if (propertyBuilder != null)
                    propertyBuilder.IndexAtCreation = _propertiesCache.Count;
                _propertiesCache.Add(propertyPath, propertyBuilder);
                return propertyBuilder;
            }
        }

        public abstract bool TrySetValue(object value);

        public abstract bool TrySetValue<T>(IEnumerable<T> values, ValueConverter<T> converter);


    }
}