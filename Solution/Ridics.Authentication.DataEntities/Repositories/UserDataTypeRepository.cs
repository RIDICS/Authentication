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
    public class UserDataTypeRepository : RepositoryBase
    {
        private readonly List<QueryOrderBy<UserDataTypeEntity>> m_defaultOrdering;

        public UserDataTypeRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            m_defaultOrdering = new List<QueryOrderBy<UserDataTypeEntity>>
            {
                new QueryOrderBy<UserDataTypeEntity> {Expression = x => x.DataTypeValue}
            };
        }

        public IList<UserDataTypeEntity> GetAllUserDataTypes()
        {
            try
            {
                return GetValuesList<UserDataTypeEntity>(null, null, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get user data type list operation failed", ex);
            }
        }

        public UserDataTypeEntity GetUserDataTypeByValue(string value)
        {
            var criterion = Restrictions.Where<UserDataTypeEntity>(x => x.DataTypeValue == value);

            try
            {
                return GetSingleValue<UserDataTypeEntity>(null, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find user data type by value operation failed", ex);
            }
        }
    }
}