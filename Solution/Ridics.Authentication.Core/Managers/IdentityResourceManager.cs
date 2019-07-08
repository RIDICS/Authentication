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
    public class IdentityResourceManager : ManagerBase
    {
        private readonly IdentityResourceUoW m_identityResourceUoW;


        public IdentityResourceManager(IdentityResourceUoW identityResourceUoW, ILogger logger, ITranslator translator,
            IMapper mapper,
            IPaginationConfiguration paginationConfiguration) : base(logger, translator, mapper,
            paginationConfiguration)
        {
            m_identityResourceUoW = identityResourceUoW;
        }

        public DataResult<IdentityResourceModel> FindIdentityResourceById(int id)
        {
            try
            {
                var identityResource = m_identityResourceUoW.FindIdentityResourceById(id);
                var viewModel = m_mapper.Map<IdentityResourceModel>(identityResource);
                return Success(viewModel);
            }
            catch (NoResultException<IdentityResourceEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<IdentityResourceModel>(m_translator.Translate("invalid-resource-id"), DataResultErrorCode.ResourceNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<IdentityResourceModel>(e.Message);
            }
        }

        public DataResult<List<IdentityResourceModel>> GetIdentityResources(int start, int count, string searchByName)
        {
            try
            {
                var identityResources = m_identityResourceUoW.GetResources(start, GetItemsOnPageCount(count), searchByName);
                var viewModeList = m_mapper.Map<List<IdentityResourceModel>>(identityResources);
                return Success(viewModeList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<IdentityResourceModel>>(e.Message);
            }
        }

        public DataResult<int> GetIdentityResourcesCount(string searchByName)
        {
            try
            {
                var identityResourcesCount = m_identityResourceUoW.GetResourcesCount(searchByName);
                return Success(identityResourcesCount);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<List<IdentityResourceModel>> GetAllIdentityResources()
        {
            try
            {
                var identityResources = m_identityResourceUoW.GetAllResources();
                var viewModeList = m_mapper.Map<List<IdentityResourceModel>>(identityResources);
                return Success(viewModeList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<IdentityResourceModel>>(e.Message);
            }
        }

        public DataResult<int> CreateIdentityResource(IdentityResourceModel identityResourceModel,
            IEnumerable<int> claimsIds)
        {
            var identityResourceEntity = new IdentityResourceEntity
            {
                Name = identityResourceModel.Name,
                Description = identityResourceModel.Description,
                Required = identityResourceModel.Required,
                ShowInDiscoveryDocument = identityResourceModel.ShowInDiscoveryDocument
            };

            try
            {
                var result = m_identityResourceUoW.CreateIdentityResource(identityResourceEntity, claimsIds);
                return Success(result);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<bool> UpdateIdentityResource(int id, IdentityResourceModel identityResourceModel,
            IEnumerable<int> claimsIds)
        {
            var identityResource = new IdentityResourceEntity
            {
                Name = identityResourceModel.Name,
                Description = identityResourceModel.Description,
                Required = identityResourceModel.Required,
                ShowInDiscoveryDocument = identityResourceModel.ShowInDiscoveryDocument
            };

            try
            {
                m_identityResourceUoW.UpdateIdentityResource(id, identityResource, claimsIds);
                return Success(true);
            }
            catch (NoResultException<IdentityResourceEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-resource-id"), DataResultErrorCode.ResourceNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> DeleteIdentityResourceWithId(int id)
        {
            try
            {
                m_identityResourceUoW.DeleteIdentityResourceById(id);
                return Success(true);
            }
            catch (NoResultException<IdentityResourceEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-resource-id"), DataResultErrorCode.ResourceNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }
    }
}