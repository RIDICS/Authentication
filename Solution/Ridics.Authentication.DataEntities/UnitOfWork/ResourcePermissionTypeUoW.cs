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
    public class ResourcePermissionTypeUoW : UnitOfWorkBase
    {
        private readonly ResourcePermissionTypeRepository m_permissionTypeRepository;
        private readonly ResourcePermissionTypeActionRepository m_permissionTypeActionRepository;

        public ResourcePermissionTypeUoW(
            ISessionManager sessionManager,
            ResourcePermissionTypeRepository permissionTypeRepository,
            ResourcePermissionTypeActionRepository permissionTypeActionRepository) :
            base(sessionManager)
        {
            m_permissionTypeRepository = permissionTypeRepository;
            m_permissionTypeActionRepository = permissionTypeActionRepository;
        }

        [Transaction]
        public virtual ResourcePermissionTypeEntity FindPermissionTypeById(int permissionTypeId)
        {
            var permissionType = m_permissionTypeRepository.FindPermissionTypeById(permissionTypeId);

            if (permissionType == null)
            {
                throw new NoResultException<ResourcePermissionTypeEntity>();
            }

            return permissionType;
        }

        [Transaction]
        public virtual int CreatePermissionType(ResourcePermissionTypeEntity permissionType)
        {
            var result = (int) m_permissionTypeRepository.Create(permissionType);

            return result;
        }


        [Transaction]
        public virtual void UpdatePermissionType(int id, ResourcePermissionTypeEntity permissionType)
        {
            var permissionTypeEntity = m_permissionTypeRepository.FindPermissionTypeById(id);

            if (permissionTypeEntity == null)
            {
                throw new NoResultException<ResourcePermissionTypeEntity>();
            }

            permissionTypeEntity.Name = permissionType.Name;
            permissionTypeEntity.Description = permissionType.Description;

            m_permissionTypeRepository.Update(permissionTypeEntity);
        }

        [Transaction]
        public virtual IList<ResourcePermissionTypeEntity> GetPermissionTypes(int start, int count, string searchByName)
        {
            var permissionTypes = m_permissionTypeRepository.GetPermissionTypes(start, count, searchByName);

            return permissionTypes;
        }

        [Transaction]
        public virtual int GetPermissionTypesCount(string searchByName)
        {
            var permissionTypesCount = m_permissionTypeRepository.GetPermissionTypesCount(searchByName);

            return permissionTypesCount;
        }

        [Transaction]
        public virtual void DeletePermissionTypeById(int permissionTypeId)
        {
            var permission = m_permissionTypeRepository.Load<ResourcePermissionTypeEntity>(permissionTypeId);

            try
            {
                m_permissionTypeRepository.Delete(permission);
            }
            catch (ObjectNotFoundException)
            {
                throw new NoResultException<ResourcePermissionTypeEntity>();
            }
        }

        [Transaction]
        public virtual IList<ResourcePermissionTypeEntity> GetAllPermissionTypes()
        {
            var permissionTypes = m_permissionTypeRepository.GetAllPermissionTypes();

            return permissionTypes;
        }

        [Transaction]
        public virtual void AssignActionsToPermissionTypeAction(int permissionTypeId, IEnumerable<int> selectedActions)
        {
            {
                var permissionTypeEntity = m_permissionTypeRepository.FindPermissionTypeById(permissionTypeId);

                if (permissionTypeEntity == null)
                {
                    throw new NoResultException<ResourcePermissionTypeEntity>();
                }

                var actions = new HashSet<ResourcePermissionTypeActionEntity>(
                    m_permissionTypeActionRepository.GetPermissionTypeActionsById(selectedActions));

                permissionTypeEntity.ResourcePermissionTypeActions = actions;

                m_permissionTypeRepository.Update(permissionTypeEntity);
            }
        }
    }
}