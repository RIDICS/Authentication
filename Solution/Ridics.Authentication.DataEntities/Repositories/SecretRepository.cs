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
    public class SecretRepository : RepositoryBase
    {
        private readonly List<QueryOrderBy<ApiSecretEntity>> m_apiSecretOrdering;
        private readonly List<QueryOrderBy<ClientSecretEntity>> m_clientSecretOrdering;

        public SecretRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            m_apiSecretOrdering = new List<QueryOrderBy<ApiSecretEntity>>
            {
                new QueryOrderBy<ApiSecretEntity> {Expression = x => x.Value}
            };


            m_clientSecretOrdering = new List<QueryOrderBy<ClientSecretEntity>>
            {
                new QueryOrderBy<ClientSecretEntity> {Expression = x => x.Value}
            };
        }

        public IList<ApiSecretEntity> GetSecretsForApiResource(int apiResourceId)
        {
            var criterion = Restrictions.Where<ApiSecretEntity>(x => x.ApiResource.Id == apiResourceId);

            try
            {
                return GetValuesList(null, criterion, null, m_apiSecretOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get secrets for api resource operation failed", ex);
            }
        }

        public IList<ClientSecretEntity> GetSecretsForClient(int clientId)
        {
            var criterion = Restrictions.Where<ClientSecretEntity>(x => x.Client.Id == clientId);

            try
            {
                return GetValuesList(null, criterion, null, m_clientSecretOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get secrets for client operation failed", ex);
            }
        }
    }
}