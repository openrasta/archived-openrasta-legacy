using System;
using System.Linq.Expressions;
using OpenRasta.Reflection;
using OpenRasta.Web.Markup;
using OpenRasta.Web.Markup.Elements;

namespace OpenRasta.Codecs.SharpView.Visitors
{
    public class ScopedForEachFinder : ExpressionVisitor
    {
        readonly int _scopeLevel;

        public ScopedForEachFinder()
            : this(0)
        {
        }

        public ScopedForEachFinder(int scopeLevel)
        {
            _scopeLevel = scopeLevel;
        }

        public Expression Iterator { get; private set; }

        public Expression FindIterator(Expression expr)
        {
            return Visit(expr);
        }

        public Expression<Func<IElement>> RewriteRoot(Expression expr)
        {
            Expression rewritten = Visit(expr);
            if (Iterator != null)
            {
                var foreachrewriter = new ForEachRewriter(_scopeLevel, Iterator);
                rewritten = foreachrewriter.RewriteSubExpression(rewritten);

                if (foreachrewriter.LambdaParameter != null)
                    rewritten =
                        new CurrentItemRewriterVisitor(foreachrewriter.Iterator, foreachrewriter.LambdaParameter).Rewrite(rewritten);
                return (Expression<Func<IElement>>)CreateWrapperElement(rewritten);
            }
            return (Expression<Func<IElement>>)expr;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.IsContentIndexer())
            {
                Expression obj = Visit(m.Object);
                return Expression.Call(obj, m.Method, m.Arguments[0]);
            }
            if (Iterator == null && m.IsForEachMethod())
            {
                Iterator = m.Arguments[1].RemoveCastToObject();
                return m.Arguments[0];
            }
            return base.VisitMethodCall(m);
        }

        static Expression CreateWrapperElement(Expression expr)
        {
            NewExpression newElementExpr = Expression.New(typeof(EmptyElement));
            return
                Expression.Lambda<Func<IElement>>(
                    ChildScopeRewriterVisitor.CreateAddElementsOnHtmlElement(newElementExpr, expr));
        }
    }
}