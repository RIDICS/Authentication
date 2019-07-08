using System.Collections.Generic;
using DryIoc.Facilities.NHibernate;
using DryIoc.Transactions;
using NHibernate;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Exceptions;
using Ridics.Authentication.DataEntities.Repositories;
using Ridics.Core.DataEntities.Shared.UnitOfWorks;

namespace Ridics.Authentication.DataEntities.UnitOfWork
{
    public class SecretUoW : UnitOfWorkBase
    {
        private readonly SecretRepository m_secretRepository;
        private readonly ApiResourceRepository m_apiResourceRepository;

        public SecretUoW(ISessionManager sessionManager, SecretRepository secretRepository,
            ApiResourceRepository apiResourceRepository) : base(sessionManager)
        {
            m_secretRepository = secretRepository;
            m_apiResourceRepository = apiResourceRepository;
        }

        [Transaction]
        public virtual IList<ApiSecretEntity> GetSecretsForApiResource(int apiResourceId)
        {
            var secrets = m_secretRepository.GetSecretsForApiResource(apiResourceId);

            return secrets;
        }

        [Transaction]
        public virtual int AddSecretToApiResource(int apiResourceId, ApiSecretEntity newSecret)
        {
            var apiResource = m_apiResourceRepository.FindById<ApiResourceEntity>(apiResourceId);

            newSecret.ApiResource = apiResource ?? throw new NoResultException<ApiResourceEntity>();
            var result = (int) m_secretRepository.Create(newSecret);

            return result;
        }

        [Transaction]
        public virtual void DeleteSecret(int secretId)
        {
            var secret = m_secretRepository.Load<SecretEntity>(secretId);

            try
            {
                m_secretRepository.Delete(secret);
            }
            catch (ObjectNotFoundException)
            {
                throw new NoResultException<SecretEntity>();
            }
        }

        [Transaction]
        public virtual IList<ClientSecretEntity> GetSecretsForClient(int clientId)
        {
            var secrets = m_secretRepository.GetSecretsForClient(clientId);

            return secrets;
        }

        [Transaction]
        public virtual int AddSecretToClient(int clientId, ClientSecretEntity secret)
        {
            var client= m_apiResourceRepository.FindById<ClientEntity>(clientId);

            secret.Client = client ?? throw new NoResultException<ClientEntity>();
            var result = (int) m_secretRepository.Create(secret);

            return result;
        }
    }
}