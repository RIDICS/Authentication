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
    public class ResourcePermissionTypeActionManager : ManagerBase
    {
        private readonly ResourcePermissionTypeActionUoW m_permissionTypeActionUoW;
        private readonly ResourcePermissionTypeUoW m_permissionTypeUoW;

        public ResourcePermissionTypeActionManager(ResourcePermissionTypeActionUoW permissionTypeActionUoW, ILogger logger, ITranslator translator, IMapper mapper,
            IPaginationConfiguration paginationConfiguration, ResourcePermissionTypeUoW permissionTypeUoW) : base(logger, translator, mapper,
            paginationConfiguration)
        {
            m_permissionTypeActionUoW = permissionTypeActionUoW;
            m_permissionTypeUoW = permissionTypeUoW;
        }

        public DataResult<ResourcePermissionTypeActionModel> FindPermissionTypeActionById(int id)
        {
            try
            {
                var permission = m_permissionTypeActionUoW.FindPermissionTypeActionById(id);
                var viewModel = m_mapper.Map<ResourcePermissionTypeActionModel>(permission);
                return Success(viewModel);
            }
            catch (NoResultException<ResourcePermissionTypeActionEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<ResourcePermissionTypeActionModel>(m_translator.Translate("invalid-permission-type-id"), DataResultErrorCode.PermissionNotExistId); 
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<ResourcePermissionTypeActionModel>(e.Message);
            }
        }


        public DataResult<List<ResourcePermissionTypeActionModel>> GetPermissionTypeActions(int start, int count, string searchByName)
        {
            try
            {
                var permissions = m_permissionTypeActionUoW.GetPermissionTypeActions(start, GetItemsOnPageCount(count), searchByName);
                var viewModelList = m_mapper.Map<List<ResourcePermissionTypeActionModel>>(permissions);
                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<ResourcePermissionTypeActionModel>>(e.Message);
            }
        }


        public DataResult<int> GetPermissionTypeActionsCount(string searchByName)
        {
            try
            {
                var permissionsCount = m_permissionTypeActionUoW.GetPermissionTypeActionsCount(searchByName);
                return Success(permissionsCount);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<int> CreatePermissionTypeAction(ResourcePermissionTypeActionModel permissionTypeModel)
        {
            try
            {
                var permissionType = new ResourcePermissionTypeActionEntity
                {
                    Name = permissionTypeModel.Name,
                    Description = permissionTypeModel.Description,
                    ResourcePermissionType = m_permissionTypeUoW.FindPermissionTypeById(permissionTypeModel.ResourcePermissionType.Id)
                };

                var result = m_permissionTypeActionUoW.CreatePermissionTypeAction(permissionType);
                return Success(result);
            }
            catch (NoResultException<ResourcePermissionTypeEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<int>(m_translator.Translate("invalid-permission-type-id"), DataResultErrorCode.PermissionTypeNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<bool> UpdatePermissionType(int id, ResourcePermissionTypeActionModel permissionTypeModel)
        {
            try
            {
                var permissionType = new ResourcePermissionTypeActionEntity
                {
                    Name = permissionTypeModel.Name,
                    Description = permissionTypeModel.Description,
                    ResourcePermissionType = m_permissionTypeUoW.FindPermissionTypeById(permissionTypeModel.ResourcePermissionType.Id)
                };

                m_permissionTypeActionUoW.UpdatePermissionTypeAction(id, permissionType);
                return Success(true);
            }
            catch (NoResultException<ResourcePermissionTypeEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-permission-type-id"), DataResultErrorCode.PermissionTypeNotExistId);
            }
            catch (NoResultException<ResourcePermissionTypeActionEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-permission-type-id"), DataResultErrorCode.PermissionNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> DeletePermissionTypeActionWithId(int id)
        {
            try
            {
                m_permissionTypeActionUoW.DeletePermissionTypeActionById(id);
                return Success(true);
            }
            catch (NoResultException<ResourcePermissionTypeActionEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-permission-type-id"), DataResultErrorCode.PermissionNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<List<ResourcePermissionTypeActionModel>> GetAllPermissionTypeActions()
        {
            try
            {
                var permissions = m_permissionTypeActionUoW.GetAllPermissionTypeActions();
                var viewModelList = m_mapper.Map<List<ResourcePermissionTypeActionModel>>(permissions);
                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<ResourcePermissionTypeActionModel>>(e.Message);
            }
        }

        public DataResult<bool> AssignRolesToPermissionTypeAction(int id, IEnumerable<int> rolesIds, bool overwriteAuthOnlyRoles = true)
        {
            try
            {
                m_permissionTypeActionUoW.AssignRolesToPermissionTypeAction(id, rolesIds, overwriteAuthOnlyRoles);
                return Success(true);
            }
            catch (NoResultException<ResourcePermissionTypeActionEntity> e)
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

        public DataResult<bool> AssignUsersToPermissionTypeAction(int id, IEnumerable<int> usersIds, bool overwriteAuthOnlyRoles = true)
        {
            try
            {
                m_permissionTypeActionUoW.AssignUsersToPermissionTypeAction(id, usersIds);
                return Success(true);
            }
            catch (NoResultException<ResourcePermissionTypeActionEntity> e)
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

        public DataResult<List<ResourcePermissionTypeActionModel>> GetActionsForResourcePermissionTypeById(int id)
        {
            try
            {
                var permissions = m_permissionTypeActionUoW.GetActionsForResourcePermissionTypeById(id);
                var viewModelList = m_mapper.Map<List<ResourcePermissionTypeActionModel>>(permissions);
                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<ResourcePermissionTypeActionModel>>(e.Message);
            }
        }
    }
}