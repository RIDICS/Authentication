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
    public class UriRepository : RepositoryBase
    {
        private readonly List<QueryOrderBy<UriEntity>> m_defaultOrdering;

        public UriRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            m_defaultOrdering = new List<QueryOrderBy<UriEntity>>
            {
                new QueryOrderBy<UriEntity> {Expression = x => x.Uri}
            };
        }

        private void FetchCollections(ISession session, ICriterion criterion = null,
            IList<QueryJoinAlias<UriEntity>> joinAliases = null)
        {
            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.UriTypes)
                .Fetch(SelectMode.Fetch, x => x.Client)
                .Future();
        }

        public IList<UriEntity> FindUrisForClient(int clientId)
        {
            var criterion = Restrictions.Where<UriEntity>(x => x.Client.Id == clientId);

            try
            {
                return GetValuesList(FetchCollections, criterion, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get uris for client operation failed", ex);
            }
        }

        public IList<UriEntity> GetAllUris()
        {
            try
            {
                return GetValuesList(FetchCollections, null, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get all uris operation failed", ex);
            }
        }
    }
}