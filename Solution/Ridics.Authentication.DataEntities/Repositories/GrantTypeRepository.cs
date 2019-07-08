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
    public class GrantTypeRepository : RepositoryBase
    {
        private readonly List<QueryOrderBy<GrantTypeEntity>> m_defaultOrdering;

        public GrantTypeRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            m_defaultOrdering = new List<QueryOrderBy<GrantTypeEntity>>
            {
                new QueryOrderBy<GrantTypeEntity> {Expression = x => x.DisplayName}
            };
        }

        private void FetchCollections(ISession session, ICriterion criterion = null,
            IList<QueryJoinAlias<GrantTypeEntity>> joinAliases = null)
        {
            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.Clients)
                .Future<GrantTypeEntity>();
        }

        public IList<GrantTypeEntity> GetAllGrantTypes()
        {
            try
            {
                return GetValuesList(FetchCollections, null, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get all grant types list operation failed", ex);
            }
        }

        public IList<GrantTypeEntity> GetGrantTypesById(IEnumerable<int> ids)
        {
            var criterion = Restrictions.On<GrantTypeEntity>(x => x.Id).IsIn(ids.ToList());

            try
            {
                return GetValuesList(FetchCollections, criterion, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get grant types list by ids operation failed", ex);
            }
        }
    }
}