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
    public class UriTypeRepository : RepositoryBase
    {
        private readonly List<QueryOrderBy<UriTypeEntity>> m_defaultOrdering;

        public UriTypeRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            m_defaultOrdering = new List<QueryOrderBy<UriTypeEntity>>
            {
                new QueryOrderBy<UriTypeEntity> {Expression = x => x.Value}
            };
        }

        private void FetchCollections(ISession session, ICriterion criterion = null,
            IList<QueryJoinAlias<UriTypeEntity>> joinAliases = null)
        {
            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.UriEntities)
                .Future();
        }

        public IList<UriTypeEntity> GetAllUriTypes()
        {
            try
            {
                return GetValuesList<UriTypeEntity>(FetchCollections, null, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get all uri types operation failed", ex);
            }
        }
    }
}