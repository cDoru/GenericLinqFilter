using System.Collections.Generic;
using System.Linq;

namespace LinqFilter.Filter.Entities
{
    public class LinqFilterRequest<T>
    {
        public IQueryable<T> Collection { get; set; }

        public List<LinqFilterOperation> Filters { get; set; } 
    }
}