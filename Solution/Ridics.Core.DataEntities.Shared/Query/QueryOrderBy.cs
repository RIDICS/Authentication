using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Ridics.Core.DataEntities.Shared.Query
{
    public class QueryOrderBy<T>
    {
        public Expression<Func<T, object>> Expression { get; set; }

        public ListSortDirection SortDirection { get; set; }
    }
}