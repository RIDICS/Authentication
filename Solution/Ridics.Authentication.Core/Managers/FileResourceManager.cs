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
    public class FileResourceManager : ManagerBase
    {
        private readonly FileResourceUoW m_fileResourceUoW;
        
        public FileResourceManager(
            FileResourceUoW fileResourceUoW,
            ILogger logger, ITranslator translator, IMapper mapper, IPaginationConfiguration paginationConfiguration
        ) : base(logger, translator, mapper, paginationConfiguration)
        {
            m_fileResourceUoW = fileResourceUoW;
        }

        public DataResult<FileResourceModel> GetFileResource(int id)
        {
            try
            {
                var externalLoginProviders = m_fileResourceUoW.GetFileResource(id);
                return Success(
                    m_mapper.Map<FileResourceModel>(externalLoginProviders)
                );
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<FileResourceModel>(e.Message, DataResultErrorCode.FileNotExistId);
            }
        }
    }
}