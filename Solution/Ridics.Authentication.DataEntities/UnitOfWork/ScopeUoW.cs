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
    public class ScopeUoW : UnitOfWorkBase
    {
        private readonly ScopeRepository m_scopeRepository;
        private readonly ApiResourceRepository m_apiResourceRepository;
        private readonly ClaimTypeRepository m_claimTypeRepository;

        public ScopeUoW(ISessionManager sessionManager, ScopeRepository scopeRepository,
            ApiResourceRepository apiResourceRepository, ClaimTypeRepository claimTypeRepository) : base(sessionManager)
        {
            m_scopeRepository = scopeRepository;
            m_apiResourceRepository = apiResourceRepository;
            m_claimTypeRepository = claimTypeRepository;
        }

        [Transaction]
        public virtual IEnumerable<ScopeEntity> GetScopesForApiResource(int apiResourceId)
        {
            var scopes = m_scopeRepository.GetScopesForApiResource(apiResourceId);

            return scopes;
        }

        [Transaction]
        public virtual int AddScopeToApiResource(int apiResourceId, ScopeEntity newScope, IEnumerable<int> claimsIds)
        {
            var apiResource = m_apiResourceRepository.FindById<ApiResourceEntity>(apiResourceId);

            newScope.ApiResource = apiResource ?? throw new NoResultException<ApiResourceEntity>();

            newScope.ClaimTypes = new HashSet<ClaimTypeEntity>(m_claimTypeRepository.GetClaimTypesById(claimsIds));

            var result = (int) m_scopeRepository.Create(newScope);

            return result;
        }

        [Transaction]
        public virtual void DeleteScope(int scopeId)
        {
            var scope = m_scopeRepository.Load<ScopeEntity>(scopeId);

            try
            {
                m_scopeRepository.Delete(scope);
            }
            catch (ObjectNotFoundException)
            {
                throw new NoResultException<ScopeEntity>();
            }
        }

        [Transaction]
        public virtual IList<ScopeEntity> GetAllScopes()
        {
            var scopes = m_scopeRepository.GetAllScopes();

            return scopes;
        }
    }
}