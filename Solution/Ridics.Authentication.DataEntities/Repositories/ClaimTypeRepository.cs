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
    public class ClaimTypeRepository : RepositoryBase
    {
        private readonly List<QueryOrderBy<ClaimTypeEntity>> m_defaultOrdering;

        public ClaimTypeRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            m_defaultOrdering = new List<QueryOrderBy<ClaimTypeEntity>>
            {
                new QueryOrderBy<ClaimTypeEntity> {Expression = x => x.Name}
            };
        }

        private void FetchCollections(ISession session, ICriterion criterion = null,
            IList<QueryJoinAlias<ClaimTypeEntity>> joinAliases = null)
        {
            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.Scopes)
                .Fetch(SelectMode.Fetch, x => x.Type)
                .Future<ClaimTypeEntity>();

            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.Claims)
                .Future<ClaimTypeEntity>();

            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.Resources)
                .Future<ClaimTypeEntity>();
        }

        private AbstractCriterion CreateSearchCriteria(string searchByName)
        {
            var criteria = string.IsNullOrEmpty(searchByName)
                ? null
                : Restrictions.On<ClaimTypeEntity>(x => x.Name).IsInsensitiveLike(searchByName, MatchMode.Anywhere);

            return criteria;
        }

        public IList<ClaimTypeEntity> GetClaimTypes(int start, int count, string searchByName = null)
        {
            var criteria = CreateSearchCriteria(searchByName);

            try
            {
                return GetValuesList(start, count, FetchCollections, criteria, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get claim type list operation failed", ex);
            }
        }

        public int GetClaimTypesCount(string searchByName = null)
        {
            var criteria = CreateSearchCriteria(searchByName);

            try
            {
                return GetValuesCount<ClaimTypeEntity>(criteria);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get claim type count operation failed", ex);
            }
        }

        public IList<ClaimTypeEntity> GetAllClaimTypes()
        {
            try
            {
                return GetValuesList(FetchCollections, null, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get claim type list operation failed", ex);
            }
        }

        public IList<ClaimTypeEntity> GetClaimTypesById(IEnumerable<int> ids)
        {
            var criterion = Restrictions.On<ClaimTypeEntity>(x => x.Id).IsIn(ids.ToList());

            try
            {
                return GetValuesList(FetchCollections, criterion, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get claim type list by ids operation failed", ex);
            }
        }

        public ClaimTypeEntity FindClaimTypeById(int id)
        {
            var criterion = Restrictions.Where<ClaimTypeEntity>(x => x.Id == id);

            try
            {
                return GetSingleValue<ClaimTypeEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get claim type list by ids operation failed", ex);
            }
        }
    }
}