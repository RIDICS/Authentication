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
    public class ScopeRepository : RepositoryBase
    {
        private readonly List<QueryOrderBy<ScopeEntity>> m_defaultOrdering;

        public ScopeRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            m_defaultOrdering = new List<QueryOrderBy<ScopeEntity>>
            {
                new QueryOrderBy<ScopeEntity> {Expression = x => x.Name}
            };
        }

        private void FetchCollections(ISession session, ICriterion criterion = null,
            IList<QueryJoinAlias<ScopeEntity>> joinAliases = null)
        {
            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.ClaimTypes)
                .Fetch(SelectMode.Fetch, x => x.ClaimTypes.First().Type)
                .Future<ScopeEntity>();
        }

        public IList<ScopeEntity> GetScopesForApiResource(int apiResourceId)
        {
            var criterion = Restrictions.Where<ScopeEntity>(x => x.ApiResource.Id == apiResourceId);

            try
            {
                return GetValuesList(FetchCollections, criterion, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get scopes for api resource operation failed", ex);
            }
        }

        public IList<ScopeEntity> GetScopesById(IEnumerable<int> ids)
        {
            var criterion = Restrictions.On<ScopeEntity>(x => x.Id).IsIn(ids.ToList());

            try
            {
                return GetValuesList<ScopeEntity>(FetchCollections, criterion,  null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get scopes list by ids operation failed", ex);
            }
        }

        public IList<ScopeEntity> GetAllScopes()
        {
            try
            {
                return GetValuesList<ScopeEntity>(FetchCollections, null, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get all scopes list operation failed", ex);
            }
        }
    }
}