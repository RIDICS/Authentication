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
    public class ScopeManager : ManagerBase
    {
        private readonly ScopeUoW m_scopeUoW;

        public ScopeManager(ScopeUoW scopeUoW, ILogger logger, ITranslator translator, IMapper mapper,
            IPaginationConfiguration paginationConfiguration) : base(logger, translator, mapper, paginationConfiguration)
        {
            m_scopeUoW = scopeUoW;
        }

        public DataResult<List<ScopeModel>> GetScopesForApiResources(int apiResourceId)
        {
            try
            {
                var scopes = m_scopeUoW.GetScopesForApiResource(apiResourceId);
                var viewModelList = m_mapper.Map<List<ScopeModel>>(scopes);
                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<ScopeModel>>(e.Message);
            }
        }

        public DataResult<int> AddScopeToApiResource(int apiResourceId, ScopeModel scope, IEnumerable<int> claimsIds)
        {
            var newScope = new ScopeEntity
            {
                Name = scope.Name,
                Description = scope.Description,
                Required = scope.Required,
                ShowInDiscoveryDocument = scope.ShowInDiscoveryDocument
            };

            try
            {
                var result = m_scopeUoW.AddScopeToApiResource(apiResourceId, newScope, claimsIds);
                return Success(result);
            }
            catch (NoResultException<ApiResourceEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<int>(m_translator.Translate("invalid-api-resource-id"), DataResultErrorCode.ApiResourceNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<bool> DeleteScope(int scopeId)
        {
            try
            {
                m_scopeUoW.DeleteScope(scopeId);
                return Success(true);
            }
            catch (NoResultException<ScopeEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-scope-id"));
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<List<ScopeModel>> GetAllScopes()
        {
            try
            {
                var scopes = m_scopeUoW.GetAllScopes();
                var viewModelList = m_mapper.Map<List<ScopeModel>>(scopes);
                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<ScopeModel>>(e.Message);
            }
        }
    }
}