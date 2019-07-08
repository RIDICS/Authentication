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
    public class ResourcePermissionTypeManager : ManagerBase
    {
        private readonly ResourcePermissionTypeUoW m_permissionTypeUoW;

        public ResourcePermissionTypeManager(ResourcePermissionTypeUoW permissionTypeUoW, ILogger logger, ITranslator translator,
            IMapper mapper,
            IPaginationConfiguration paginationConfiguration) : base(logger, translator, mapper,
            paginationConfiguration)
        {
            m_permissionTypeUoW = permissionTypeUoW;
        }

        public DataResult<ResourcePermissionTypeModel> FindPermissionTypeById(int id)
        {
            try
            {
                var permission = m_permissionTypeUoW.FindPermissionTypeById(id);
                var viewModel = m_mapper.Map<ResourcePermissionTypeModel>(permission);
                return Success(viewModel);
            }
            catch (NoResultException<ResourcePermissionTypeEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<ResourcePermissionTypeModel>(m_translator.Translate("invalid-permission-type-id"),
                    DataResultErrorCode.PermissionNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<ResourcePermissionTypeModel>(e.Message);
            }
        }


        public DataResult<List<ResourcePermissionTypeModel>> GetPermissionTypes(int start, int count, string searchByName)
        {
            try
            {
                var permissions = m_permissionTypeUoW.GetPermissionTypes(start, GetItemsOnPageCount(count), searchByName);
                var viewModelList = m_mapper.Map<List<ResourcePermissionTypeModel>>(permissions);
                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<ResourcePermissionTypeModel>>(e.Message);
            }
        }


        public DataResult<int> GetPermissionTypesCount(string searchByName)
        {
            try
            {
                var permissionsCount = m_permissionTypeUoW.GetPermissionTypesCount(searchByName);
                return Success(permissionsCount);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<int> CreatePermissionType(ResourcePermissionTypeModel permissionTypeModel)
        {
            var permissionType = new ResourcePermissionTypeEntity
            {
                Name = permissionTypeModel.Name,
                Description = permissionTypeModel.Description
            };

            try
            {
                var result = m_permissionTypeUoW.CreatePermissionType(permissionType);
                return Success(result);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<bool> UpdatePermissionType(int id, ResourcePermissionTypeModel permissionTypeModel)
        {
            var permissionType = new ResourcePermissionTypeEntity
            {
                Name = permissionTypeModel.Name,
                Description = permissionTypeModel.Description
            };

            try
            {
                m_permissionTypeUoW.UpdatePermissionType(id, permissionType);
                return Success(true);
            }
            catch (NoResultException<ResourcePermissionTypeEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-permission-type-id"),
                    DataResultErrorCode.PermissionNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> DeletePermissionTypeWithId(int id)
        {
            try
            {
                m_permissionTypeUoW.DeletePermissionTypeById(id);
                return Success(true);
            }
            catch (NoResultException<ResourcePermissionTypeEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-permission-type-id"),
                    DataResultErrorCode.PermissionNotExistId); 
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<List<ResourcePermissionTypeModel>> GetAllPermissionTypes()
        {
            try
            {
                var permissions = m_permissionTypeUoW.GetAllPermissionTypes();
                var viewModelList = m_mapper.Map<List<ResourcePermissionTypeModel>>(permissions);
                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<ResourcePermissionTypeModel>>(e.Message);
            }
        }

        public DataResult<bool> AssignActionsToPermissionTypeAction(int id, IEnumerable<int> selectedActions)
        {
            try
            {
                m_permissionTypeUoW.AssignActionsToPermissionTypeAction(id, selectedActions);
                return Success(true);
            }
            catch (NoResultException<ResourcePermissionTypeEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-permission-id"), DataResultErrorCode.PermissionNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }
    }
}