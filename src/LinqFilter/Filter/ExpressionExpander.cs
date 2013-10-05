using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using LinqFilter.Filter.UtilsExtensions;

namespace LinqFilter.Filter
{
    /// <summary>
    /// Custom expresssion visitor for ExpandableQuery. This expands calls to Expression.Compile() and
    /// collapses captured lambda references in subqueries which LINQ to SQL can't otherwise handle.
    /// </summary>
    internal class ExpressionExpander : ExpressionVisitor
    {
        // Replacement parameters - for when invoking a lambda expression.
        private readonly Dictionary<ParameterExpression, Expression> _replaceVars;

        internal ExpressionExpander()
        {
        }

        private ExpressionExpander(Dictionary<ParameterExpression, Expression> replaceVars)
        {
            _replaceVars = replaceVars;
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            if ((_replaceVars != null) && (_replaceVars.ContainsKey(p)))
                return _replaceVars[p];
            return base.VisitParameter(p);
        }

        /// <summary>
        /// Flatten calls to Invoke so that Entity Framework can understand it. Calls to Invoke are generated
        /// by PredicateBuilder.
        /// </summary>
        protected override Expression VisitInvocation(InvocationExpression iv)
        {
            var target = iv.Expression;
            if (target is MemberExpression) target = TransformExpr((MemberExpression) target);
            if (target is ConstantExpression) target = ((ConstantExpression) target).Value as Expression;

            var lambda = (LambdaExpression) target;

            var replaceVars = _replaceVars == null
                                  ? new Dictionary<ParameterExpression, Expression>()
                                  : new Dictionary<ParameterExpression, Expression>(_replaceVars);
            try
            {
                // ReSharper disable PossibleNullReferenceException
                for (int i = 0; i < lambda.Parameters.Count; i++)
                    // ReSharper restore PossibleNullReferenceException
                    replaceVars.Add(lambda.Parameters[i], iv.Arguments[i]);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException(
                    "Invoke cannot be called recursively - try using a temporary variable.", ex);
            }

            return new ExpressionExpander(replaceVars).Visit(lambda.Body);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.Name == "Invoke" && m.Method.DeclaringType == typeof(ExpressionExtensions))
            {
                Expression target = m.Arguments[0];
                if (target is MemberExpression) target = TransformExpr((MemberExpression) target);
                if (target is ConstantExpression) target = ((ConstantExpression) target).Value as Expression;

                var lambda = (LambdaExpression) target;

                var replaceVars = _replaceVars == null
                                      ? new Dictionary<ParameterExpression, Expression>()
                                      : new Dictionary<ParameterExpression, Expression>(_replaceVars);

                try
                {
                    // ReSharper disable PossibleNullReferenceException
                    for (var i = 0; i < lambda.Parameters.Count; i++)
                        // ReSharper restore PossibleNullReferenceException
                        replaceVars.Add(lambda.Parameters[i], m.Arguments[i + 1]);
                }
                catch (ArgumentException ex)
                {
                    throw new InvalidOperationException(
                        "Invoke cannot be called recursively - try using a temporary variable.", ex);
                }

                return new ExpressionExpander(replaceVars).Visit(lambda.Body);
            }

            // Expand calls to an expression's Compile() method:
            if (m.Method.Name == "Compile" && m.Object is MemberExpression)
            {
                var me = (MemberExpression) m.Object;
                var newExpr = TransformExpr(me);
                if (newExpr != me) return newExpr;
            }

            // Strip out any nested calls to AsExpandable():
            if (m.Method.Name == "AsExpandable" && m.Method.DeclaringType == typeof (CollectionExtensions))
                return m.Arguments[0];

            return base.VisitMethodCall(m);
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            // Strip out any references to expressions captured by outer variables - LINQ to SQL can't handle these:
            // ReSharper disable PossibleNullReferenceException
            if (m.Member.DeclaringType.Name.StartsWith("<>"))
                // ReSharper restore PossibleNullReferenceException
                return TransformExpr(m);

            return base.VisitMemberAccess(m);
        }

        private Expression TransformExpr(MemberExpression input)
        {
            // Collapse captured outer variables
            if (input == null
                || !(input.Member is FieldInfo)
                || !input.Member.ReflectedType.IsNestedPrivate
                || !input.Member.ReflectedType.Name.StartsWith("<>")) // captured outer variable
                return input;

            var expression = input.Expression as ConstantExpression;
            if (expression != null)
            {
                object obj = expression.Value;
                if (obj == null) return input;
                var t = obj.GetType();
                if (!t.IsNestedPrivate || !t.Name.StartsWith("<>")) return input;
                var fi = (FieldInfo) input.Member;
                var result = fi.GetValue(obj);
                var exp = result as Expression;
                if (exp != null)
                    return Visit(exp);
            }
            return input;
        }
    }
}
