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
    public class ClaimTypeEnumRepository : RepositoryBase
    {
        private readonly List<QueryOrderBy<ClaimTypeEnumEntity>> m_defaultOrdering;

        public ClaimTypeEnumRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            m_defaultOrdering = new List<QueryOrderBy<ClaimTypeEnumEntity>>
            {
                new QueryOrderBy<ClaimTypeEnumEntity> {Expression = x => x.Name}
            };
        }

        private void FetchCollections(ISession session, ICriterion criterion = null, IList<QueryJoinAlias<ClaimTypeEnumEntity>> joinAliases = null)
        {
            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.ClaimTypes)
                .Future<ClaimTypeEnumEntity>();
        }

        public IList<ClaimTypeEnumEntity> GetAllClaimTypesEnum()
        {
            try
            {
                return GetValuesList(FetchCollections, null, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get claim type enum list operation failed", ex);
            }
        }
    }
}