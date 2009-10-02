using System.Linq.Expressions;
using OpenRasta.Reflection;

namespace OpenRasta.Codecs.SharpView.Visitors
{
    public class ChildScopeForEachFinder : ExpressionVisitor
    {
        MethodCallExpression _currentScopeIndexer;
        bool _scopeFound;
        public Expression HtmlBuilder { get; private set; }
        public Expression Iterator { get; private set; }
        public Expression ScopeToRewrite { get; private set; }

        public bool FindNextScopeForRewriting(Expression expr)
        {
            Visit(expr);
            return _scopeFound;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (!_scopeFound)
            {
                if (m.IsContentIndexer())
                {
                    _currentScopeIndexer = m;
                    var finder = new ScopedForEachFinder();
                    Expression rewrittenIndexerChild = finder.FindIterator(m.Arguments[0]);
                    if (finder.Iterator != null)
                    {
                        _scopeFound = true;
                        ScopeToRewrite = _currentScopeIndexer;
                        HtmlBuilder = rewrittenIndexerChild;
                        Iterator = finder.Iterator;
                    }
                }
            }
            return base.VisitMethodCall(m);
        }
    }
}