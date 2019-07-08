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
    public class ClaimTypeUoW : UnitOfWorkBase
    {
        private readonly ClaimTypeRepository m_claimTypeRepository;
        private readonly ClaimTypeEnumRepository m_claimTypeEnumRepository;

        public ClaimTypeUoW(ISessionManager sessionManager, ClaimTypeRepository claimTypeRepository,
            ClaimTypeEnumRepository claimTypeEnumRepository) : base(sessionManager)
        {
            m_claimTypeRepository = claimTypeRepository;
            m_claimTypeEnumRepository = claimTypeEnumRepository;
        }

        [Transaction]
        public virtual ClaimTypeEntity FindClaimTypeById(int id)
        {
            var claimType = m_claimTypeRepository.FindClaimTypeById(id);

            if (claimType == null)
            {
                throw new NoResultException<ClaimTypeEntity>();
            }

            return claimType;
        }

        [Transaction]
        public virtual IList<ClaimTypeEntity> GetClaimTypes(int start, int count, string searchByName)
        {
            var claimTypes = m_claimTypeRepository.GetClaimTypes(start, count, searchByName);

            return claimTypes;
        }

        [Transaction]
        public virtual int GetClaimTypesCount(string searchByName)
        {
            var claimTypesCount = m_claimTypeRepository.GetClaimTypesCount(searchByName);

            return claimTypesCount;
        }

        [Transaction]
        public virtual IList<ClaimTypeEntity> GetAllClaimTypes()
        {
            var claimTypes = m_claimTypeRepository.GetAllClaimTypes();

            return claimTypes;
        }

        [Transaction]
        public virtual int CreateClaimType(ClaimTypeEntity claimType, int claimTypeEnumId)
        {
            var claimTypeEnum = m_claimTypeEnumRepository.FindById<ClaimTypeEnumEntity>(claimTypeEnumId);

            claimType.Type = claimTypeEnum ?? throw new NoResultException<ClaimTypeEnumEntity>();

            var result = (int) m_claimTypeRepository.Create(claimType);

            return result;
        }

        [Transaction]
        public virtual IList<ClaimTypeEnumEntity> GetAllClaimTypeEnums()
        {
            var result = m_claimTypeEnumRepository.GetAllClaimTypesEnum();

            return result;
        }

        [Transaction]
        public virtual void DeleteClaimTypeById(int id)
        {
            var claimType = m_claimTypeRepository.Load<ClaimTypeEntity>(id);

            try
            {
                m_claimTypeRepository.Delete(claimType);
            }
            catch (ObjectNotFoundException)
            {
                throw new NoResultException<ClaimTypeEntity>();
            }
        }

        [Transaction]
        public virtual void UpdateClaimType(int claimTypeId, ClaimTypeEntity claimType, int claimTypeEnumId)
        {
            var claimTypeEntity = m_claimTypeRepository.FindClaimTypeById(claimTypeId);

            if (claimTypeEntity == null)
            {
                throw new NoResultException<ClaimTypeEntity>();
            }

            var type = m_claimTypeEnumRepository.FindById<ClaimTypeEnumEntity>(claimTypeEnumId);

            claimTypeEntity.Type = type ?? throw new NoResultException<ClaimTypeEnumEntity>();
            claimTypeEntity.Name = claimType.Name;
            claimTypeEntity.Description = claimType.Description;

            m_claimTypeRepository.Update(claimTypeEntity);
        }
    }
}