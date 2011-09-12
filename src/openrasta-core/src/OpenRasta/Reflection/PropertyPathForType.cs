using System;
using System.Linq.Expressions;

namespace OpenRasta.Reflection
{
    public class PropertyPathForType<TType, TProperty> : PropertyPathExpressionTree
    {
        public PropertyPathForType(Expression<Func<TType, TProperty>> property)
            : base(property)
        {
        }
    }
}