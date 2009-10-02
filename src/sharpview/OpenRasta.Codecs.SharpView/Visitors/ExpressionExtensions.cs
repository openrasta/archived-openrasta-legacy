using System;
using System.Linq.Expressions;
using OpenRasta.Web.Markup;
using OpenRasta.Web.Markup.Modules;

namespace OpenRasta.Codecs.SharpView.Visitors
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<IElement>> ApplySharpView(this Expression<Func<IElement>> expression)
        {
            int scopeLevel = 0;
            Expression<Func<IElement>> rewrittenExpression = new ScopedForEachFinder(scopeLevel).RewriteRoot(expression);

            ChildScopeForEachFinder childScopeFinder;
            while ((childScopeFinder = new ChildScopeForEachFinder()).FindNextScopeForRewriting(rewrittenExpression))
            {
                rewrittenExpression = (Expression<Func<IElement>>)
                                      new ChildScopeRewriterVisitor(scopeLevel,
                                                                    childScopeFinder.ScopeToRewrite,
                                                                    childScopeFinder.Iterator,
                                                                    childScopeFinder.HtmlBuilder).
                                          RewriteScopeToMethodCall(rewrittenExpression);
                scopeLevel++;
            }

            return
                (Expression<Func<IElement>>)
                new IfRewriterVisitor().Rewrite(new SelectHtmlPropertyPathRewriter().Rewrite(rewrittenExpression));
        }

        public static bool IsContentIndexer(this MethodCallExpression expr)
        {
            return expr.Object != null && typeof(IContentModel).IsAssignableFrom(expr.Object.Type) &&
                   expr.Method.Name == "get_Item";
        }

        public static bool IsCurrentMethod(this MethodCallExpression expr)
        {
            return expr.Method.DeclaringType == typeof(SourcedElementExtensions) && expr.Method.Name == "Current";
        }

        public static bool IsForEachMethod(this MethodCallExpression expr)
        {
            return expr.Method.DeclaringType == typeof(SourcedElementExtensions) && expr.Method.Name == "ForEach";
        }

        public static bool IsIfMethod(this MethodCallExpression m)
        {
            return m != null && m.Method.DeclaringType == typeof(SourcedElementExtensions) && m.Method.Name == "If";
        }

        public static bool IsSelectHtmlMethod(this MethodCallExpression expr)
        {
            return expr.Method.DeclaringType == typeof(SourcedElementExtensions) && expr.Method.Name == "SelectHtml";
        }

        public static Expression RemoveCastToObject(this Expression expression)
        {
            if (expression.NodeType == ExpressionType.Convert)
            {
                var convert = (UnaryExpression)expression;
                if (convert.Type == typeof(object) && convert.Operand.Type == typeof(bool)) return convert.Operand;
            }
            return expression;
        }
    }
}