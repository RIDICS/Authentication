using System.Collections.Generic;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Configuration;
using Ridics.Authentication.Shared;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.DataResult;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Exceptions;
using Ridics.Authentication.DataEntities.UnitOfWork;
using Ridics.Core.Shared;
using Ridics.Core.Shared.Exceptions;

namespace Ridics.Authentication.Core.Managers
{
    public class ClaimManager : ManagerBase
    {
        private readonly ClaimUoW m_claimUoW;

        public ClaimManager(ClaimUoW claimUoW, ILogger logger, ITranslator translator, IMapper mapper,
            IPaginationConfiguration paginationConfiguration) : base(logger, translator, mapper, paginationConfiguration)
        {
            m_claimUoW = claimUoW;
        }

        public DataResult<List<ClaimModel>> GetUserClaimsByUserId(int userId)
        {
            try
            {
                var claims = m_claimUoW.GetUserClaimsByUserId(userId);
                var claimsViewModel = m_mapper.Map<List<ClaimModel>>(claims);
                return Success(claimsViewModel);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<ClaimModel>>(e.Message);
            }
        }

        public DataResult<int> AddClaimToUser(int userId, ClaimModel claimModel, int claimTypeId)
        {
            var newClaim = new ClaimEntity
            {
                Value = claimModel.Value
            };

            try
            {
                var result = m_claimUoW.AddClaimToUser(userId, newClaim, claimTypeId);
                return Success(result);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<int>(m_translator.Translate("invalid-user-id"), DataResultErrorCode.UserNotExistId);
            }
            catch (NoResultException<ClaimTypeEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<int>(m_translator.Translate("invalid-claim-type"), DataResultErrorCode.ClaimTypeNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<bool> RemoveClaimFromUser(int userId, int claimTypeId)
        {
            try
            {
                m_claimUoW.RemoveClaimFromUser(userId, claimTypeId);
                return Success(true);
            }
            catch (NoResultException<ClaimEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("nothing-to-remove"), DataResultErrorCode.ClaimNothingToRemove);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }
    }
}
