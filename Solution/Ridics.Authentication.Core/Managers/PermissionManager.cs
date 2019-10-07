using System.Collections.Generic;
using System.Linq;
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
    public class PermissionManager : ManagerBase
    {
        private readonly PermissionUoW m_permissionUoW;

        public PermissionManager(PermissionUoW permissionUoW, ILogger logger, ITranslator translator, IMapper mapper,
            IPaginationConfiguration paginationConfiguration) : base(logger, translator, mapper,
            paginationConfiguration)
        {
            m_permissionUoW = permissionUoW;
        }

        public DataResult<PermissionModel> FindPermissionById(int id, bool fetchRoles)
        {
            try
            {
                var permission = m_permissionUoW.FindPermissionById(id, fetchRoles);
                var viewModel = m_mapper.Map<PermissionModel>(permission);
                return Success(viewModel);
            }
            catch (NoResultException<PermissionEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<PermissionModel>(m_translator.Translate("invalid-permission-id"), DataResultErrorCode.PermissionNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<PermissionModel>(e.Message);
            }
        }


        public DataResult<List<PermissionModel>> GetPermissions(int start, int count, string searchByName, bool fetchRoles)
        {
            try
            {
                var permissions = m_permissionUoW.GetPermissions(start, GetItemsOnPageCount(count), searchByName, fetchRoles);
                var viewModelList = m_mapper.Map<List<PermissionModel>>(permissions);
                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<PermissionModel>>(e.Message);
            }
        }


        public DataResult<int> GetPermissionsCount(string searchByName)
        {
            try
            {
                var permissionsCount = m_permissionUoW.GetPermissionsCount(searchByName);
                return Success(permissionsCount);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<bool> CheckUserHasPermission(int userId, string permissionName)
        {
            try
            {
                var permission = m_permissionUoW.GetPermissionByUserByName(userId, permissionName);
                var permissionExists = permission != null;
                return Success(permissionExists);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<int> CreatePermission(PermissionModel permissionModel)
        {
            var permission = new PermissionEntity
            {
                Name = permissionModel.Name,
                Description = permissionModel.Description
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

        public DataResult<bool> UpdatePermission(int id, PermissionModel permissionModel)
        {
            var permission = new PermissionEntity
            {
                Name = permissionModel.Name,
                Description = permissionModel.Description
            };

            try
            {
                m_permissionUoW.UpdatePermission(id, permission);
                return Success(true);
            }
            catch (NoResultException<PermissionEntity> e)
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
            catch (NoResultException<PermissionEntity> e)
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

        public DataResult<List<PermissionModel>> GetAllPermissions(string search = null)
        {
            try
            {
                var permissions = m_permissionUoW.GetAllPermissions(false, search);
                var viewModelList = m_mapper.Map<List<PermissionModel>>(permissions);
                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<PermissionModel>>(e.Message);
            }
        }

        public DataResult<bool> AssignRolesToPermissions(int id, IEnumerable<int> roleIds, bool overwriteAuthOnlyRoles = true)
        {
            try
            {
                m_permissionUoW.AssignRolesToPermission(id, roleIds, overwriteAuthOnlyRoles);
                return Success(true);
            }
            catch (NoResultException<PermissionEntity> e)
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

        public DataResult<bool> EnsurePermissionsExist(IList<PermissionInfoModel> permissions, string newAssignToRoleName)
        {
            try
            {
                var permissionEntities = permissions.Select(x => new PermissionEntity
                {
                    Name = x.Name,
                    Description = x.Description,
                });
                m_permissionUoW.EnsurePermissionsExist(permissionEntities, newAssignToRoleName);
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