using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using OpenRasta.Reflection;
using OpenRasta.Web.Markup;

namespace OpenRasta.Codecs.SharpView.Visitors
{
    public class ForEachRewriter
    {
        public ForEachRewriter(int scopeLevel, Expression iterator)
        {
            Iterator = iterator;
            ScopeLevel = scopeLevel;
        }

        public Expression Iterator { get; private set; }
        public ParameterExpression LambdaParameter { get; private set; }
        int ScopeLevel { get; set; }

        public Expression RewriteSubExpression(Expression htmlBuilder)
        {
            var expressionAsFunc = htmlBuilder as Expression<Func<IElement>>;
            if (expressionAsFunc != null)
                htmlBuilder = expressionAsFunc.Body;

            Type iteratorItemType = ExtractEnumeratorItemType(Iterator.Type);
            if (iteratorItemType == null)
                throw new InvalidOperationException(
                    "The type {0} doesn't implement IEnumerable<>".With(Iterator.Type.Name));

            if (Iterator.Type.IsArray)
                Iterator = Expression.Convert(Iterator, typeof(IEnumerable<>).MakeGenericType(iteratorItemType));

            // Creates Func<TSource,TResult>
            Type projectionType = typeof(Func<,>).MakeGenericType(iteratorItemType, htmlBuilder.Type);

            // Creates item=>{element building expression}
            string itemParameterName = "__OR_foreachItem" + ScopeLevel;
            LambdaParameter = Expression.Parameter(iteratorItemType, itemParameterName);
            LambdaExpression projection = Expression.Lambda(projectionType, htmlBuilder, LambdaParameter);

            // rewrite a.b.c to
            // a != null ? a.b != null ? a.b.c : null : null : null ?? new List<T>
            Expression nullPropagatingIterator = Iterator;

            // Create SelectHtml<TSource,TResult>(IEnumerable<TSource>,Func<TSource,TResult>>);
            Type resultType = htmlBuilder.Type;
            MethodInfo selectMethodInfo = GetSelectMethod(iteratorItemType, resultType);
            return Expression.Call(null,
                                   selectMethodInfo,
                                   nullPropagatingIterator,
                                   Expression.Constant(null, typeof(object)),
                                   Expression.Constant(null, typeof(PropertyPath)),
                                   projection);
        }

        static Type ExtractEnumeratorItemType(Type type)
        {
            if (type.IsArray)
                return type.GetElementType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return type.GetGenericArguments()[0];

            // now more diffcult, need to find the interface implemetation on a list type.
            Type enumerableInterface = type.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    .First();
            if (enumerableInterface != null)
                return enumerableInterface.GetGenericArguments()[0];
            return null;
        }

        static MethodInfo GetSelectMethod(Type source, Type result)
        {
            return typeof(SourcedElementExtensions).GetMethods().Where(
                mi => mi.Name == "SelectHtml" && mi.GetGenericArguments().Length == 2).First().MakeGenericMethod(
                source, result);
        }
    }
}