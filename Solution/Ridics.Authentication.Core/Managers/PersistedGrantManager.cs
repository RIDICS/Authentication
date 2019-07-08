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
    public class PersistedGrantManager : ManagerBase
    {
        private readonly PersistedGrantUoW m_persistedGrantUoW;

        public PersistedGrantManager(PersistedGrantUoW persistedGrantUoW, ILogger logger, ITranslator translator, IMapper mapper,
            IPaginationConfiguration paginationConfiguration) : base(logger, translator, mapper, paginationConfiguration)
        {
            m_persistedGrantUoW = persistedGrantUoW;
        }

        public DataResult<bool> SavePersistedGrant(PersistedGrantModel grant)
        {
            var newGrant = new PersistedGrantEntity
            {
                Key = grant.Key,
                Type = grant.Type,
                CreationTime = grant.CreationTime,
                ExpirationTime = grant.ExpirationTime,
                Data = grant.Data
            };

            try
            {
                m_persistedGrantUoW.SavePersistedGrant(grant.User.Id, grant.Client.Name, newGrant);
                return Success(true);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-user-id"), DataResultErrorCode.UserNotExistId);
            }
            catch (NoResultException<ClientEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-client-id"), DataResultErrorCode.ClientNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<PersistedGrantModel> FindPersistedGrantByKey(string key)
        {
            try
            {
                var persistedGrantEntity =
                    m_persistedGrantUoW.FindPersistedGrantByKey(key);
                var persistedGrant = m_mapper.Map<PersistedGrantModel>(persistedGrantEntity);
                return Success(persistedGrant);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<PersistedGrantModel>(e.Message);
            }
        }

        public DataResult<List<PersistedGrantModel>> GetAllPersitedGrantForUser(string userId)
        {
            if (!int.TryParse(userId, out var id)) return Error<List<PersistedGrantModel>>();

            try
            {
                var persistedGrantEntityList = m_persistedGrantUoW.GetAllPersistedGrantForUser(id);
                var persistedGrantList = m_mapper.Map<List<PersistedGrantModel>>(persistedGrantEntityList);
                return Success(persistedGrantList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<PersistedGrantModel>>(e.Message);
            }
        }

        public DataResult<bool> DeleteGrantByKey(string key)
        {
            try
            {
                m_persistedGrantUoW.DeleteGrantByKey(key);
                return Success(true);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> DeleteAllPersistedGrant(string userId, string clientId, string type = null)
        {
            if (!int.TryParse(userId, out var parsedUserId) || !int.TryParse(clientId, out var parsedClientId))
            {
                return Error<bool>();
            }

            try
            {
                m_persistedGrantUoW.DeleteAllPersistedGrant(parsedUserId, parsedClientId, type);
                return Success(true);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }
    }
}