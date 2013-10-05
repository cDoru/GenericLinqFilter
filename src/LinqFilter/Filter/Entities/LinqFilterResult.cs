using System.Linq;

namespace LinqFilter.Filter.Entities
{
    public class LinqFilterResult<T>
    {
        public IQueryable<T> FilterResults { get; set; }
    }
}