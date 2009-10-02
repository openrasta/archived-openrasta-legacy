#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using OpenRasta.Reflection;


namespace OpenRasta.Client
{
    public class QueryStringTranslator : ExpressionVisitor
    {
        StringBuilder _sb;

        public string Translate(Expression expression)
        {
            _sb = new StringBuilder();
            Visit(expression);
            return _sb.ToString();
        }
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
            {
                // ignore the argument
                // this.Visit(m.Arguments[0]);

                var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);

                Visit(lambda.Body);

                return m;
            }

            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
        }

        static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }

            return e;
        }
        protected override Expression VisitMemberAccess(MemberExpression m)
        {

            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                _sb.Append(m.Member.Name);
                return m;
            }

            throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));

        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (c.Type == typeof(string))
                _sb.Append('\'').Append(c.Value).Append('\'');
            else
                _sb.Append(c.Value);
            return c;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            _sb.Append("");

            Visit(b.Left);

            switch (b.NodeType)
            {
                case ExpressionType.And:

                    _sb.Append(" and ");

                    break;

                case ExpressionType.Or:

                    _sb.Append(" or ");

                    break;

                case ExpressionType.Equal:

                    _sb.Append(" eq ");

                    break;

                case ExpressionType.NotEqual:

                    _sb.Append(" neq ");

                    break;

                case ExpressionType.LessThan:

                    _sb.Append(" lt ");

                    break;

                case ExpressionType.LessThanOrEqual:

                    _sb.Append(" lte ");

                    break;

                case ExpressionType.GreaterThan:

                    _sb.Append(" gt ");

                    break;

                case ExpressionType.GreaterThanOrEqual:

                    _sb.Append(" gte ");

                    break;

                default:

                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported",
                                                                  b.NodeType));
            }

            Visit(b.Right);

            return b;
        }
    }
}
#region Full license
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion
