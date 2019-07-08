using DryIoc.Facilities.NHibernate;
using NHibernate;
using NHibernate.Criterion;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Core.DataEntities.Shared.Repositories;
using Ridics.Core.Shared.Exceptions;

namespace Ridics.Authentication.DataEntities.Repositories
{
    public class ExternalLoginRepository : RepositoryBase
    {
        public ExternalLoginRepository(ISessionManager sessionManager) : base(sessionManager)
        {
        }

        public int GetExternalLoginCountByProvider(int externalLoginProviderId)
        {
            var criterion = Restrictions.Where<ExternalLoginEntity>(x => x.Provider.Id == externalLoginProviderId);

            try
            {
                return GetValuesCount<ExternalLoginEntity>(criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get count externalLoginProvider operation failed", ex);
            }
        }
    }
}