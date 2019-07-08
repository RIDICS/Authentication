using AutoMapper;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Configuration;
using Ridics.Authentication.Core.Models.DataResult;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.UnitOfWork;
using Ridics.Authentication.Shared;
using Ridics.Core.Shared;
using Ridics.Core.Shared.Exceptions;

namespace Ridics.Authentication.Core.Managers
{
    public class ExternalLoginManager : ManagerBase
    {
        private readonly UserUoW m_userUoW;

        public ExternalLoginManager(UserUoW userUoW, ILogger logger, ITranslator translator, IMapper mapper,
            IPaginationConfiguration paginationConfiguration) : base(logger, translator, mapper, paginationConfiguration)
        {
            m_userUoW = userUoW;
        }


        public DataResult<int> CreateExternalLogin(int userId, ExternalLoginProviderEntity externalLoginProvider, string providerKey)
        {
            var externalLogin = new ExternalLoginEntity
            {
                Provider = externalLoginProvider,
                ProviderKey = providerKey
            };

            try
            {
                var result = m_userUoW.CreateExternalLogin(userId, externalLogin);
                return Success(result);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }
    }
}
