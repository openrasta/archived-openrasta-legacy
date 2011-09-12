using System.Linq.Expressions;
using System.Text;
using OpenRasta.Reflection;

namespace OpenRasta.Codecs.SharpView.Visitors
{
    public class PropertyPathForIteratorVisitor : PropertyPathVisitor
    {
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            base.VisitMethodCall(m);
            if (m.IsCurrentMethod())
            {
                PropertyPathBuilder = new StringBuilder();
                RootType = m.Method.ReturnType.Name;
            }
            return m;
        }
    }
}