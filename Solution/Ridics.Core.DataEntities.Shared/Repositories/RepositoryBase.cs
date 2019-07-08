using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DryIoc.Facilities.NHibernate;
using NHibernate;
using NHibernate.Criterion;
using Ridics.Core.DataEntities.Shared.Daos;
using Ridics.Core.DataEntities.Shared.Query;

namespace Ridics.Core.DataEntities.Shared.Repositories
{
    public abstract class RepositoryBase : NHibernateDao
    {
        protected RepositoryBase(ISessionManager sessionManager) : base(sessionManager)
        {
        }

        protected IQueryOver<T, T> CreateBaseQuery<T>(
            ISession session,
            ICriterion criterion = null,
            IList<QueryJoinAlias<T>> joinAliases = null
        ) where T : class
        {
            var query = session.QueryOver<T>();

            if (joinAliases != null)
            {
                foreach (var joinAlias in joinAliases)
                {
                    query.JoinAlias(joinAlias.Function, joinAlias.Alias, joinAlias.JoinType);
                }
            }

            if (criterion != null)
            {
                query.Where(criterion);
            }

            return query;
        }

        protected T GetSingleValue<T>(
            Action<ISession, ICriterion, IList<QueryJoinAlias<T>>> fetchMethod = null,
            ICriterion criterion = null,
            IList<QueryJoinAlias<T>> joinAliases = null
        ) where T : class
        {
            var session = GetSession();

            var result = CreateBaseQuery(session, criterion, joinAliases).FutureValue<T>();

            fetchMethod?.Invoke(session, criterion, joinAliases);

            return result.Value;
        }


        protected IList<T> GetValuesList<T>(
            Action<ISession, ICriterion, IList<QueryJoinAlias<T>>> fetchMethod = null,
            ICriterion criterion = null,
            IList<QueryJoinAlias<T>> joinAliases = null,
            IList<QueryOrderBy<T>> orderByList = null
        ) where T : class
        {
            var session = GetSession();

            var query = CreateBaseQuery(session, criterion, joinAliases);

            if (orderByList != null)
            {
                foreach (var orderBy in orderByList)
                {
                    query = orderBy.SortDirection == ListSortDirection.Descending
                        ? query.OrderBy(orderBy.Expression).Desc
                        : query.OrderBy(orderBy.Expression).Asc;
                }
            }

            var result = query.Future<T>();

            fetchMethod?.Invoke(session, criterion, joinAliases);

            return result.ToList();
        }

        protected IList<T> GetValuesList<T>(
            int start,
            int count,
            Action<ISession, ICriterion, IList<QueryJoinAlias<T>>> fetchMethod = null,
            ICriterion criterion = null,
            IList<QueryJoinAlias<T>> joinAliases = null,
            IList<QueryOrderBy<T>> orderByList = null
        ) where T : class
        {
            var session = GetSession();

            var query = CreateBaseQuery(session, criterion, joinAliases);

            if (orderByList != null)
            {
                foreach (var orderBy in orderByList)
                {
                    query = orderBy.SortDirection == ListSortDirection.Descending
                        ? query.OrderBy(orderBy.Expression).Desc
                        : query.OrderBy(orderBy.Expression).Asc;
                }
            }

            var result = query.Skip(start)
                .Take(count)
                .Future<T>();

            fetchMethod?.Invoke(session, criterion, joinAliases);

            return result.ToList();
        }

        protected int GetValuesCount<T>(
            ICriterion criterion = null,
            IList<QueryJoinAlias<T>> joinAliases = null
        ) where T : class
        {
            var session = GetSession();

            var query = CreateBaseQuery(session, criterion, joinAliases);

            var result = query.RowCount();

            return result;
        }
    }
}