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
    public class ResourcePermissionManager : ManagerBase
    {
        private readonly ResourcePermissionUoW m_permissionUoW;

        public ResourcePermissionManager(ResourcePermissionUoW permissionUoW, ILogger logger, ITranslator translator, IMapper mapper,
            IPaginationConfiguration paginationConfiguration) : base(logger, translator, mapper,
            paginationConfiguration)
        {
            m_permissionUoW = permissionUoW;
        }

        public DataResult<ResourcePermissionModel> FindPermissionById(int id)
        {
            try
            {
                var permission = m_permissionUoW.FindPermissionById(id);
                var viewModel = m_mapper.Map<ResourcePermissionModel>(permission);
                return Success(viewModel);
            }
            catch (NoResultException<ResourcePermissionEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<ResourcePermissionModel>(m_translator.Translate("invalid-permission-id"), DataResultErrorCode.PermissionNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<ResourcePermissionModel>(e.Message);
            }
        }


        public DataResult<List<ResourcePermissionModel>> GetPermissions(int start, int count)
        {
            try
            {
                var permissions = m_permissionUoW.GetPermissions(start, GetItemsOnPageCount(count));
                var viewModelList = m_mapper.Map<List<ResourcePermissionModel>>(permissions);
                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<ResourcePermissionModel>>(e.Message);
            }
        }


        public DataResult<int> GetPermissionsCount()
        {
            try
            {
                var permissionsCount = m_permissionUoW.GetPermissionsCount();
                return Success(permissionsCount);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<int> CreatePermission(ResourcePermissionModel permissionModel)
        {
            var permission = new ResourcePermissionEntity
            {
                ResourceId = permissionModel.ResourceId,
                ResourceTypeAction = m_permissionUoW.LoadPermissionTypeActionById(permissionModel.ResourceTypeAction.Id)
            };

            try
            {
                var result = m_permissionUoW.CreatePermission(permission);
                return Success(result);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<bool> UpdatePermission(int id, ResourcePermissionModel permissionModel)
        {
            var permission = new ResourcePermissionEntity
            {
                ResourceId = permissionModel.ResourceId,
                ResourceTypeAction = m_permissionUoW.LoadPermissionTypeActionById(permissionModel.ResourceTypeAction.Id)
            };

            try
            {
                m_permissionUoW.UpdatePermission(id, permission);
                return Success(true);
            }
            catch (NoResultException<ResourcePermissionEntity> e)
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

        public DataResult<bool> DeletePermissionWithId(int id)
        {
            try
            {
                m_permissionUoW.DeletePermissionById(id);
                return Success(true);
            }
            catch (NoResultException<ResourcePermissionEntity> e)
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

        public DataResult<List<ResourcePermissionModel>> GetAllPermissions()
        {
            try
            {
                var permissions = m_permissionUoW.GetAllPermissions();
                var viewModelList = m_mapper.Map<List<ResourcePermissionModel>>(permissions);
                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<ResourcePermissionModel>>(e.Message);
            }
        }

        public DataResult<bool> AssignRolesToPermission(int id, IEnumerable<int> rolesIds, bool overwriteAuthOnlyRoles = true)
        {
            try
            {
                m_permissionUoW.AssignRolesToPermission(id, rolesIds, overwriteAuthOnlyRoles);
                return Success(true);
            }
            catch (NoResultException<ResourcePermissionEntity> e)
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

        public DataResult<bool> AssignUsersToPermission(int id, IEnumerable<int> usersIds, bool overwriteAuthOnlyRoles = true)
        {
            try
            {
                m_permissionUoW.AssignUsersToPermission(id, usersIds);
                return Success(true);
            }
            catch (NoResultException<ResourcePermissionEntity> e)
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