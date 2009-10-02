using System.Linq.Expressions;
using OpenRasta.Reflection;

namespace OpenRasta.Codecs.SharpView.Visitors
{
    public class SelectHtmlPropertyPathRewriter : ExpressionVisitor
    {
        public Expression Rewrite(Expression expression)
        {
            return Visit(expression);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.IsSelectHtmlMethod())
            {
                Expression iterator = m.Arguments[0];
                Expression nullPropagatingIterator = new MemberAccessNullPropagateVisitor().Rewrite(iterator);

                PropertyPath iteratorPath = CreatePropertyPath(iterator);

                Expression iteratorRoot = GetIteratorRoot(iterator);

                Expression rootInstanceParameter = iteratorRoot ?? Expression.Constant(null, typeof(object));
                ConstantExpression propertyPathParameter = Expression.Constant(iteratorPath);

                MethodCallExpression callExpression = Expression.Call(m.Method,
                                                                      nullPropagatingIterator,
                                                                      rootInstanceParameter,
                                                                      propertyPathParameter,
                                                                      Visit(m.Arguments[3]));
                return callExpression;
            }
            return base.VisitMethodCall(m);
        }

        static PropertyPath CreatePropertyPath(Expression expression)
        {
            return new PropertyPathForIteratorVisitor().BuildPropertyPath(expression);
        }

        static Expression GetIteratorRoot(Expression expression)
        {
            return new IteratorRootFinder().FindRoot(expression);
        }

        class IteratorRootFinder : ExpressionVisitor
        {
            ParameterExpression _rootParameter;

            public Expression FindRoot(Expression expression)
            {
                Visit(expression);
                return _rootParameter;
            }

            protected override Expression VisitParameter(ParameterExpression p)
            {
                if (p.Name.StartsWith("__OR_foreachItem"))
                    _rootParameter = p;
                return base.VisitParameter(p);
            }
        }
    }
}