using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LinqFilter.Filter.Entities;
using LinqFilter.Filter.UtilsExtensions;

namespace LinqFilter.Filter.Impl
{
    public class GenericFilterCommand<T>
    {
        public LinqFilterResult<T> Process(LinqFilterRequest<T> request)
        {
            var filteringPredicate = PredicateBuilder.True<T>();
            foreach (var filter in request.Filters)
            {
                var propertyName = filter.EntityPropertyAccessPath;
                var propertyTestValues = filter.TestValues.ToList();
                if (propertyTestValues.Count() > 1)
                {
                    var filteringSubPredicate = PredicateBuilder.False<T>();
                    filteringSubPredicate = propertyTestValues.Aggregate(filteringSubPredicate, (current, propertyTestValue) => current.Or(CreateConditionalInvocation(propertyName, propertyTestValue)));
                    filteringPredicate = filteringPredicate.And(filteringSubPredicate.Expand());
                }
                else
                {
                    filteringPredicate = filteringPredicate.And(CreateConditionalInvocation(propertyName, propertyTestValues.First()));
                }
            }


            var collectionResult = request.Collection.AsExpandable().Where(filteringPredicate);
            
            return new LinqFilterResult<T>
                       {
                           FilterResults = collectionResult
                       };
        }

        private Expression<Func<T, bool>> CreateConditionalInvocation(string propertyName, string value)
        {
            var parameter = Expression.Parameter(typeof(T), "f");
            var propertyAccess = parameter.NestedExpressionProperty(propertyName);
            var declaringType = ((PropertyInfo)propertyAccess.Member).PropertyType;

            var operation = OperationKind.Unknown;
            var conversion = ExpressionUtil.ComposeCastExpression(declaringType, value, ref operation);
            if (conversion == null || operation == OperationKind.Unknown)
            {
                throw new NullReferenceException("Could not map convert expression from type string to type {0}.Maybe not implemented yet");
            }

            switch (operation)
            {
                case OperationKind.Equals:
                    {
                        var equals = Expression.Equal(propertyAccess, conversion);
                        return Expression.Lambda<Func<T, bool>>(equals, parameter);
                    }
                case OperationKind.Contains:
                    {
                        var like = Expression.Call(propertyAccess, "Contains", null, Expression.Constant(value, typeof(string)));
                        return Expression.Lambda<Func<T, bool>>(like, parameter);
                    }
                default:
                    {
                        throw new Exception(string.Format("Invalid operation type {0}", operation.ToString()));
                    }
            }
        }
    }
}