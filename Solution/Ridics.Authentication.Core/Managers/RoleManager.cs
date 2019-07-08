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
    public class RoleManager : ManagerBase
    {
        private readonly RoleUoW m_roleUoW;

        public RoleManager(RoleUoW roleUoW, ILogger logger, ITranslator translator, IMapper mapper,
            IPaginationConfiguration paginationConfiguration) : base(logger, translator, mapper,
            paginationConfiguration)
        {
            m_roleUoW = roleUoW;
        }

        public DataResult<RoleModel> FindRoleById(int id)
        {
            try
            {
                var role = m_roleUoW.FindRoleById(id);
                var viewModel = m_mapper.Map<RoleModel>(role);

                return Success(viewModel);
            }
            catch (NoResultException<RoleEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<RoleModel>(m_translator.Translate("invalid-role-id"), DataResultErrorCode.RoleNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<RoleModel>(e.Message);
            }
        }

        public DataResult<List<RoleModel>> GetAllRoles()
        {
            var roles = m_roleUoW.GetAllRoles();

            if (roles == null)
            {
                return Error<List<RoleModel>>();
            }

            var viewModelList = m_mapper.Map<List<RoleModel>>(roles);

            return Success(viewModelList);
        }

        public DataResult<List<RoleModel>> GetAllNonAuthenticationServiceRoles()
        {
            var roles = m_roleUoW.GetAllNonAuthenticationServiceRoles();

            if (roles == null)
            {
                return Error<List<RoleModel>>();
            }

            var viewModelList = m_mapper.Map<List<RoleModel>>(roles);

            return Success(viewModelList);
        }

        public DataResult<List<RoleModel>> GetRoles(int start, int count, string searchByName)
        {
            try
            {
                var roles = m_roleUoW.GetRoles(start, GetItemsOnPageCount(count), searchByName);
                var viewModelList = m_mapper.Map<List<RoleModel>>(roles);

                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<RoleModel>>(e.Message);
            }
        }

        public DataResult<int> GetRolesCount(string searchByName)
        {
            try
            {
                var rolesCount = m_roleUoW.GetRolesCount(searchByName);
                return Success(rolesCount);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
            
        }

        public DataResult<List<RoleModel>> GetNonAuthenticationServiceRoles(int start, int count, string searchByName)
        {
            try
            {
                var roles = m_roleUoW.GetNonAuthenticationServiceRoles(start, GetItemsOnPageCount(count), searchByName);
                var viewModelList = m_mapper.Map<List<RoleModel>>(roles);
                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<RoleModel>>(e.Message);
            }
        }

        public DataResult<int> GetNonAuthenticationServiceRolesCount(string searchByName)
        {
            try
            {
                var rolesCount = m_roleUoW.GetNonAuthenticationServiceRolesCount(searchByName);
                return Success(rolesCount);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<List<RoleModel>> GetRolesByUser(int userId)
        {
            var roles = m_roleUoW.GetRolesByUser(userId);
            var viewModelList = m_mapper.Map<List<RoleModel>>(roles);

            return Success(viewModelList);
        }

        public DataResult<int> CreateRole(RoleModel roleModel)
        {
            var role = new RoleEntity()
            {
                Name = roleModel.Name,
                Description = roleModel.Description,
            };

            try
            {
                var result = m_roleUoW.CreateRole(role);

                return Success(result);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<bool> UpdateRole(int id, RoleModel roleModel)
        {
            var role = new RoleEntity
            {
                Name = roleModel.Name,
                Description = roleModel.Description
            };

            try
            {
                m_roleUoW.UpdateRole(id, role);

                return Success(true);
            }
            catch (NoResultException<RoleEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-role-id"), DataResultErrorCode.RoleNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> DeleteRoleWithId(int id)
        {
            try
            {
                m_roleUoW.DeleteRoleById(id);

                return Success(true);
            }
            catch (NoResultException<RoleEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-role-id"), DataResultErrorCode.RoleNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> AssignPermissionsToRole(int id, IEnumerable<int> permissionIds)
        {
            try
            {
                m_roleUoW.AssignPermissionsToRole(id, permissionIds);

                return Success(true);
            }
            catch (NoResultException<RoleEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-role-id"), DataResultErrorCode.RoleNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> AssignResourcePermissionsToRole(int id, IEnumerable<int> permissionIds)
        {
            try
            {
                m_roleUoW.AssignResourcePermissionsToRole(id, permissionIds);

                return Success(true);
            }
            catch (NoResultException<RoleEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-role-id"), DataResultErrorCode.RoleNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> AssignResourcePermissionTypeActionsToRole(int id, IEnumerable<int> permissionTypeActionIds)
        {
            try
            {
                m_roleUoW.AssignResourcePermissionTypeActionsToRole(id, permissionTypeActionIds);

                return Success(true);
            }
            catch (NoResultException<RoleEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-role-id"), DataResultErrorCode.RoleNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }
    }
}