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
    public class ClaimRepository : RepositoryBase
    {
        private readonly List<QueryOrderBy<ClaimEntity>> m_defaultOrdering;

        public ClaimRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            m_defaultOrdering = new List<QueryOrderBy<ClaimEntity>>
            {
                new QueryOrderBy<ClaimEntity> {Expression = x => x.Value}
            };
        }

        private void FetchCollections(ISession session, ICriterion criterion = null, IList<QueryJoinAlias<ClaimEntity>> joinAliases = null)
        {
            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.ClaimType)
                .Fetch(SelectMode.Fetch, x => x.ClaimType.Type)
                .Future<ClaimEntity>();
        }

        public IList<ClaimEntity> GetClaimsForUser(int userId)
        {
            var criterion = Restrictions.Where<ClaimEntity>(x => x.User.Id == userId);

            try
            {
                return GetValuesList(FetchCollections, criterion, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get claims for user operation failed", ex);
            }
        }

        public IList<ClaimEntity> GetClaimsById(IEnumerable<int> ids)
        {
            var criterion = Restrictions.On<ClaimEntity>(c => c.Id).IsIn(ids.ToList());

            try
            {
                return GetValuesList(FetchCollections, criterion, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get claims list by ids operation failed", ex);
            }
        }
    }
}