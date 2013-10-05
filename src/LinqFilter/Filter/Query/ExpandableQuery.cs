using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqFilter.Filter.Query
{
    public class ExpandableQuery<T> : IOrderedQueryable<T>
    {
        private readonly ExpandableQueryProvider<T> _provider;
        private readonly IQueryable<T> _inner;

        internal IQueryable<T> InnerQuery
        {
            get { return _inner; }
        }

        // Original query, that we're wrapping

        internal ExpandableQuery(IQueryable<T> inner)
        {
            _inner = inner;
            _provider = new ExpandableQueryProvider<T>(this);
        }

        Expression IQueryable.Expression
        {
            get { return _inner.Expression; }
        }

        Type IQueryable.ElementType
        {
            get { return typeof (T); }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return _provider; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        public override string ToString()
        {
            return _inner.ToString();
        }
    }
}
