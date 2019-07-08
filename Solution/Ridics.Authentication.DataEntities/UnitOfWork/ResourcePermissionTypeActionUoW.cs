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
    public class ResourcePermissionTypeActionUoW : UnitOfWorkBase
    {
        private readonly ResourcePermissionTypeActionRepository m_permissionTypeActionRepository;
        private readonly ResourcePermissionTypeRepository m_permissionTypeRepository;
        private readonly RoleRepository m_roleRepository;
        private readonly UserRepository m_userRepository;

        public ResourcePermissionTypeActionUoW(
            ISessionManager sessionManager,
            ResourcePermissionTypeActionRepository permissionTypeActionRepository,
            ResourcePermissionTypeRepository permissionTypeRepository,
            RoleRepository roleRepository,
            UserRepository userRepository) : base(sessionManager)
        {
            m_permissionTypeActionRepository = permissionTypeActionRepository;
            m_permissionTypeRepository = permissionTypeRepository;
            m_roleRepository = roleRepository;
            m_userRepository = userRepository;
        }

        [Transaction]
        public virtual ResourcePermissionTypeActionEntity FindPermissionTypeActionById(int permissionTypeActionId)
        {
            var permissionType = m_permissionTypeActionRepository.FindPermissionTypeActionById(permissionTypeActionId);

            if (permissionType == null)
            {
                throw new NoResultException<ResourcePermissionTypeActionEntity>();
            }

            return permissionType;
        }

        [Transaction]
        public virtual int CreatePermissionTypeAction(ResourcePermissionTypeActionEntity permissionType)
        {
            var result = (int) m_permissionTypeActionRepository.Create(permissionType);

            return result;
        }


        [Transaction]
        public virtual void UpdatePermissionTypeAction(int id, ResourcePermissionTypeActionEntity permissionType)
        {
            var permissionTypeActionEntity = m_permissionTypeActionRepository.FindPermissionTypeActionById(id);

            if (permissionTypeActionEntity == null)
            {
                throw new NoResultException<ResourcePermissionTypeActionEntity>();
            }

            permissionTypeActionEntity.Name = permissionType.Name;
            permissionTypeActionEntity.Description = permissionType.Description;

            m_permissionTypeActionRepository.Update(permissionTypeActionEntity);
        }

        [Transaction]
        public virtual IList<ResourcePermissionTypeActionEntity> GetPermissionTypeActions(int start, int count, string searchByName)
        {
            var permissionTypes = m_permissionTypeActionRepository.GetPermissionTypeActions(start, count, searchByName);

            return permissionTypes;
        }

        [Transaction]
        public virtual int GetPermissionTypeActionsCount(string searchByName)
        {
            var permissionTypesCount = m_permissionTypeActionRepository.GetPermissionTypeActionsCount(searchByName);

            return permissionTypesCount;
        }

        [Transaction]
        public virtual void DeletePermissionTypeActionById(int permissionTypeActionId)
        {
            var permission = m_permissionTypeActionRepository.Load<ResourcePermissionTypeActionEntity>(permissionTypeActionId);

            try
            {
                m_permissionTypeActionRepository.Delete(permission);
            }
            catch (ObjectNotFoundException)
            {
                throw new NoResultException<ResourcePermissionTypeActionEntity>();
            }
        }

        [Transaction]
        public virtual IList<ResourcePermissionTypeActionEntity> GetAllPermissionTypeActions()
        {
            var permissionTypes = m_permissionTypeActionRepository.GetAllPermissionTypeActions();

            return permissionTypes;
        }

        [Transaction]
        public virtual void AssignRolesToPermissionTypeAction(int permissionTypeActionId, IEnumerable<int> roleIds, bool overwriteAuthOnlyRoles)
        {
            var permissionTypeActionEntity = m_permissionTypeActionRepository.FindPermissionTypeActionById(permissionTypeActionId);

            if (permissionTypeActionEntity == null)
            {
                throw new NoResultException<ResourcePermissionTypeActionEntity>();
            }

            var roles = new HashSet<RoleEntity>(m_roleRepository.GetRolesById(roleIds));

            if (!overwriteAuthOnlyRoles)
            {
                var authOnlyRoles = permissionTypeActionEntity.Roles.Where(x => x.AuthenticationServiceOnly);

                foreach (var authOnlyRole in authOnlyRoles)
                {
                    if (!roles.Contains(authOnlyRole))
                    {
                        roles.Add(authOnlyRole);
                    }
                }
            }

            permissionTypeActionEntity.Roles = roles;

            m_permissionTypeActionRepository.Update(permissionTypeActionEntity);
        }

        [Transaction]
        public virtual void AssignUsersToPermissionTypeAction(int permissionTypeActionId, IEnumerable<int> usersIds)
        {
            var permissionTypeActionEntity = m_permissionTypeActionRepository.FindPermissionTypeActionById(permissionTypeActionId);

            if (permissionTypeActionEntity == null)
            {
                throw new NoResultException<ResourcePermissionTypeActionEntity>();
            }

            var users = new HashSet<UserEntity>(m_userRepository.GetUsersById(usersIds));

            permissionTypeActionEntity.Users = users;

            m_permissionTypeActionRepository.Update(permissionTypeActionEntity);
        }

        [Transaction]
        public virtual IList<ResourcePermissionTypeActionEntity> GetActionsForResourcePermissionTypeById(int id)
        {
            var permissionTypes = m_permissionTypeActionRepository.GetActionsForResourcePermissionTypeById(id);

            return permissionTypes;
        }
    }
}