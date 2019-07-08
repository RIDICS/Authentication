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
    public class ApiResourceUoW : UnitOfWorkBase
    {
        private readonly ApiResourceRepository m_apiResourceRepository;
        private readonly ClaimTypeRepository m_claimTypeRepository;
        private readonly ScopeRepository m_scopeRepository;

        public ApiResourceUoW(ISessionManager sessionManager, ApiResourceRepository apiResourceRepository,
            ClaimTypeRepository claimTypeRepository, ScopeRepository scopeRepository) : base(sessionManager)
        {
            m_apiResourceRepository = apiResourceRepository;
            m_claimTypeRepository = claimTypeRepository;
            m_scopeRepository = scopeRepository;
        }

        [Transaction]
        public virtual ApiResourceEntity FindApiResourceById(int id)
        {
            var apiResource = m_apiResourceRepository.FindApiResourceById(id);

            if (apiResource == null)
            {
                throw new NoResultException<ApiResourceEntity>();
            }

            return apiResource;
        }

        [Transaction]
        public virtual ApiResourceEntity FindApiResourceByName(string name)
        {
            var apiResource = m_apiResourceRepository.FindByName(name);

            if (apiResource == null)
            {
                throw new NoResultException<ApiResourceEntity>();
            }

            return apiResource;
        }

        [Transaction]
        public virtual void DeleteApiResource(int id)
        {
            var apiResource = m_apiResourceRepository.Load<ApiResourceEntity>(id);

            try
            {
                m_apiResourceRepository.Delete(apiResource);
            }
            catch (ObjectNotFoundException)
            {
                throw new NoResultException<ApiResourceEntity>();
            }
        }

        [Transaction]
        public virtual IList<ApiResourceEntity> GetResources(int start, int count, string searchByName)
        {
            var resources = m_apiResourceRepository.GetResources(start, count, searchByName);

            return resources;
        }

        [Transaction]
        public virtual int GetResourcesCount(string searchByName)
        {
            var resourcesCount = m_apiResourceRepository.GetResourcesCount(searchByName);

            return resourcesCount;
        }

        [Transaction]
        public virtual IList<ApiResourceEntity> GetAllResources()
        {
            var resources = m_apiResourceRepository.GetAllResources();

            return resources;
        }

        [Transaction]
        public virtual int CreateApiResource(ApiResourceEntity apiResourceEntity, IEnumerable<int> claimsIds, ScopeEntity defaultScope)
        {
            var claimTypes = m_claimTypeRepository.GetClaimTypesById(claimsIds);
            apiResourceEntity.ClaimTypes = new HashSet<ClaimTypeEntity>(claimTypes);

            var result = (int) m_apiResourceRepository.Create(apiResourceEntity);

            var apiResource = m_apiResourceRepository.Load<ApiResourceEntity>(result);

            defaultScope.ApiResource = apiResource;
            defaultScope.ClaimTypes = new HashSet<ClaimTypeEntity>(claimTypes);

            var scopeResult = (int)m_scopeRepository.Create(defaultScope);

            return result;
        }

        [Transaction]
        public virtual void UpdateApiResource(int id, ApiResourceEntity apiResource, IEnumerable<int> claimsIds)
        {
            var apiResourceEntity = m_apiResourceRepository.FindApiResourceById(id);

            if (apiResourceEntity == null)
            {
                throw new NoResultException<ApiResourceEntity>();
            }

            apiResourceEntity.Name = apiResource.Name;
            apiResourceEntity.Description = apiResource.Description;
            apiResourceEntity.Required = apiResource.Required;
            apiResourceEntity.ShowInDiscoveryDocument = apiResource.ShowInDiscoveryDocument;

            apiResourceEntity.ClaimTypes =
                new HashSet<ClaimTypeEntity>(m_claimTypeRepository.GetClaimTypesById(claimsIds));
            m_apiResourceRepository.Update(apiResourceEntity);
        }
    }
}