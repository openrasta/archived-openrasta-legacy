using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using OpenRasta.Reflection;

namespace OpenRasta.Codecs.SharpView.Visitors
{
    public class ChildScopeRewriterVisitor : ExpressionVisitor
    {
        readonly Expression _htmlBuilder;
        readonly Expression _iterator;
        readonly Expression _scopeToRewrite;

        public ChildScopeRewriterVisitor(int scopeLevel,
                                         Expression scopeToRewrite,
                                         Expression iterator,
                                         Expression htmlBuilder)
        {
            ScopeLevel = scopeLevel;
            _scopeToRewrite = scopeToRewrite;
            _htmlBuilder = htmlBuilder;
            _iterator = iterator;
        }

        int ScopeLevel { get; set; }

        public static MethodCallExpression CreateAddElementsOnHtmlElement(Expression htmlElement,
                                                                          Expression rewrittenExpression)
        {
            MethodInfo addElementsMethod =
                typeof(SourcedElementExtensions).GetMethods().Where(mi => mi.Name == "AddElements").First().
                    MakeGenericMethod(htmlElement.Type);
            return Expression.Call(null, addElementsMethod, htmlElement, rewrittenExpression);
        }

        public Expression RewriteScopeToMethodCall(Expression expr)
        {
            return Visit(expr);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m == _scopeToRewrite)
            {
                var forEachRewriterVisitor = new ForEachRewriter(ScopeLevel, _iterator);
                Expression rewrittenChild = forEachRewriterVisitor.RewriteSubExpression(_htmlBuilder);
                rewrittenChild =
                    new CurrentItemRewriterVisitor(_iterator, forEachRewriterVisitor.LambdaParameter).Rewrite(
                        rewrittenChild);
                return CreateAddElementsOnHtmlElement(m.Object, rewrittenChild);
            }

            return base.VisitMethodCall(m);
        }
    }
}