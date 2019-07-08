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
    public class ClientUoW : UnitOfWorkBase
    {
        private readonly ClientRepository m_clientRepository;
        private readonly GrantTypeRepository m_grantTypeRepository;
        private readonly IdentityResourceRepository m_identityResourceRepository;
        private readonly ApiResourceRepository m_apiResourceRepository;
        private readonly ScopeRepository m_scopeRepository;

        public ClientUoW(ISessionManager sessionManager, ClientRepository clientRepository,
            GrantTypeRepository grantTypeRepository, IdentityResourceRepository identityResourceRepository,
            ApiResourceRepository apiResourceRepository, ScopeRepository scopeRepository) : base(sessionManager)
        {
            m_clientRepository = clientRepository;
            m_grantTypeRepository = grantTypeRepository;
            m_identityResourceRepository = identityResourceRepository;
            m_apiResourceRepository = apiResourceRepository;
            m_scopeRepository = scopeRepository;
        }

        [Transaction]
        public virtual void DeleteClientById(int id)
        {
            var client = m_clientRepository.Load<ClientEntity>(id);

            try
            {
                m_clientRepository.Delete(client);
            }
            catch (ObjectNotFoundException)
            {
                throw new NoResultException<ClientEntity>();
            }
        }

        [Transaction]
        public virtual ClientEntity FindClientByClientId(string clientId)
        {
            var client = m_clientRepository.FindClientByName(clientId);

            if (client == null)
            {
                throw new NoResultException<ClientEntity>();
            }

            return client;
        }

        [Transaction]
        public virtual ClientEntity FindClientById(int id)
        {
            var client = m_clientRepository.FindClientById(id);

            if (client == null)
            {
                throw new NoResultException<ClientEntity>();
            }

            return client;
        }

        [Transaction]
        public virtual IEnumerable<ClientEntity> GetClients(int start, int count, string searchByName)
        {
            var clients = m_clientRepository.GetClients(start, count, searchByName);

            return clients;
        }

        [Transaction]
        public virtual int GetClientsCount(string searchByName)
        {
            var clientsCount = m_clientRepository.GetClientsCount(searchByName);

            return clientsCount;
        }

        [Transaction]
        public virtual int CreateClient(ClientEntity newClient, IEnumerable<int> identityResourcesIds, IEnumerable<int> grantTypesIds, IEnumerable<int> scopesIds)
        {
            var identityResources = identityResourcesIds == null
                ? null
                : m_identityResourceRepository.GetResourcesById(identityResourcesIds);
            var grantTypes = grantTypesIds == null ? null : m_grantTypeRepository.GetGrantTypesById(grantTypesIds);

            var scopes = scopesIds == null ? null : m_scopeRepository.GetScopesById(scopesIds);

            if (grantTypes != null)
            {
                newClient.AllowedGrantTypes = new HashSet<GrantTypeEntity>(grantTypes);
            }

            if (identityResources != null)
            {
                newClient.AllowedIdentityResources = new HashSet<IdentityResourceEntity>(identityResources);
            }

            if (scopes != null)
            {
                newClient.AllowedScopes = new HashSet<ScopeEntity>(scopes);
            }

            var result = (int) m_clientRepository.Create(newClient);

            return result;
        }

        [Transaction]
        public virtual void UpdateClient(int id, ClientEntity client, IEnumerable<int> identityResourcesIds, IEnumerable<int> grantTypesIds, IEnumerable<int> scopeIds)
        {
            var clientEntity = m_clientRepository.FindById<ClientEntity>(id);

            if (clientEntity == null)
            {
                throw new NoResultException<ClientEntity>();
            }

            clientEntity.Name = client.Name;
            clientEntity.Description = client.Description;
            clientEntity.DisplayUrl = client.DisplayUrl;
            clientEntity.LogoUrl = client.LogoUrl;
            clientEntity.RequireConsent = client.RequireConsent;
            clientEntity.AllowAccessTokensViaBrowser = client.AllowAccessTokensViaBrowser;

            var identityResources = identityResourcesIds == null
                ? null
                : m_identityResourceRepository.GetResourcesById(identityResourcesIds);
            var grantTypes = grantTypesIds == null ? null : m_grantTypeRepository.GetGrantTypesById(grantTypesIds);
            var scopes = scopeIds == null ? null : m_scopeRepository.GetScopesById(scopeIds);

            if (grantTypes != null)
            {
                clientEntity.AllowedGrantTypes = new HashSet<GrantTypeEntity>(grantTypes);
            }

            if (identityResources != null)
            {
                clientEntity.AllowedIdentityResources = new HashSet<IdentityResourceEntity>(identityResources);
            }

            if (scopes != null)
            {
                clientEntity.AllowedScopes = new HashSet<ScopeEntity>(scopes);
            }

            m_clientRepository.Update(clientEntity);
        }
    }
}