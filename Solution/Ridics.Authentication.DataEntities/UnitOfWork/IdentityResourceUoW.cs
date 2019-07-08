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
    public class IdentityResourceUoW : UnitOfWorkBase
    {
        private readonly IdentityResourceRepository m_identityResourceRepository;
        private readonly ClaimTypeRepository m_claimTypeRepository;

        public IdentityResourceUoW(ISessionManager sessionManager,
            IdentityResourceRepository identityResourceRepository,
            ClaimTypeRepository claimTypeRepository) : base(sessionManager)
        {
            m_identityResourceRepository = identityResourceRepository;
            m_claimTypeRepository = claimTypeRepository;
        }

        [Transaction]
        public virtual IdentityResourceEntity FindIdentityResourceById(int id)
        {
            var result = m_identityResourceRepository.FindIdentityResourceById(id);

            if (result == null)
            {
                throw new NoResultException<IdentityResourceEntity>();
            }

            return result;
        }

        [Transaction]
        public virtual IList<IdentityResourceEntity> GetResources(int start, int count, string searchByName)
        {
            var identityResources = m_identityResourceRepository.GetResources(start, count, searchByName);

            return identityResources;
        }

        [Transaction]
        public virtual int GetResourcesCount(string searchByName)
        {
            var identityResourcesCount = m_identityResourceRepository.GetResourcesCount(searchByName);

            return identityResourcesCount;
        }

        [Transaction]
        public virtual IList<IdentityResourceEntity> GetAllResources()
        {
            var identityResources = m_identityResourceRepository.GetAllResources();

            return identityResources;
        }

        [Transaction]
        public virtual int CreateIdentityResource(IdentityResourceEntity identityResourceEntity,
            IEnumerable<int> claimsIds)
        {
            identityResourceEntity.ClaimTypes =
                new HashSet<ClaimTypeEntity>(m_claimTypeRepository.GetClaimTypesById(claimsIds));
            var result = (int) m_identityResourceRepository.Create(identityResourceEntity);

            return result;
        }

        [Transaction]
        public virtual void UpdateIdentityResource(int id, IdentityResourceEntity identityResource,
            IEnumerable<int> claimsIds)
        {
            var identityResourceEntity = m_identityResourceRepository.FindIdentityResourceById(id);

            if (identityResourceEntity == null)
            {
                throw new NoResultException<IdentityResourceEntity>();
            }

            identityResourceEntity.Name = identityResource.Name;
            identityResourceEntity.Description = identityResource.Description;
            identityResourceEntity.Required = identityResource.Required;
            identityResourceEntity.ShowInDiscoveryDocument = identityResource.ShowInDiscoveryDocument;
            identityResourceEntity.ClaimTypes =
                new HashSet<ClaimTypeEntity>(m_claimTypeRepository.GetClaimTypesById(claimsIds));

            m_identityResourceRepository.Update(identityResourceEntity);
        }

        [Transaction]
        public virtual void DeleteIdentityResourceById(int id)
        {
            var identityResource = m_identityResourceRepository.Load<IdentityResourceEntity>(id);

            try
            {
                m_identityResourceRepository.Delete(identityResource);
            }
            catch (ObjectNotFoundException)
            {
                throw new NoResultException<IdentityResourceEntity>();
            }
        }
    }
}