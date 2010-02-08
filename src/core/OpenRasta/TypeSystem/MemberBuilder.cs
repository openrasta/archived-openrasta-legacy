using System;
using System.Collections.Generic;
using System.Linq;
using OpenRasta.Binding;

namespace OpenRasta.TypeSystem
{
    public abstract class MemberBuilder : IMemberBuilder
    {
        protected readonly Dictionary<string, IPropertyBuilder> PropertiesCache =
            new Dictionary<string, IPropertyBuilder>(StringComparer.OrdinalIgnoreCase);

        protected MemberBuilder(IMemberBuilder parent, IMember member)
        {
            Parent = parent;
            Member = member;
            Assignment = new AssignmentFrame { Builder = this };
        }

        public virtual bool CanWrite
        {
            get { return true; }
        }

        public abstract bool HasValue { get; }

        public IMember Member { get; private set; }
        public IMemberBuilder Parent { get; set; }
        public abstract object Value { get; }
        protected AssignmentFrame Assignment { get; set; }
        public abstract object Apply(object target, out object assignedValue);

        public virtual IPropertyBuilder GetProperty(string propertyPath)
        {
            if (PropertiesCache.ContainsKey(propertyPath))
                return PropertiesCache[propertyPath];

            lock (PropertiesCache)
            {
                var property = Member.FindPropertyByPath(propertyPath);

                if (property == null)
                    return PropertiesCache[propertyPath] = null;

                var currentFrame = Assignment;
                foreach (var member in property.GetCallStack().OfType<IProperty>().Reverse())
                {
                    if (currentFrame.Children.ContainsKey(member))
                    {
                        currentFrame = currentFrame.Children[member];
                        continue;
                    }

                    currentFrame.Children.Add(member, 
                                              currentFrame = new AssignmentFrame
                                              {
                                                  Builder = member.CreateBuilder(currentFrame.Builder)
                                              });
                }

                var lastProperty = (IPropertyBuilder)currentFrame.Builder;
                PropertiesCache[propertyPath] = lastProperty;
                return lastProperty;
            }
        }

        public abstract bool TrySetValue(object value);

        public abstract bool TrySetValue<T>(IEnumerable<T> values, ValueConverter<T> converter);

        protected class AssignmentFrame
        {
            public AssignmentFrame()
            {
                Children = new Dictionary<IProperty, AssignmentFrame>();
                Children = new Dictionary<IProperty, AssignmentFrame>();
            }

            public IMemberBuilder Builder { get; set; }

            public Dictionary<IProperty, AssignmentFrame> Children { get; set; }
        }
    }
}