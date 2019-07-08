using System.Collections.Generic;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Configuration;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.DataResult;
using Ridics.Authentication.DataEntities.UnitOfWork;
using Ridics.Authentication.Shared;
using Ridics.Core.Shared;
using Ridics.Core.Shared.Exceptions;

namespace Ridics.Authentication.Core.Managers
{
    public class ExternalIdentityManager : ManagerBase
    {
        private readonly ExternalIdentityUoW m_externalIdentityUoW;

        public ExternalIdentityManager(
            ExternalIdentityUoW externalIdentityUoW,
            ILogger logger, ITranslator translator, IMapper mapper, IPaginationConfiguration paginationConfiguration
        ) : base(logger, translator, mapper, paginationConfiguration)
        {
            m_externalIdentityUoW = externalIdentityUoW;
        }

        public DataResult<IList<ExternalIdentityModel>> FindAllExternalIdentity()
        {
            try
            {
                var externalLoginProviders = m_externalIdentityUoW.FindAllExternalIdentity();
                return Success(
                    m_mapper.Map<IList<ExternalIdentityModel>>(externalLoginProviders)
                );
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<IList<ExternalIdentityModel>>(e.Message);
            }
        }

        public DataResult<IList<ExternalIdentityModel>> GetExternalIdentityByName(string name)
        {
            try
            {
                var externalLoginProviders = m_externalIdentityUoW.GetExternalIdentityByName(name);
                return Success(
                    m_mapper.Map<IList<ExternalIdentityModel>>(externalLoginProviders)
                );
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<IList<ExternalIdentityModel>>(e.Message);
            }
        }
    }
}