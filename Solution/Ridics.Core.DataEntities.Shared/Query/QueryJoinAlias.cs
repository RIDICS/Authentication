using System;
using System.Linq.Expressions;
using NHibernate.SqlCommand;

namespace Ridics.Core.DataEntities.Shared.Query
{
    public class QueryJoinAlias<T>
    {
        public Expression<Func<T, object>> Function { get; set; }

        public Expression<Func<object>> Alias { get; set; }

        public JoinType JoinType { get; set; }
    }
}