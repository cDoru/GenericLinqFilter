using System;
using System.Linq;
using System.Linq.Expressions;

namespace LinqFilter.Filter.UtilsExtensions
{
    public static class ExpressionExtensions
    {
        public static Expression<TDelegate> Expand<TDelegate>(this Expression<TDelegate> expr)
        {
            return (Expression<TDelegate>)new ExpressionExpander().Visit(expr);
        }

        public static Expression Expand(this Expression expr)
        {
            return new ExpressionExpander().Visit(expr);
        }

        public static TResult Invoke<TResult>(this Expression<Func<TResult>> expr)
        {
            return expr.Compile().Invoke();
        }

        public static TResult Invoke<T1, TResult>(this Expression<Func<T1, TResult>> expr, T1 arg1)
        {
            return expr.Compile().Invoke(arg1);
        }

        public static TResult Invoke<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> expr, T1 arg1, T2 arg2)
        {
            return expr.Compile().Invoke(arg1, arg2);
        }

        public static TResult Invoke<T1, T2, T3, TResult>(
            this Expression<Func<T1, T2, T3, TResult>> expr, T1 arg1, T2 arg2, T3 arg3)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3);
        }

        public static TResult Invoke<T1, T2, T3, T4, TResult>(
            this Expression<Func<T1, T2, T3, T4, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4);
        }

        public static MemberExpression NestedExpressionProperty(this Expression expression, string propertyPath)
        {
            string result = null;
            var first = true;
            
            var parts = propertyPath.Split('.');
            var partsL = parts.Length;

            // move (recurrently) top down through the propertypath to get the nested member access expression
            foreach (var s in parts.Take(partsL - 1))
            {
                if (first)
                {
                    first = false;
                    result = s;
                    continue;
                }

                result = result + "." + s;
            }
            
            return (partsL > 1)
                    ? Expression.Property(NestedExpressionProperty(expression, result),parts[partsL - 1])
                    : Expression.Property(expression, propertyPath);
        }
    }
}