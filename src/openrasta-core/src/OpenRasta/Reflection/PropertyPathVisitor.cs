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
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using OpenRasta.TypeSystem.ReflectionBased;

namespace OpenRasta.Reflection
{
    public class PropertyPathVisitor : ExpressionVisitor
    {
        public PropertyPathVisitor()
        {
            PropertyPathBuilder = new StringBuilder();
        }

        protected StringBuilder PropertyPathBuilder { get; set; }
        protected string RootType { get; set; }

        public Type PropertyType { get; private set; }

        public PropertyPath BuildPropertyPath(Expression expression)
        {
            Visit(expression);
            if (RootType != null || PropertyPathBuilder.Length != 0)
            {
                return new PropertyPath
                {
                    TypePrefix = RootType,
                    TypeSuffix = PropertyPathBuilder.ToString()
                };
            }
            return null;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            Visit(m.Expression);
            PropertyType = m.Type;
            // try to get the value
            try
            {
                var rootInstance = Expression.Lambda(typeof (Func<object>), m.Expression).Compile().DynamicInvoke();
                if (rootInstance != null)
                {
                    {
                        var pi = m.Member as PropertyInfo;
                        object resultingInstance = null;
                        if (pi != null)
                             resultingInstance = pi.GetValue(rootInstance, null);
                        var fi = m.Member as FieldInfo;
                        if (fi != null)
                            resultingInstance = fi.GetValue(rootInstance);
                        if (resultingInstance != null)
                        {
                            var propertyPath = ObjectPaths.Get(resultingInstance);
                            if (propertyPath != null)
                            {
                                RootType = propertyPath.TypePrefix;
                                AppendPropertyPath(propertyPath.TypeSuffix);
                                return m;
                            }
                        }
                    }
                        
                }
            }
            catch
            {
            }
            if (RootType == null)
            {
                RootType = ExtractType(m.Member).GetTypeString();
            }
            else
            {
                var name = m.Member.Name;
                AppendPropertyPath(name);
            }
            return m;
        }

        void AppendPropertyPath(string name)
        {
            if (PropertyPathBuilder.Length > 0)
                PropertyPathBuilder.Append(".");
            PropertyPathBuilder.Append(name);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            if (RootType == null)
            {
                RootType = p.Type.GetTypeString();
            }
            return base.VisitParameter(p);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            base.VisitMethodCall(m);
            if (m.Method.Name == "get_Item" && m.Arguments.Count == 1 && m.Arguments[0].NodeType == ExpressionType.Constant)
            {
                object argumentValue = ((ConstantExpression) m.Arguments[0]).Value;
                PropertyPathBuilder.Append(":").Append(argumentValue.ConvertToString());
            }
            return m;
        }

        Type ExtractType(MemberInfo info)
        {
            var pi = info as PropertyInfo;
            if (pi != null)
                return pi.PropertyType;
            var fi = info as FieldInfo;
            if (fi != null)
                return fi.FieldType;
            return null;
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