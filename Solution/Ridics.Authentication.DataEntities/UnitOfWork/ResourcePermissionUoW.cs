using System.Collections.Generic;
using System.Linq;
using DryIoc.Facilities.NHibernate;
using DryIoc.Transactions;
using NHibernate;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Exceptions;
using Ridics.Authentication.DataEntities.Repositories;
using Ridics.Core.DataEntities.Shared.UnitOfWorks;

namespace Ridics.Authentication.DataEntities.UnitOfWork
{
    public class ResourcePermissionUoW : UnitOfWorkBase
    {
        private readonly ResourcePermissionRepository m_permissionRepository;
        private readonly RoleRepository m_roleRepository;
        private readonly UserRepository m_userRepository;

        public ResourcePermissionUoW(ISessionManager sessionManager, ResourcePermissionRepository permissionRepository,
            RoleRepository roleRepository, UserRepository userRepository) : base(sessionManager)
        {
            m_permissionRepository = permissionRepository;
            m_roleRepository = roleRepository;
            m_userRepository = userRepository;
        }

        [Transaction]
        public virtual ResourcePermissionEntity FindPermissionById(int permissionId)
        {
            var permission = m_permissionRepository.FindPermissionById(permissionId);

            if (permission == null)
            {
                throw new NoResultException<ResourcePermissionEntity>();
            }

            return permission;
        }

        [Transaction]
        public virtual ResourcePermissionTypeActionEntity LoadPermissionTypeActionById(int permissionTypeActionId)
        {
            var permissionTypeAction = m_permissionRepository.FindById<ResourcePermissionTypeActionEntity>(permissionTypeActionId);

            if (permissionTypeAction == null)
            {
                throw new NoResultException<ResourcePermissionTypeActionEntity>();
            }

            return permissionTypeAction;
        }

        [Transaction]
        public virtual ResourcePermissionTypeEntity FindPermissionTypeById(int permissionTypeId)
        {
            var permissionType = m_permissionRepository.FindPermissionTypeById(permissionTypeId);

            if (permissionType == null)
            {
                throw new NoResultException<ResourcePermissionTypeEntity>();
            }

            return permissionType;
        }

        [Transaction]
        public virtual int CreatePermission(ResourcePermissionEntity permission)
        {
            var result = (int) m_permissionRepository.Create(permission);

            return result;
        }


        [Transaction]
        public virtual void UpdatePermission(int id, ResourcePermissionEntity permission)
        {
            var permissionEntity = m_permissionRepository.FindPermissionById(id);

            if (permissionEntity == null)
            {
                throw new NoResultException<ResourcePermissionEntity>();
            }

            permissionEntity.ResourceId = permission.ResourceId;
            permissionEntity.ResourceTypeAction = permission.ResourceTypeAction;

            m_permissionRepository.Update(permissionEntity);
        }

        [Transaction]
        public virtual IList<ResourcePermissionEntity> GetPermissions(int start, int count)
        {
            var permissions = m_permissionRepository.GetPermissions(start, count);

            return permissions;
        }

        [Transaction]
        public virtual int GetPermissionsCount()
        {
            var permissionsCount = m_permissionRepository.GetPermissionsCount();

            return permissionsCount;
        }

        [Transaction]
        public virtual void DeletePermissionById(int permissionId)
        {
            var permission = m_permissionRepository.Load<ResourcePermissionEntity>(permissionId);

            try
            {
                m_permissionRepository.Delete(permission);
            }
            catch (ObjectNotFoundException)
            {
                throw new NoResultException<ResourcePermissionEntity>();
            }
        }

        [Transaction]
        public virtual IList<ResourcePermissionEntity> GetAllPermissions()
        {
            var permissions = m_permissionRepository.GetAllPermissions();

            return permissions;
        }

        [Transaction]
        public virtual void AssignRolesToPermission(int permissionId, IEnumerable<int> roleIds, bool overwriteAuthOnlyRoles)
        {
            var permissionEntity = m_permissionRepository.FindPermissionById(permissionId);

            if (permissionEntity == null)
            {
                throw new NoResultException<ResourcePermissionEntity>();
            }

            var roles = new HashSet<RoleEntity>(m_roleRepository.GetRolesById(roleIds, false));

            if (!overwriteAuthOnlyRoles)
            {
                var authOnlyRoles = permissionEntity.Roles.Where(x => x.AuthenticationServiceOnly);

                foreach (var authOnlyRole in authOnlyRoles)
                {
                    if (!roles.Contains(authOnlyRole))
                    {
                        roles.Add(authOnlyRole);
                    }
                }
            }

            permissionEntity.Roles = roles;

            m_permissionRepository.Update(permissionEntity);
        }

        [Transaction]
        public virtual void AssignUsersToPermission(int permissionId, IEnumerable<int> usersIds)
        {
            var permissionEntity = m_permissionRepository.FindPermissionById(permissionId);

            if (permissionEntity == null)
            {
                throw new NoResultException<ResourcePermissionEntity>();
            }

            var users = new HashSet<UserEntity>(m_userRepository.GetUsersById(usersIds));

            permissionEntity.Users = users;

            m_permissionRepository.Update(permissionEntity);
        }
    }
}