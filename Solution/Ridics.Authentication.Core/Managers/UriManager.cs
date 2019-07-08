using System.Collections.Generic;
using System.Linq;
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
    public class UriManager : ManagerBase
    {
        private readonly UriUoW m_uriUoW;
        private readonly UriTypeUoW m_uriTypeUoW;

        public UriManager(UriUoW uriUoW, ILogger logger, ITranslator translator, IMapper mapper,
            IPaginationConfiguration paginationConfiguration, UriTypeUoW uriTypeUoW) : base(logger, translator, mapper, paginationConfiguration)
        {
            m_uriUoW = uriUoW;
            m_uriTypeUoW = uriTypeUoW;
        }

        public DataResult<List<UriModel>> FindUrisForClient(int clientId)
        {
            try
            {
                var uris = m_uriUoW.FindUrisForClient(clientId);
                var viewModelList = m_mapper.Map<List<UriModel>>(uris);
                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<UriModel>>(e.Message);
            }
        }

        public DataResult<List<UriModel>> GetAllUris()
        {
            try
            {
                var uris = m_uriUoW.GetAllUris();
                var viewModelList = m_mapper.Map<List<UriModel>>(uris);
                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<UriModel>>(e.Message);
            }
        }

        public DataResult<int> CreateUriForClient(int clientId, UriModel uri)
        {
            var uriTypes = m_uriTypeUoW.GetAllUriTypes();

            var newUri = new UriEntity
            {
                Uri = uri.Value,
                UriTypes = new HashSet<UriTypeEntity>(uri.UriTypes.Select(x => uriTypes.FirstOrDefault(y => y.Value == x.UriTypeValue))),
            };

            try
            {
                var result = m_uriUoW.CreateUriForClient(clientId, newUri);
                return Success(result);
            }
            catch (NoResultException<ClientEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<int>(m_translator.Translate("invalid-client-id"), DataResultErrorCode.ClientNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<bool> DeleteUriForClient(int uriId)
        {
            try
            {
                m_uriUoW.DeleteUriForClient(uriId);
                return Success(true);
            }
            catch (NoResultException<UriEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-uri-id"), DataResultErrorCode.UriNoxExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }
    }
}