using System;
using System.Linq.Expressions;

namespace OpenRasta.Reflection
{
    public class PropertyPathForInstance<TProperty> : PropertyPathExpressionTree
    {
        public new TProperty Value { get { return (TProperty) base.Value; } }
        
        public PropertyPathForInstance(Expression<Func<TProperty>> instanceProperty)
        {

            ProcessMemberAccess(instanceProperty);
            try
            {
                var accessor = instanceProperty.Compile();
                base.Value = accessor();
            }
            catch (NullReferenceException)
            {
            }
            
        }
    }
}