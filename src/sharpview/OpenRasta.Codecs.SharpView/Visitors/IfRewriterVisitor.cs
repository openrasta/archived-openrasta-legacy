#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using OpenRasta.Reflection;
using OpenRasta.TypeSystem.ReflectionBased;

namespace OpenRasta.Codecs.SharpView.Visitors
{
    /// <summary>
    /// Rewrites the If extension method.
    /// </summary>
    public class IfRewriterVisitor : ExpressionVisitor
    {
        public Expression Rewrite(Expression expression)
        {
            Expression collapsed = new MemberAccessOnIfCollapser().CollapseIfs(expression);
            Expression result = Visit(collapsed);
            return result;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.IsIfMethod())
            {
                Expression predicate = m.Arguments[1].RemoveCastToObject();
                Expression result = new MemberAccessNullPropagateVisitor().Rewrite(predicate);

                BinaryExpression conditionPredicate = Expression.Equal(result, Expression.Constant(result.Type.GetDefaultValue(), result.Type));
                return Expression.Condition(conditionPredicate, Expression.Constant(m.Arguments[0].Type.GetDefaultValue(), m.Arguments[0].Type), m.Arguments[0]);
            }
            return base.VisitMethodCall(m);
        }

        class MemberAccessOnIfCollapser : ExpressionVisitor
        {
            public Expression CollapseIfs(Expression e)
            {
                return Visit(e);
            }

            protected override Expression VisitMethodCall(MethodCallExpression m)
            {
                // rewrite instance methods
                // aka element.If(condition)["child"] = SEE.If(element,condition)["child"] -> SEE.If(element["child"],condition)
                var rewrittenMember = Visit(m.Object);
                var site = rewrittenMember as MethodCallExpression;

                if (site != null && site.IsIfMethod())
                {
                    Expression htmlElement = Visit(site.Arguments[0]);
                    MethodCallExpression newMethod = Expression.Call(htmlElement, m.Method, m.Arguments);
                    return Expression.Call(null, site.Method, newMethod, site.Arguments[1]);
                }

                // rewrite extension methods
                // aka element.If(condition).Class("class") = Ext.Class(SEE.If(element,condition),"class") -> SEE.If(Ext.Class(element,"class"),condition)
                if (m.Arguments.Count >= 1 && m.Object == null && (site = Visit(m.Arguments[0]) as MethodCallExpression) != null)
                {
                    if (site.IsIfMethod())
                    {
                        Expression htmlElement = Visit(site.Arguments[0]);
                        IEnumerable<Expression> newArgumentList = (new[] { htmlElement }).Concat(m.Arguments.Skip(1));
                        MethodCallExpression newExtensionMethod = Expression.Call(null, m.Method, newArgumentList);
                        MethodCallExpression newIfMethod = Expression.Call(null, site.Method, newExtensionMethod, site.Arguments[1]);
                        return newIfMethod;
                    }
                }

                return base.VisitMethodCall(m);
            }
        }
    }
}

#region Full license
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion