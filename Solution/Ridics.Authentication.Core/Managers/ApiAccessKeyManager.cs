using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Configuration;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.DataResult;
using Ridics.Authentication.Core.Models.Enum;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;
using Ridics.Authentication.DataEntities.Exceptions;
using Ridics.Authentication.DataEntities.UnitOfWork;
using Ridics.Authentication.Shared;
using Ridics.Core.Shared;
using Ridics.Core.Shared.Exceptions;

namespace Ridics.Authentication.Core.Managers
{
    public class ApiAccessKeyManager : ManagerBase
    {
        private readonly ApiAccessKeyUoW m_apiAccessKeyUoW;
        private readonly HashManager m_hasher;

        public ApiAccessKeyManager(ApiAccessKeyUoW apiAccessKeyUoW, ILogger logger, ITranslator translator, IMapper mapper,
            IPaginationConfiguration paginationConfiguration, HashManager hasher) : base(logger, translator, mapper, paginationConfiguration)
        {
            m_apiAccessKeyUoW = apiAccessKeyUoW;
            m_hasher = hasher;
        }

        public IList<ApiAccessKeyModel> GetAllHashedKeys()
        {
            var apiAccessKeyList = m_apiAccessKeyUoW.GetAllAccessKeys();
            var model = m_mapper.Map<IList<ApiAccessKeyModel>>(apiAccessKeyList);

            return model;
        }

        public DataResult<List<ApiAccessKeyModel>> GetApiAccessKeys(int start, int count, string searchByName)
        {
            try
            {
                var apiAccessKeys = m_apiAccessKeyUoW.GetApiAccessKeys(start, GetItemsOnPageCount(count), searchByName);
                var modelList = m_mapper.Map<List<ApiAccessKeyModel>>(apiAccessKeys);
                return Success(modelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<ApiAccessKeyModel>>(e.Message);
            }
        }

        public DataResult<int> GetApiAccessKeysCount(string searchByName)
        {
            try
            {
                var apiAccessKeysCount = m_apiAccessKeyUoW.GetApiAccessKeysCount(searchByName);
                return Success(apiAccessKeysCount);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<bool> DeleteApiAccessKeyWithId(int id)
        {
            try
            {
                m_apiAccessKeyUoW.DeleteApiAccessKey(id);
                return Success(true);
            }
            catch (NoResultException<ApiAccessKeyModel> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-api-access-key-id"), DataResultErrorCode.ApiAccessKeyNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<ApiAccessKeyModel> FindApiAccessKeyById(int id)
        {
            try
            {
                var apiResource = m_apiAccessKeyUoW.FindApiAccessKeyById(id);
                var model = m_mapper.Map<ApiAccessKeyModel>(apiResource);
                return Success(model);
            }
            catch (NoResultException<ApiAccessKeyModel> e)
            {
                m_logger.LogWarning(e);
                return Error<ApiAccessKeyModel>(m_translator.Translate("invalid-api-access-key-id"), DataResultErrorCode.ApiAccessKeyNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<ApiAccessKeyModel>(e.Message);
            }
        }

        public DataResult<int> CreateApiAccessKey(ApiAccessKeyModel apiAccessKeyModel, IEnumerable<ApiAccessPermissionEnumModel> permissions)
        {
            var apiAccessKeyEntity = new ApiAccessKeyEntity
            {
                Name = apiAccessKeyModel.Name,
                HashAlgorithm = apiAccessKeyModel.HashAlgorithm,
                ApiKeyHash = apiAccessKeyModel.ApiKeyHash,
            };

            var permissionEntities = m_mapper.Map<IList<ApiAccessPermissionEnum>>(permissions);

            try
            {
                var result = m_apiAccessKeyUoW.CreateApiAccessKey(apiAccessKeyEntity, permissionEntities);
                return Success(result);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<bool> UpdateApiAccessKey(int id, ApiAccessKeyModel apiAccessKeyModel, IEnumerable<ApiAccessPermissionEnumModel> permissions)
        {
            var apiAccessKeyEntity = new ApiAccessKeyEntity
            {
                Name = apiAccessKeyModel.Name,
            };

            var permissionEntities = m_mapper.Map<IList<ApiAccessPermissionEnum>>(permissions);

            try
            {
                m_apiAccessKeyUoW.UpdateApiAccessKey(id, apiAccessKeyEntity, permissionEntities);
                return Success(true);
            }
            catch (NoResultException<ApiAccessKeyEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-api-access-key-id"), DataResultErrorCode.ApiAccessKeyNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> UpdateApiAccessKeyHash(int id, string hash, string algorithm)
        {
            var apiAccessKeyEntity = new ApiAccessKeyEntity
            {
                ApiKeyHash = hash,
                HashAlgorithm = algorithm,
            };

            try
            {
                m_apiAccessKeyUoW.UpdateApiAccessKeyHash(id, apiAccessKeyEntity);
                return Success(true);
            }
            catch (NoResultException<ApiAccessKeyEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-api-access-key-id"), DataResultErrorCode.ApiAccessKeyNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        /// <summary>
        /// Verifies API access token against token hashes in database
        /// </summary>
        /// <param name="apiToken">API access token for verification</param>
        /// <param name="requiredPermission"></param>
        /// <returns>True if token was verified, false otherwise</returns>
        public bool VerifyApplicationToken(string apiToken, ApiAccessPermissionEnumModel requiredPermission)
        {
            if (string.IsNullOrEmpty(apiToken))
            {
                return false;
            }

            var hashedKeys = GetAllHashedKeys();
            foreach (var hashedKey in hashedKeys)
            {
                var hashingAlgorithm = hashedKey.HashAlgorithm;
                if (m_hasher.ValidateHash(apiToken, hashedKey.ApiKeyHash, hashingAlgorithm))
                {
                    var requiredPermissionInt = Convert.ToInt32(requiredPermission);

                    if (hashedKey.Permissions.Any(x => x.Permission == requiredPermissionInt))
                    {
                        return true;
                    }

                }

                //TODO implement other algorithms
            }

            return false;
        }
    }
}