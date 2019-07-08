using System;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Configuration;
using Ridics.Authentication.Core.Models.DataResult;
using Ridics.Authentication.Shared;

namespace Ridics.Authentication.Core.Managers
{
    public abstract class ManagerBase
    {
        protected readonly ILogger m_logger;
        protected readonly ITranslator m_translator;
        protected readonly IMapper m_mapper;
        protected readonly IPaginationConfiguration m_paginationConfiguration;

        protected ManagerBase(ILogger logger, ITranslator translator, IMapper mapper, IPaginationConfiguration paginationConfiguration)
        {
            m_logger = logger;
            m_translator = translator;
            m_mapper = mapper;
            m_paginationConfiguration = paginationConfiguration;
        }

        protected DataResult<T> Error<T>(string errorMessage = null, string errorCode = null)
        {
            return new DataResult<T>
            {
                Error = new DataResultError
                {
                    Code = string.IsNullOrEmpty(errorCode) ? DataResultErrorCode.GenericError : errorCode,
                    Message = string.IsNullOrEmpty(errorMessage) ? m_translator.Translate("error-occured") : errorMessage
                }
            };
        }

        protected DataResult<T> Error<T>(DataResultError resultError)
        {
            return new DataResult<T>
            {
                Error = resultError
            };
        }

        protected DataResult<T> Success<T>(T result)
        {
            return new DataResult<T>
            {
                Result = result,
            };
        }

        protected int GetItemsOnPageCount(int requestedCount)
        {
            return Math.Min(requestedCount, m_paginationConfiguration.MaxItemsOnPage);
        }
    }
}