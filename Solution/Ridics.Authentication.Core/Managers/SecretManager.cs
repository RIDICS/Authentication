using System.Collections.Generic;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Configuration;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.DataResult;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Exceptions;
using Ridics.Authentication.DataEntities.UnitOfWork;
using Ridics.Authentication.Shared;
using Ridics.Core.Shared;
using Ridics.Core.Shared.Exceptions;

namespace Ridics.Authentication.Core.Managers
{
    public class SecretManager : ManagerBase
    {
        private readonly SecretUoW m_secretUoW;

        public SecretManager(SecretUoW secretUoW, ILogger logger, ITranslator translator, IMapper mapper,
            IPaginationConfiguration paginationConfiguration) : base(logger, translator, mapper, paginationConfiguration)
        {
            m_secretUoW = secretUoW;
        }

        public DataResult<List<SecretModel>> GetSecretsForApiResources(int apiResourceId)
        {
            try
            {
                var secrets = m_secretUoW.GetSecretsForApiResource(apiResourceId);
                var viewModelList = m_mapper.Map<List<SecretModel>>(secrets);
                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<SecretModel>>(e.Message);
            }
        }

        public DataResult<int> AddSecretToApiResource(int apiResourceId, SecretModel secret)
        {
            var newSecret = new ApiSecretEntity
            {
                Value = secret.Value,
                Description = secret.Description,
                Expiration = secret.Expiration
            };

            try
            {
                var result = m_secretUoW.AddSecretToApiResource(apiResourceId, newSecret);
                return Success(result);
            }
            catch (NoResultException<ApiResourceEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<int>(m_translator.Translate("invalid-api-resource-id"), DataResultErrorCode.ApiResourceNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<bool> DeleteSecret(int secretId)
        {
            try
            {
                m_secretUoW.DeleteSecret(secretId);
                return Success(true);
            }
            catch (NoResultException<SecretEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-secret-id"), DataResultErrorCode.SecretNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<List<SecretModel>> GetSecretsForClient(int clientId)
        {
            try
            {
                var secrets = m_secretUoW.GetSecretsForClient(clientId);
                var viewModelList = m_mapper.Map<List<SecretModel>>(secrets);
                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<SecretModel>>(e.Message);
            }
        }

        public DataResult<int> AddSecretToClient(int clientId, SecretModel secret)
        {
            var newSecret = new ClientSecretEntity
            {
                Value = secret.Value,
                Description = secret.Description,
                Expiration = secret.Expiration
            };

            try
            {
                var result = m_secretUoW.AddSecretToClient(clientId, newSecret);
                return Success(result);
            }
            catch (NoResultException<ClientEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<int>(m_translator.Translate("invalid-client-id"), DataResultErrorCode.ClientNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }
    }
}