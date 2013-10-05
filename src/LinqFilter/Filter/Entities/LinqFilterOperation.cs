using System.Collections.Generic;

namespace LinqFilter.Filter.Entities
{
    public class LinqFilterOperation
    {
        public string EntityPropertyAccessPath { get; set; }

        public OperationKind OperationKind { get; set; }

        public IEnumerable<string> TestValues { get; set; } 
    }
}
