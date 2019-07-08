using System.Collections.Generic;
using DryIoc.Facilities.NHibernate;
using NHibernate;
using NHibernate.Criterion;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;
using Ridics.Core.DataEntities.Shared.Query;
using Ridics.Core.DataEntities.Shared.Repositories;
using Ridics.Core.Shared.Exceptions;

namespace Ridics.Authentication.DataEntities.Repositories
{
    public class LevelOfAssuranceRepository : RepositoryBase
    {
        private readonly List<QueryOrderBy<LevelOfAssuranceEntity>> m_defaultOrdering;

        public LevelOfAssuranceRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            m_defaultOrdering = new List<QueryOrderBy<LevelOfAssuranceEntity>>
            {
                new QueryOrderBy<LevelOfAssuranceEntity> {Expression = x => x.Name}
            };
        }

        public IList<LevelOfAssuranceEntity> FindAllLevelOfAssurance()
        {
            try
            {
                return GetValuesList(null,null, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get level of assurance list operation failed", ex);
            }
        }

        public LevelOfAssuranceEntity GetLevelOfAssuranceByName(LevelOfAssuranceEnum levelOfAssuranceEnum)
        {
            var criterion = Restrictions.Where<LevelOfAssuranceEntity>(x => x.Name == levelOfAssuranceEnum);

            try
            {
                return GetSingleValue<LevelOfAssuranceEntity>(null, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get level of assurance by name operation failed", ex);
            }
        }
    }
}