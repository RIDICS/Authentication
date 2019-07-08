using System.Collections.Generic;
using System.Linq;
using DryIoc.Facilities.NHibernate;
using NHibernate;
using NHibernate.Criterion;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Core.DataEntities.Shared.Query;
using Ridics.Core.DataEntities.Shared.Repositories;
using Ridics.Core.Shared.Exceptions;

namespace Ridics.Authentication.DataEntities.Repositories
{
    public class ApiResourceRepository : RepositoryBase
    {
        private readonly List<QueryOrderBy<ApiResourceEntity>> m_defaultOrdering;

        public ApiResourceRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            m_defaultOrdering = new List<QueryOrderBy<ApiResourceEntity>>
            {
                new QueryOrderBy<ApiResourceEntity> {Expression = x => x.Name}
            };
        }

        private void FetchCollections(ISession session, ICriterion where = null,
            IList<QueryJoinAlias<ApiResourceEntity>> joinAliases = null)
        {
            CreateBaseQuery(session, where, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.ApiSecrets)
                .Future<ApiResourceEntity>();

            CreateBaseQuery(session, where, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.Scopes)
                .Fetch(SelectMode.Fetch, x => x.Scopes.First().ClaimTypes)
                .Fetch(SelectMode.Fetch, x => x.Scopes.First().ClaimTypes.First().Type)
                .Future<ApiResourceEntity>();

            CreateBaseQuery(session, where, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.ClaimTypes)
                .Fetch(SelectMode.Fetch, x => x.ClaimTypes.First().Type)
                .Future<ApiResourceEntity>();
        }

        private AbstractCriterion CreateSearchCriteria(string searchByName)
        {
            var criteria = string.IsNullOrEmpty(searchByName)
                ? null
                : Restrictions.On<ApiResourceEntity>(x => x.Name).IsInsensitiveLike(searchByName, MatchMode.Anywhere);

            return criteria;
        }

        public IList<ApiResourceEntity> GetResources(int start, int count, string searchByName = null)
        {
            var criteria = CreateSearchCriteria(searchByName);

            try
            {
                return GetValuesList<ApiResourceEntity>(start, count, FetchCollections, criteria, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get api resource list operation failed", ex);
            }
        }

        public int GetResourcesCount(string searchByName = null)
        {
            var criteria = CreateSearchCriteria(searchByName);

            try
            {
                return GetValuesCount<ApiResourceEntity>(criteria);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get api resource count operation failed", ex);
            }
        }


        public ApiResourceEntity FindByName(string name)
        {
            var criterion = Restrictions.Where<ApiResourceEntity>(x => x.Name == name);

            try
            {
                return GetSingleValue<ApiResourceEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get api resource by name operation failed", ex);
            }
        }

        public IList<ApiResourceEntity> GetAllResources()
        {
            try
            {
                return GetValuesList<ApiResourceEntity>(FetchCollections, null, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get all api resource list operation failed", ex);
            }
        }

        public IList<ApiResourceEntity> GetResourcesById(IEnumerable<int> ids)
        {
            var criterion = Restrictions.On<ApiResourceEntity>(x => x.Id).IsIn(ids.ToList());

            try
            {
                return GetValuesList<ApiResourceEntity>(FetchCollections, criterion, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get api resource list by ids operation failed", ex);
            }
        }

        public ApiResourceEntity FindApiResourceById(int id)
        {
            var criterion = Restrictions.Where<ApiResourceEntity>(x => x.Id == id);

            try
            {
                return GetSingleValue<ApiResourceEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get api resource by name operation failed", ex);
            }
        }
    }
}