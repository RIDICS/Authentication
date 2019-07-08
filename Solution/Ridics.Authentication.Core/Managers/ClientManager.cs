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
    public class ClientManager : ManagerBase
    {
        private readonly ClientUoW m_clientUoW;

        public ClientManager(ClientUoW clientUoW, ILogger logger, ITranslator translator, IMapper mapper,
            IPaginationConfiguration paginationConfiguration) : base(logger, translator, mapper,
            paginationConfiguration)
        {
            m_clientUoW = clientUoW;
        }

        public DataResult<ClientModel> FindClientByClientId(string clientId)
        {
            try
            {
                var client = m_clientUoW.FindClientByClientId(clientId);
                var viewModel = m_mapper.Map<ClientModel>(client);
                return Success(viewModel);
            }
            catch (NoResultException<ClientEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<ClientModel>(m_translator.Translate("invalid-client-id"), DataResultErrorCode.ClientNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<ClientModel>(e.Message);
            }
        }

        public DataResult<ClientModel> FindClientById(int id)
        {
            try
            {
                var client = m_clientUoW.FindClientById(id);
                var viewModel = m_mapper.Map<ClientModel>(client);
                return Success(viewModel);
            }
            catch (NoResultException<ClientEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<ClientModel>(m_translator.Translate("invalid-client-id"), DataResultErrorCode.ClientNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<ClientModel>(e.Message);
            }
        }

        public DataResult<List<ClientModel>> GetClients(int start, int count, string searchByName)
        {
            try
            {
                var clients = m_clientUoW.GetClients(start, GetItemsOnPageCount(count), searchByName);
                var viewModelList = m_mapper.Map<List<ClientModel>>(clients);
                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<ClientModel>>(e.Message);
            }
        }

        public DataResult<int> GetClientsCount(string searchByName)
        {
            try
            {
                var clientsCount = m_clientUoW.GetClientsCount(searchByName);
                return Success(clientsCount);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }

        }

        public DataResult<int> CreateClient(ClientModel client, IEnumerable<int> identityResourcesIds, IEnumerable<int> grantTypesIds, IEnumerable<int> scopesIds)
        {
            var newClient = new ClientEntity
            {
                Name = client.Name,
                Description = client.Description,
                DisplayUrl = client.DisplayUrl,
                LogoUrl = client.LogoUrl,
                RequireConsent = client.RequireConsent,
                AllowAccessTokensViaBrowser = client.AllowAccessTokensViaBrowser,
            };

            try
            {
                var result = m_clientUoW.CreateClient(newClient, identityResourcesIds, grantTypesIds, scopesIds);
                return Success(result);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<bool> UpdateClient(int id, ClientModel client, IEnumerable<int> identityResourcesIds, IEnumerable<int> grantTypesIds, IEnumerable<int> scopeIds)
        {
            var newClient = new ClientEntity
            {
                Name = client.Name,
                Description = client.Description,
                DisplayUrl = client.DisplayUrl,
                LogoUrl = client.LogoUrl,
                RequireConsent = client.RequireConsent,
                AllowAccessTokensViaBrowser = client.AllowAccessTokensViaBrowser,
            };

            try
            {
                m_clientUoW.UpdateClient(id, newClient, identityResourcesIds, grantTypesIds, scopeIds);
                return Success(true);
            }
            catch (NoResultException<ClientEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-client-id"), DataResultErrorCode.ClientNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> DeleteClientWithId(int id)
        {
            try
            {
                m_clientUoW.DeleteClientById(id);
                return Success(true);
            }
            catch (NoResultException<ClientEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-client-id"), DataResultErrorCode.ClientNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }
    }
}