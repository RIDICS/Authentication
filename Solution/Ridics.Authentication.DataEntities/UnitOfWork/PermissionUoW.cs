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
    public class PermissionUoW : UnitOfWorkBase
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly RoleRepository m_roleRepository;

        public PermissionUoW(ISessionManager sessionManager, PermissionRepository permissionRepository,
            RoleRepository roleRepository) : base(sessionManager)
        {
            m_permissionRepository = permissionRepository;
            m_roleRepository = roleRepository;
        }

        [Transaction]
        public virtual PermissionEntity FindPermissionById(int id)
        {
            var permission = m_permissionRepository.FindPermissionById(id);

            if (permission == null)
            {
                throw new NoResultException<PermissionEntity>();
            }

            return permission;
        }

        [Transaction]
        public virtual PermissionEntity GetPermissionByUserByName(int userId, string permissionName)
        {
            var permission = m_permissionRepository.GetPermissionByUserByName(userId, permissionName);
            
            return permission;
        }

        [Transaction]
        public virtual int CreatePermission(PermissionEntity permission)
        {
            var result = (int) m_permissionRepository.Create(permission);

            return result;
        }


        [Transaction]
        public virtual void UpdatePermission(int id, PermissionEntity permission)
        {
            var permissionEntity = m_permissionRepository.FindPermissionById(id);

            if (permissionEntity == null)
            {
                throw new NoResultException<PermissionEntity>();
            }

            permissionEntity.Name = permission.Name;
            permissionEntity.Description = permission.Description;

            m_permissionRepository.Update(permissionEntity);
        }

        [Transaction]
        public virtual IList<PermissionEntity> GetPermissions(int start, int count, string searchByName)
        {
            var permissions = m_permissionRepository.GetPermissions(start, count, searchByName);

            return permissions;
        }

        [Transaction]
        public virtual int GetPermissionsCount(string searchByName)
        {
            var permissionsCount = m_permissionRepository.GetPermissionsCount(searchByName);

            return permissionsCount;
        }

        [Transaction]
        public virtual void DeletePermissionById(int id)
        {
            var permission = m_permissionRepository.Load<PermissionEntity>(id);

            try
            {
                m_permissionRepository.Delete(permission);
            }
            catch (ObjectNotFoundException)
            {
                throw new NoResultException<PermissionEntity>();
            }
        }

        [Transaction]
        public virtual IList<PermissionEntity> GetAllPermissions(string search = null)
        {
            var permissions = m_permissionRepository.GetAllPermissions(search);

            return permissions;
        }

        [Transaction]
        public virtual void AssignRolesToPermission(int id, IEnumerable<int> roleIds, bool overwriteAuthOnlyRoles)
        {
            var permissionEntity = m_permissionRepository.FindPermissionById(id);

            if (permissionEntity == null)
            {
                throw new NoResultException<PermissionEntity>();
            }

            var roles = new HashSet<RoleEntity>(m_roleRepository.GetRolesById(roleIds));

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
        public virtual void EnsurePermissionsExist(IEnumerable<PermissionEntity> permissions, string newAssignToRoleName)
        {
            var roleEntity = !string.IsNullOrEmpty(newAssignToRoleName) ? m_roleRepository.GetRoleByName(newAssignToRoleName) : null;

            foreach (var permission in permissions)
            {
                var permissionEntity = m_permissionRepository.GetPermissionByName(permission.Name);
                if (permissionEntity == null)
                {
                    if (roleEntity != null)
                    {
                        permission.Roles = new HashSet<RoleEntity>
                        {
                            roleEntity
                        };
                    }
                    
                    m_permissionRepository.Create(permission);
                }
            }
        }
    }
}