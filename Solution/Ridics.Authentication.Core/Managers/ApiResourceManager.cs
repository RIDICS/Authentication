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
    public class ApiResourceManager : ManagerBase
    {
        private readonly ApiResourceUoW m_apiResourceUoW;

        public ApiResourceManager(ApiResourceUoW apiResourceUoW, ILogger logger, ITranslator translator, IMapper mapper,
            IPaginationConfiguration paginationConfiguration) : base(logger, translator, mapper,
            paginationConfiguration)
        {
            m_apiResourceUoW = apiResourceUoW;
        }

        public DataResult<ApiResourceModel> FindApiResourceById(int id)
        {
            try
            {
                var apiResource = m_apiResourceUoW.FindApiResourceById(id);
                var viewModel = m_mapper.Map<ApiResourceModel>(apiResource);
                return Success(viewModel);
            }
            catch (NoResultException<ApiResourceEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<ApiResourceModel>(m_translator.Translate("invalid-api-resource-id"),
                    DataResultErrorCode.ApiResourceNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<ApiResourceModel>(e.Message);
            }
        }

        public DataResult<ApiResourceModel> FindApiResourceByName(string name)
        {
            try
            {
                var apiResource = m_apiResourceUoW.FindApiResourceByName(name);
                var viewModel = m_mapper.Map<ApiResourceModel>(apiResource);
                return Success(viewModel);
            }
            catch (NoResultException<ApiResourceEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<ApiResourceModel>(m_translator.Translate("invalid-api-resource-name"),
                    DataResultErrorCode.ApiResourceNotExistName);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<ApiResourceModel>(e.Message);
            }
        }

        public DataResult<List<ApiResourceModel>> GetApiResources(int start, int count, string searchByName)
        {
            try
            {
                var apiResources = m_apiResourceUoW.GetResources(start, GetItemsOnPageCount(count), searchByName);
                var viewModelList = m_mapper.Map<List<ApiResourceModel>>(apiResources);
                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<ApiResourceModel>>(e.Message);
            }
        }

        public DataResult<int> GetApiResourcesCount(string searchByName)
        {
            try
            {
                var apiResourcesCount = m_apiResourceUoW.GetResourcesCount(searchByName);
                return Success(apiResourcesCount);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<List<ApiResourceModel>> GetAllApiResources()
        {
            try
            {
                var apiResources = m_apiResourceUoW.GetAllResources();
                var viewModelList = m_mapper.Map<List<ApiResourceModel>>(apiResources);
                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<ApiResourceModel>>(e.Message);
            }
        }

        public DataResult<int> CreateApiResource(ApiResourceModel apiResourceModel, IEnumerable<int> claimsIds)
        {
            var defaultScope = new ScopeEntity
            {
                Name = apiResourceModel.Name,
                Description = apiResourceModel.Description,
                Required = apiResourceModel.Required,
                ShowInDiscoveryDocument = apiResourceModel.ShowInDiscoveryDocument
            };

            var apiResourceEntity = new ApiResourceEntity
            {
                Name = apiResourceModel.Name,
                Description = apiResourceModel.Description,
                Required = apiResourceModel.Required,
                ShowInDiscoveryDocument = apiResourceModel.ShowInDiscoveryDocument,
            };

            try
            {
                var result = m_apiResourceUoW.CreateApiResource(apiResourceEntity, claimsIds, defaultScope);
                return Success(result);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<bool> UpdateApiResource(int id, ApiResourceModel apiResourceModel, IEnumerable<int> claimsIds)
        {
            var apiResource = new ApiResourceEntity
            {
                Name = apiResourceModel.Name,
                Description = apiResourceModel.Description,
                Required = apiResourceModel.Required,
                ShowInDiscoveryDocument = apiResourceModel.ShowInDiscoveryDocument
            };

            try
            {
                m_apiResourceUoW.UpdateApiResource(id, apiResource, claimsIds);
                return Success(true);
            }
            catch (NoResultException<ApiResourceEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-api-resource-id"), DataResultErrorCode.ApiResourceNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> DeleteApiResourceWithId(int id)
        {
            try
            {
                m_apiResourceUoW.DeleteApiResource(id);
                return Success(true);
            }
            catch (NoResultException<ApiResourceEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-api-resource-id"), DataResultErrorCode.ApiResourceNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }
    }
}