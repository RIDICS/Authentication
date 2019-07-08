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
    public class ExternalIdentityRepository : RepositoryBase
    {
        private readonly List<QueryOrderBy<ExternalIdentityEntity>> m_defaultOrdering;

        public ExternalIdentityRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            m_defaultOrdering = new List<QueryOrderBy<ExternalIdentityEntity>>
            {
                new QueryOrderBy<ExternalIdentityEntity> {Expression = x => x.Name}
            };
        }

        public IList<ExternalIdentityEntity> FindAllExternalIdentity()
        {
            try
            {
                return GetValuesList(null, null, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find all externalIdentity list operation failed", ex);
            }
        }

        public ExternalIdentityEntity GetExternalIdentityByName(string name)
        {
            var criterion = Restrictions.Where<ExternalIdentityEntity>(x => x.Name == name);

            try
            {
                return GetSingleValue<ExternalIdentityEntity>(null, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get externalIdentity by name operation failed", ex);
            }
        }
    }
}