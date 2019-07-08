using System.Collections.Generic;
using System.Linq;
using DryIoc.Facilities.NHibernate;
using DryIoc.Transactions;
using NHibernate;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;
using Ridics.Authentication.DataEntities.Exceptions;
using Ridics.Authentication.DataEntities.Repositories;
using Ridics.Core.DataEntities.Shared.UnitOfWorks;

namespace Ridics.Authentication.DataEntities.UnitOfWork
{
    public class ApiAccessKeyUoW : UnitOfWorkBase
    {
        private readonly ApiAccessKeyRepository m_apiAccessKeyRepository;

        public ApiAccessKeyUoW(ISessionManager sessionManager, ApiAccessKeyRepository apiAccessKeyRepository) : base(sessionManager)
        {
            m_apiAccessKeyRepository = apiAccessKeyRepository;
        }

        [Transaction]
        public virtual IList<ApiAccessKeyEntity> GetAllAccessKeys()
        {
            var resultList = m_apiAccessKeyRepository.GetAllAccessKeys();

            return resultList;
        }

        [Transaction]
        public virtual IList<ApiAccessKeyEntity> GetApiAccessKeys(int start, int count, string searchByName)
        {
            var resultList = m_apiAccessKeyRepository.GetApiAccessKeys(start, count, searchByName);

            return resultList;
        }

        [Transaction]
        public virtual int GetApiAccessKeysCount(string searchByName)
        {
            var apiAccessKeysCount = m_apiAccessKeyRepository.GetApiAccessKeysCount(searchByName);

            return apiAccessKeysCount;
        }

        [Transaction]
        public virtual void DeleteApiAccessKey(int id)
        {
            var apiAccessKey = m_apiAccessKeyRepository.Load<ApiAccessKeyEntity>(id);

            try
            {
                m_apiAccessKeyRepository.Delete(apiAccessKey);
            }
            catch (ObjectNotFoundException)
            {
                throw new NoResultException<ApiAccessKeyEntity>();
            }
        }

        [Transaction]
        public virtual ApiAccessKeyEntity FindApiAccessKeyById(int id)
        {
            var apiAccessKey = m_apiAccessKeyRepository.FindApiAccessKeyById(id);

            if (apiAccessKey == null)
            {
                throw new NoResultException<ApiAccessKeyEntity>();
            }

            return apiAccessKey;
        }

        [Transaction]
        public virtual int CreateApiAccessKey(ApiAccessKeyEntity apiAccessKeyEntity, IEnumerable<ApiAccessPermissionEnum> permissions)
        {
            var permissionEntities = permissions.Select(x => new ApiAccessKeyPermissionEntity { Permission = x, ApiAccessKey = apiAccessKeyEntity });
            apiAccessKeyEntity.Permissions = permissionEntities.ToList();

            var result = (int)m_apiAccessKeyRepository.Create(apiAccessKeyEntity);
            return result;
        }

        [Transaction]
        public virtual void UpdateApiAccessKey(int id, ApiAccessKeyEntity apiAccessKey, IEnumerable<ApiAccessPermissionEnum> permissions)
        {
            var apiAccessKeyEntity = m_apiAccessKeyRepository.FindApiAccessKeyById(id);

            if (apiAccessKeyEntity == null)
            { 
                throw new NoResultException<ApiAccessKeyEntity>();
            }

            apiAccessKeyEntity.Name = apiAccessKey.Name;
            //For updating hash and hash algorithm use specific method

            var permissionEntities = permissions.Select(x => new ApiAccessKeyPermissionEntity { Permission = x, ApiAccessKey = apiAccessKeyEntity });

            //Clear list and insert new permissions, do not replace list with new one, otherwise O-R mapping error is thrown
            apiAccessKeyEntity.Permissions.Clear();

            foreach (var x in permissionEntities)
            {
                apiAccessKeyEntity.Permissions.Add(x);
            }
            
            m_apiAccessKeyRepository.Update(apiAccessKeyEntity);
        }

        [Transaction]
        public virtual void UpdateApiAccessKeyHash(int id, ApiAccessKeyEntity apiAccessKey)
        {
            var apiAccessKeyEntity = m_apiAccessKeyRepository.FindApiAccessKeyById(id);

            if (apiAccessKeyEntity == null)
            {
                throw new NoResultException<ApiAccessKeyEntity>();
            }

            apiAccessKeyEntity.HashAlgorithm = apiAccessKey.HashAlgorithm;
            apiAccessKeyEntity.ApiKeyHash = apiAccessKey.ApiKeyHash;
            
            m_apiAccessKeyRepository.Update(apiAccessKeyEntity);
        }
    }
}