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
    public class ClaimTypeManager : ManagerBase
    {
        private readonly ClaimTypeUoW m_claimTypeUoW;

        public ClaimTypeManager(ClaimTypeUoW claimTypeUoW, ILogger logger, ITranslator translator, IMapper mapper,
            IPaginationConfiguration paginationConfiguration) : base(logger, translator, mapper, paginationConfiguration)
        {
            m_claimTypeUoW = claimTypeUoW;
        }

        public DataResult<ClaimTypeModel> FindClaimTypeById(int id)
        {
            try
            {
                var claimType = m_claimTypeUoW.FindClaimTypeById(id);
                var viewModel = m_mapper.Map<ClaimTypeModel>(claimType);
                return Success(viewModel);
            }
            catch (NoResultException<ClaimTypeEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<ClaimTypeModel>(m_translator.Translate("invalid-claim-type-id"), DataResultErrorCode.ClaimTypeNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<ClaimTypeModel>(e.Message);
            }
        }

        public DataResult<List<ClaimTypeModel>> GetClaimTypes(int start, int count, string searchByName = null)
        {
            try
            {
                var claimTypes = m_claimTypeUoW.GetClaimTypes(start, GetItemsOnPageCount(count), searchByName);
                var viewModelList = m_mapper.Map<List<ClaimTypeModel>>(claimTypes);
                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<ClaimTypeModel>>(e.Message);
            }
        }

        public DataResult<int> GetClaimTypesCount(string searchByName)
        {
            try
            {
                var claimTypesCount = m_claimTypeUoW.GetClaimTypesCount(searchByName);
                return Success(claimTypesCount);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<List<ClaimTypeModel>> GetAllClaimTypes()
        {
            try
            {
                var claimTypes = m_claimTypeUoW.GetAllClaimTypes();
                var viewModelList = m_mapper.Map<List<ClaimTypeModel>>(claimTypes);
                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<ClaimTypeModel>>(e.Message);
            }
        }

        public DataResult<int> CreateClaimType(ClaimTypeModel claimTypeModel)
        {
            var claimType = new ClaimTypeEntity
            {
                Name = claimTypeModel.Name,
                Description = claimTypeModel.Description
            };

            try
            {
                var result = m_claimTypeUoW.CreateClaimType(claimType, claimTypeModel.SelectedType);
                return Success(result);
            }
            catch (NoResultException<ClaimTypeEnumEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<int>(m_translator.Translate("invalid-claim-type-enum-id"), DataResultErrorCode.ClaimTypeEnumNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<bool> UpdateClaimType(ClaimTypeModel claimTypeModel)
        {
            var claimType = new ClaimTypeEntity
            {
                Name = claimTypeModel.Name,
                Description = claimTypeModel.Description
            };

            try
            {
                m_claimTypeUoW.UpdateClaimType(claimTypeModel.Id, claimType, claimTypeModel.SelectedType);
                return Success(true);
            }
            catch (NoResultException<ClaimTypeEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-claim-type-id"), DataResultErrorCode.ClaimTypeNotExistId);
            }
            catch (NoResultException<ClaimTypeEnumEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-claim-type-enum-id"), DataResultErrorCode.ClaimTypeEnumNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> DeleteClaimTypeWithId(int id)
        {
            try
            {
                m_claimTypeUoW.DeleteClaimTypeById(id);
                return Success(true);
            }
            catch (NoResultException<ClaimTypeEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-claim-type-id"), DataResultErrorCode.ClaimTypeNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<IList<ClaimTypeEnumModel>> GetAllClaimTypeEnums()
        {
            try
            {
                var claimTypes = m_claimTypeUoW.GetAllClaimTypeEnums();
                var viewModelList = m_mapper.Map<IList<ClaimTypeEnumModel>>(claimTypes);
                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<IList<ClaimTypeEnumModel>>(e.Message);
            }
        }
    }
}