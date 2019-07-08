using System.Collections.Generic;
using DryIoc.Facilities.NHibernate;
using DryIoc.Transactions;
using NHibernate;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Exceptions;
using Ridics.Authentication.DataEntities.Repositories;
using Ridics.Core.DataEntities.Shared.UnitOfWorks;

namespace Ridics.Authentication.DataEntities.UnitOfWork
{
    public class RoleUoW : UnitOfWorkBase
    {
        private readonly RoleRepository m_roleRepository;
        private readonly PermissionRepository m_permissionRepository;
        private readonly ResourcePermissionRepository m_resourcePermissionRepository;
        private readonly ResourcePermissionTypeActionRepository m_resourcePermissionTypeActionRepository;

        public RoleUoW(ISessionManager sessionManager, RoleRepository roleRepository,
            PermissionRepository permissionRepository, ResourcePermissionRepository resourcePermissionRepository,
            ResourcePermissionTypeActionRepository resourcePermissionTypeActionRepository) : base(sessionManager)
        {
            m_roleRepository = roleRepository;
            m_permissionRepository = permissionRepository;
            m_resourcePermissionRepository = resourcePermissionRepository;
            m_resourcePermissionTypeActionRepository = resourcePermissionTypeActionRepository;
        }

        [Transaction]
        public virtual IList<RoleEntity> GetAllRoles()
        {
            var roles = m_roleRepository.GetAllRoles();

            return roles;
        }

        [Transaction]
        public virtual IList<RoleEntity> GetAllNonAuthenticationServiceRoles()
        {
            var roles = m_roleRepository.GetAllNonAuthenticationServiceRoles();

            return roles;
        }

        [Transaction]
        public virtual IList<RoleEntity> GetRolesByUser(int userId)
        {
            var roles = m_roleRepository.GetRolesByUser(userId);

            return roles;
        }

        [Transaction]
        public virtual void DeleteRoleById(int id)
        {
            var role = m_roleRepository.Load<RoleEntity>(id);

            try
            {
                m_roleRepository.Delete(role);
            }
            catch (ObjectNotFoundException)
            {
                throw new NoResultException<RoleEntity>();
            }
        }

        [Transaction]
        public virtual int CreateRole(RoleEntity role)
        {
            var result = (int) m_roleRepository.Create(role);

            return result;
        }

        [Transaction]
        public virtual RoleEntity FindRoleById(int id)
        {
            var role = m_roleRepository.FindRoleById(id);

            if (role == null)
            {
                throw new NoResultException<RoleEntity>();
            }

            return role;
        }

        [Transaction]
        public virtual void UpdateRole(int id, RoleEntity role)
        {
            var roleEntity = m_roleRepository.FindRoleById(id);

            if (roleEntity == null)
            {
                throw new NoResultException<RoleEntity>();
            }

            roleEntity.Name = role.Name;
            roleEntity.Description = role.Description;

            m_roleRepository.Update(roleEntity);
        }

        [Transaction]
        public virtual IList<RoleEntity> GetRoles(int start, int count, string searchByName)
        {
            var roles = m_roleRepository.GetRoles(start, count, searchByName);

            return roles;
        }

        [Transaction]
        public virtual int GetRolesCount(string searchByName)
        {
            var rolesCount = m_roleRepository.GetRolesCount(searchByName);

            return rolesCount;
        }

        [Transaction]
        public virtual IList<RoleEntity> GetNonAuthenticationServiceRoles(int start, int count, string searchByName)
        {
            var roles = m_roleRepository.GetNonAuthenticationServiceRoles(start, count, searchByName);

            return roles;
        }

        [Transaction]
        public virtual int GetNonAuthenticationServiceRolesCount(string searchByName)
        {
            var rolesCount = m_roleRepository.GetNonAuthenticationServiceRolesCount(searchByName);

            return rolesCount;
        }

        [Transaction]
        public virtual void AssignPermissionsToRole(int id, IEnumerable<int> permissionIds)
        {
            var roleEntity = m_roleRepository.FindById<RoleEntity>(id);

            if (roleEntity == null)
            {
                throw new NoResultException<RoleEntity>();
            }

            roleEntity.Permissions =
                new HashSet<PermissionEntity>(m_permissionRepository.GetPermissionsById(permissionIds));

            m_roleRepository.Update(roleEntity);
        }

        [Transaction]
        public virtual void AssignResourcePermissionsToRole(int id, IEnumerable<int> permissionIds)
        {
            var roleEntity = m_roleRepository.FindById<RoleEntity>(id);

            if (roleEntity == null)
            {
                throw new NoResultException<RoleEntity>();
            }

            roleEntity.ResourcePermissions =
                new HashSet<ResourcePermissionEntity>(m_resourcePermissionRepository.GetPermissionsById(permissionIds));

            m_roleRepository.Update(roleEntity);
        }

        [Transaction]
        public virtual void AssignResourcePermissionTypeActionsToRole(int id, IEnumerable<int> permissionTypeActionIds)
        {
            var roleEntity = m_roleRepository.FindById<RoleEntity>(id);

            if (roleEntity == null)
            {
                throw new NoResultException<RoleEntity>();
            }
            
            roleEntity.ResourcePermissionTypeActions =
                new HashSet<ResourcePermissionTypeActionEntity>(m_resourcePermissionTypeActionRepository.GetPermissionTypeActionsById(permissionTypeActionIds));

            m_roleRepository.Update(roleEntity);
        }
    }
}