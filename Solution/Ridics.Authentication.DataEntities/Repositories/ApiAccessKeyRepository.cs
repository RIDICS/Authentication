using System.Collections.Generic;
using DryIoc.Facilities.NHibernate;
using NHibernate;
using NHibernate.Criterion;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Core.DataEntities.Shared.Query;
using Ridics.Core.DataEntities.Shared.Repositories;
using Ridics.Core.Shared.Exceptions;

namespace Ridics.Authentication.DataEntities.Repositories
{
    public class ApiAccessKeyRepository : RepositoryBase
    {
        private readonly List<QueryOrderBy<ApiAccessKeyEntity>> m_defaultOrdering;

        public ApiAccessKeyRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            m_defaultOrdering = new List<QueryOrderBy<ApiAccessKeyEntity>>
            {
                new QueryOrderBy<ApiAccessKeyEntity> {Expression = x => x.Name}
            };
        }

        private void FetchCollections(ISession session, ICriterion criterion = null,
            IList<QueryJoinAlias<ApiAccessKeyEntity>> joinAliases = null)
        {
            CreateBaseQuery<ApiAccessKeyEntity>(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.Permissions)
                .Future<ApiAccessKeyEntity>();
        }

        private AbstractCriterion CreateSearchCriteria(string searchByName)
        {
            var criteria = string.IsNullOrEmpty(searchByName)
                ? null
                : Restrictions.On<ApiAccessKeyEntity>(x => x.Name).IsInsensitiveLike(searchByName, MatchMode.Anywhere);

            return criteria;
        }

        public IList<ApiAccessKeyEntity> GetAllAccessKeys()
        {
            try
            {
                return GetValuesList(FetchCollections, null, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get api access key entity operation failed", ex);
            }
        }

        public IList<ApiAccessKeyEntity> GetApiAccessKeys(int start, int count, string searchByName = null)
        {
            var criteria = CreateSearchCriteria(searchByName);

            try
            {
                return GetValuesList(start, count, FetchCollections, criteria, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get api access key list operation failed", ex);
            }
        }

        public int GetApiAccessKeysCount(string searchByName = null)
        {
            var criteria = CreateSearchCriteria(searchByName);

            try
            {
                return GetValuesCount<ApiAccessKeyEntity>(criteria);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get api access key count operation failed", ex);
            }
        }

        public ApiAccessKeyEntity FindApiAccessKeyById(int id)
        {
            var criterion = Restrictions.Where<ApiAccessKeyEntity>(x => x.Id == id);

            try
            {
                return GetSingleValue<ApiAccessKeyEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get api access key by id operation failed", ex);
            }
        }
    }
}