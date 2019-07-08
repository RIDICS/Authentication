using System.Collections.Generic;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Configuration;
using Ridics.Authentication.Shared;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.DataResult;
using Ridics.Authentication.Core.Models.Enum;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;
using Ridics.Authentication.DataEntities.Exceptions;
using Ridics.Authentication.DataEntities.UnitOfWork;
using Ridics.Core.Shared;
using Ridics.Core.Shared.Exceptions;

namespace Ridics.Authentication.Core.Managers
{
    public class DataSourceManager : ManagerBase
    {
        private readonly DataSourceUoW m_dataSourceUoW;

        public DataSourceManager(
            DataSourceUoW dataSourceUoW,
            ILogger logger, ITranslator translator, IMapper mapper, IPaginationConfiguration paginationConfiguration
        ) : base(logger, translator, mapper, paginationConfiguration)
        {
            m_dataSourceUoW = dataSourceUoW;
        }

        public DataResult<DataSourceModel> GetDataSourceById(int id)
        {
            try
            {
                var dataSource = m_dataSourceUoW.GetDataSourceById(id);
                var dataSourceModel = m_mapper.Map<DataSourceModel>(dataSource);

                return Success(dataSourceModel);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<DataSourceModel>(e.Message);
            }
        }

        public DataResult<DataSourceModel> GetDataSourceByDataSource(DataSourceEnumModel dataSourceEnumModel, ExternalLoginProviderModel externalLoginProvider)
        {
            try
            {
                var dataSourceEnum = m_mapper.Map<DataSourceEnum>(dataSourceEnumModel);

                var dataSource = m_dataSourceUoW.GetDataSourceByDataSource(dataSourceEnum, externalLoginProvider.Id);
                var dataSourceModel = m_mapper.Map<DataSourceModel>(dataSource);

                return Success(dataSourceModel);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<DataSourceModel>(e.Message);
            }
        }

        public DataResult<IList<DataSourceModel>> FindDataSourceByDataSource(DataSourceEnumModel dataSourceEnumModel)
        {
            try
            {
                var dataSourceEnum = m_mapper.Map<DataSourceEnum>(dataSourceEnumModel);

                var dataSource = m_dataSourceUoW.FindDataSourceByDataSource(dataSourceEnum);
                var dataSourceModel = m_mapper.Map<IList<DataSourceModel>>(dataSource);

                return Success(dataSourceModel);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<IList<DataSourceModel>>(e.Message);
            }
        }

        public DataResult<int> AddDataSource(DataSourceEnumModel dataSourceEnumModel,ExternalLoginProviderModel externalLoginProvider)
        {
            var dataSourceEnum = m_mapper.Map<DataSourceEnum>(dataSourceEnumModel);

            var dataSource = new DataSourceEntity
            {
                DataSource = dataSourceEnum
            };

            try
            {
                var result = m_dataSourceUoW.AddDataSource(dataSource, externalLoginProvider.Id);
                return Success(result);
            }
            catch (NoResultException<ExternalLoginProviderEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<int>(m_translator.Translate("invalid-external-login-provider-id"), DataResultErrorCode.ClaimNothingToRemove);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<bool> RemoveDataSource(int id)
        {
            try
            {
                m_dataSourceUoW.RemoveDataSource(id);
                return Success(true);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }
    }
}
