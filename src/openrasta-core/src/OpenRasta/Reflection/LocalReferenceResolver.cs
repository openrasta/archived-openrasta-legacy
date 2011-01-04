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
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenRasta.Reflection
{
    public static class Evaluator
    {
        /// <summary>
        /// Performs evaluation and replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <param name="fnCanBeEvaluated">A function that decides whether a given expression node can be part of the local function.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression PartialEval(Expression expression, Func<Expression, bool> fnCanBeEvaluated)
        {
            return new SubtreeEvaluator(new SubtreeNominator(fnCanBeEvaluated).Nominate(expression)).Eval(expression);
        }

        /// <summary>
        /// Performs evaluation and replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression PartialEval(Expression expression)
        {
            return PartialEval(expression, CanBeEvaluatedLocally);
        }

        static bool CanBeEvaluatedLocally(Expression expression)
        {
            return expression.NodeType != ExpressionType.Parameter;
        }
    }

    /// <summary>
    /// Evaluates and replaces sub-trees when first candidate is reached (top-down)
    /// </summary>
    public class SubtreeEvaluator : ExpressionVisitor
    {
        readonly HashSet<Expression> candidates;

        internal SubtreeEvaluator(HashSet<Expression> candidates)
        {
            this.candidates = candidates;
        }

        internal Expression Eval(Expression exp)
        {
            return this.Visit(exp);
        }

        protected override Expression Visit(Expression exp)
        {
            if (exp == null) return null;

            if (this.candidates.Contains(exp)) return this.Evaluate(exp);

            return base.Visit(exp);
        }

        Expression Evaluate(Expression e)
        {
            if (e.NodeType == ExpressionType.Constant) return e;

            LambdaExpression lambda = Expression.Lambda(e);

            Delegate fn = lambda.Compile();

            return Expression.Constant(fn.DynamicInvoke(null), e.Type);
        }
    }

    /// <summary>
    /// Performs bottom-up analysis to determine which nodes can possibly
    /// be part of an evaluated sub-tree.
    /// </summary>
    public class SubtreeNominator : ExpressionVisitor
    {
        readonly Func<Expression, bool> fnCanBeEvaluated;

        HashSet<Expression> candidates;
        bool cannotBeEvaluated;

        internal SubtreeNominator(Func<Expression, bool> fnCanBeEvaluated)
        {
            this.fnCanBeEvaluated = fnCanBeEvaluated;
        }

        internal HashSet<Expression> Nominate(Expression expression)
        {
            this.candidates = new HashSet<Expression>();
            this.Visit(expression);
            return this.candidates;
        }

        protected override Expression Visit(Expression expression)
        {
            if (expression != null)
            {
                bool saveCannotBeEvaluated = this.cannotBeEvaluated;

                this.cannotBeEvaluated = false;

                base.Visit(expression);

                if (!this.cannotBeEvaluated)
                    if (this.fnCanBeEvaluated(expression)) candidates.Add(expression);

                    else cannotBeEvaluated = true;

                cannotBeEvaluated |= saveCannotBeEvaluated;
            }

            return expression;
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
