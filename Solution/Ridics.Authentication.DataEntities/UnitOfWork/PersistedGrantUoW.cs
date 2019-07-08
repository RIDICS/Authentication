using System.Collections.Generic;
using DryIoc.Facilities.NHibernate;
using DryIoc.Transactions;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Exceptions;
using Ridics.Authentication.DataEntities.Repositories;
using Ridics.Core.DataEntities.Shared.UnitOfWorks;

namespace Ridics.Authentication.DataEntities.UnitOfWork
{
    public class PersistedGrantUoW : UnitOfWorkBase
    {
        private readonly PersistedGrantRepository m_persistedGrantRepository;
        private readonly UserRepository m_userRepository;
        private readonly ClientRepository m_clientRepository;

        public PersistedGrantUoW(ISessionManager sessionManager, PersistedGrantRepository persistedGrantRepository,
            UserRepository userRepository,
            ClientRepository clientRepository) :
            base(sessionManager)
        {
            m_persistedGrantRepository = persistedGrantRepository;
            m_userRepository = userRepository;
            m_clientRepository = clientRepository;
        }

        [Transaction]
        public virtual void SavePersistedGrant(int userId, string grantClientId, PersistedGrantEntity newGrant)
        {
            var user = m_userRepository.FindById<UserEntity>(userId);
            
            if (user == null)
            {
                throw new NoResultException<UserEntity>();
            }

            var client = m_clientRepository.FindClientByName(grantClientId);

            if (client == null)
            {
                throw new NoResultException<ClientEntity>();
            }

            var persistedGrant = m_persistedGrantRepository.FindByKey(newGrant.Key);

            if (persistedGrant != null) //TODO Investigate performance of this
            {
                persistedGrant.Client = client;
                persistedGrant.User = user;
                persistedGrant.CreationTime = newGrant.CreationTime;
                persistedGrant.ExpirationTime = newGrant.ExpirationTime;
                persistedGrant.Type = newGrant.Type;
                persistedGrant.Data = newGrant.Data;
              
                m_persistedGrantRepository.Save(persistedGrant);
            }
            else
            {
                newGrant.User = user;
                newGrant.Client = client;

                m_persistedGrantRepository.Create(newGrant);
            }
        }

        [Transaction]
        public virtual PersistedGrantEntity FindPersistedGrantByKey(string key)
        {
            var result = m_persistedGrantRepository.FindByKey(key);

            return result;
        }

        [Transaction]
        public virtual IList<PersistedGrantEntity> GetAllPersistedGrantForUser(int id)
        {
            var result = m_persistedGrantRepository.GetAllForUser(id);

            return result;
        }

        [Transaction]
        public virtual void DeleteGrantByKey(string key)
        {
            var persistedGrantEntity = m_persistedGrantRepository.FindByKey(key);
            m_persistedGrantRepository.Delete(persistedGrantEntity);
        }

        [Transaction]
        public virtual void DeleteAllPersistedGrant(int userId, int clientId, string type)
        {
            var persistedGrantEntityList = type == null
                ? m_persistedGrantRepository.GetAllForUserClient(userId, clientId)
                : m_persistedGrantRepository.GetAllForUserClientType(userId, clientId, type);
            m_persistedGrantRepository.DeleteAll(persistedGrantEntityList);
        }
    }
}