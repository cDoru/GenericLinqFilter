using System;
using System.Linq.Expressions;
using LinqFilter.Filter.Entities;

namespace LinqFilter.Filter
{
    public class ExpressionUtil
    {
        public static UnaryExpression ComposeCastExpression(Type declaringType, string value, ref OperationKind operation)
        {
            UnaryExpression conversionResult = null;

            if (declaringType == typeof(Int32))
            {
                operation = OperationKind.Equals;
                var constant = Expression.Constant(Int32.Parse(value));
                conversionResult = Expression.Convert(constant, typeof(Int32));
            }
            else if (declaringType == typeof(Int64))
            {
                operation = OperationKind.Equals;
                var constant = Expression.Constant(Int64.Parse(value));
                conversionResult = Expression.Convert(constant, typeof(Int64));
            }
            else if (declaringType == typeof(byte))
            {
                operation = OperationKind.Equals;
                var constant = Expression.Constant(byte.Parse(value));
                conversionResult = Expression.Convert(constant, typeof(byte));
            }
            else if (declaringType == typeof(double))
            {
                operation = OperationKind.Equals;
                var constant = Expression.Constant(double.Parse(value));
                conversionResult = Expression.Convert(constant, typeof(double));
            }
            else if (declaringType == typeof(string))
            {
                operation = OperationKind.Contains;
                var constant = Expression.Constant(value);
                conversionResult = Expression.Convert(constant, typeof(string));
            }
            else if (declaringType == typeof(long))
            {
                operation = OperationKind.Equals;
                var constant = Expression.Constant(long.Parse(value));
                conversionResult = Expression.Convert(constant, typeof(long));
            }
            else if (declaringType == typeof(sbyte))
            {
                operation = OperationKind.Equals;
                var constant = Expression.Constant(sbyte.Parse(value));
                conversionResult = Expression.Convert(constant, typeof(sbyte));
            }
            else if (declaringType == typeof(Guid))
            {
                operation = OperationKind.Equals;
                var constant = Expression.Constant(Guid.Parse(value));
                conversionResult = Expression.Convert(constant, typeof(Guid));
            }
            else if (declaringType == typeof(Enum))
            {
                operation = OperationKind.Equals;
                var constant = Expression.Constant(Enum.Parse(declaringType, value));
                conversionResult = Expression.Convert(constant, typeof(Enum));
            }
            else if (declaringType == typeof(object))
            {
                operation = OperationKind.Equals;
                var constant = Expression.Constant(value);
                conversionResult = Expression.Convert(constant, typeof(object));
            }

            return conversionResult;
        }
    }
}
