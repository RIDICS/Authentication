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
    public class IdentityResourceRepository : RepositoryBase
    {
        private readonly List<QueryOrderBy<IdentityResourceEntity>> m_defaultOrdering;

        public IdentityResourceRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            m_defaultOrdering = new List<QueryOrderBy<IdentityResourceEntity>>
            {
                new QueryOrderBy<IdentityResourceEntity> {Expression = x => x.Name}
            };
        }

        private void FetchCollections(ISession session, ICriterion criterion = null,
            IList<QueryJoinAlias<IdentityResourceEntity>> joinAliases = null)
        {
            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.ClaimTypes)
                .Fetch(SelectMode.Fetch, x => x.ClaimTypes.First().Type)
                .Future<IdentityResourceEntity>();

            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.Clients)
                .Future<IdentityResourceEntity>();
        }

        private AbstractCriterion CreateSearchCriteria(string searchByName)
        {
            var criteria = string.IsNullOrEmpty(searchByName)
                ? null
                : Restrictions.On<IdentityResourceEntity>(x => x.Name).IsInsensitiveLike(searchByName, MatchMode.Anywhere);

            return criteria;
        }

        public IList<IdentityResourceEntity> GetResources(int start, int count, string searchByName = null)
        {
            var criteria = CreateSearchCriteria(searchByName);

            try
            {
                return GetValuesList(start, count, FetchCollections, criteria, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get identity resource list operation failed", ex);
            }
        }

        public int GetResourcesCount(string searchByName = null)
        {
            var criteria = CreateSearchCriteria(searchByName);

            try
            {
                return GetValuesCount<IdentityResourceEntity>(criteria);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get identity resource count operation failed", ex);
            }
        }

        public IList<IdentityResourceEntity> GetAllResources()
        {
            try
            {
                return GetValuesList(FetchCollections, null, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get all identity resource list operation failed", ex);
            }
        }

        public IList<IdentityResourceEntity> GetResourcesById(IEnumerable<int> ids)
        {
            var criterion = Restrictions.On<IdentityResourceEntity>(x => x.Id).IsIn(ids.ToList());

            try
            {
                return GetValuesList(FetchCollections, criterion, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get identity resource list by ids operation failed", ex);
            }
        }

        public IdentityResourceEntity FindIdentityResourceById(int id)
        {
            var criterion = Restrictions.Where<IdentityResourceEntity>(x => x.Id == id);

            try
            {
                return GetSingleValue<IdentityResourceEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get identity resource operation failed", ex);
            }
        }
    }
}