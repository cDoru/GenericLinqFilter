using System;
using System.Collections.Generic;
using System.Linq;
using LinqFilter.Filter.Query;

namespace LinqFilter.Filter.UtilsExtensions
{
    public static class CollectionExtensions
    {
        public static IQueryable<T> AsExpandable<T>(this IQueryable<T> query)
        {
            if (query is ExpandableQuery<T>) return query;
            return new ExpandableQuery<T>(query);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
                action(element);
        }
    }
}
