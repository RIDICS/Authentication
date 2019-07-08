using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Configuration;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.DataResult;
using Ridics.Authentication.Core.Models.Enum;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.UnitOfWork;
using Ridics.Authentication.Shared;
using Ridics.Core.Shared;
using Ridics.Core.Shared.Exceptions;

namespace Ridics.Authentication.Core.Managers
{
    public class ExternalLoginProviderManager : ManagerBase
    {
        private readonly ExternalLoginProviderUoW m_externalLoginProviderUoW;

        public ExternalLoginProviderManager(
            ExternalLoginProviderUoW externalLoginProviderUoW,
            ILogger logger,
            ITranslator translator,
            IMapper mapper,
            IPaginationConfiguration paginationConfiguration
        ) : base(logger, translator, mapper, paginationConfiguration)
        {
            m_externalLoginProviderUoW = externalLoginProviderUoW;
        }

        public DataResult<IList<ExternalLoginProviderModel>> FindAllExternalLoginProviders()
        {
            try
            {
                var externalLoginProviders = m_externalLoginProviderUoW.FindAllExternalLoginProviders();
                return Success(
                    m_mapper.Map<IList<ExternalLoginProviderModel>>(externalLoginProviders)
                );
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<IList<ExternalLoginProviderModel>>(e.Message);
            }
        }

        public DataResult<IList<ExternalLoginProviderModel>> FindExternalLoginProviders(int start, int count)
        {
            try
            {
                var externalLoginProviders = m_externalLoginProviderUoW.FindExternalLoginProviders(start, count);
                return Success(
                    m_mapper.Map<IList<ExternalLoginProviderModel>>(externalLoginProviders)
                );
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<IList<ExternalLoginProviderModel>>(e.Message);
            }
        }

        public DataResult<int> GetExternalLoginProvidersCount()
        {
            try
            {
                var externalLoginProvidersCount = m_externalLoginProviderUoW.GetExternalLoginProvidersCount();
                return Success(
                    externalLoginProvidersCount
                );
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<IList<ExternalLoginProviderModel>> FindManageableExternalLoginProviders()
        {
            try
            {
                var externalLoginProviders = m_externalLoginProviderUoW.FindManageableExternalLoginProviders();
                return Success(
                    m_mapper.Map<IList<ExternalLoginProviderModel>>(externalLoginProviders)
                );
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<IList<ExternalLoginProviderModel>>(e.Message);
            }
        }

        public DataResult<ExternalLoginProviderModel> GetExternalLoginProvidersByName(string name)
        {
            try
            {
                var externalLoginProvider = m_externalLoginProviderUoW.GetExternalLoginProviderByName(name);
                return Success(
                    m_mapper.Map<ExternalLoginProviderModel>(externalLoginProvider)
                );
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<ExternalLoginProviderModel>(e.Message);
            }
        }

        public DataResult<ExternalLoginProviderModel> GetExternalLoginProvidersByDynamicModule(DynamicModuleModel dynamicModule)
        {
            try
            {
                var externalLoginProvider = m_externalLoginProviderUoW.GetExternalLoginProvidersByDynamicModule(dynamicModule.Id);
                return Success(
                    m_mapper.Map<ExternalLoginProviderModel>(externalLoginProvider)
                );
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<ExternalLoginProviderModel>(e.Message);
            }
        }

        public DataResult<bool> UpdateLogo(ExternalLoginProviderModel externalLoginProvider, FileResourceModel file)
        {
            try
            {
                return Success(
                    m_externalLoginProviderUoW.UpdateLogo(externalLoginProvider.Id, file.Id)
                );
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> UpdateNames(
            ExternalLoginProviderModel externalLoginProvider,
            DynamicModuleModel dynamicModule,
            Type moduleInfoModuleConfigType
        )
        {
            var dynamicModuleConfiguration = dynamicModule.Configuration(moduleInfoModuleConfigType);

            if (string.IsNullOrEmpty(dynamicModuleConfiguration.Name))
            {
                var message = $"Property '{nameof(dynamicModuleConfiguration.Name)}' is required";

                m_logger.LogWarning(message);
                return Error<bool>(message);
            }

            if (string.IsNullOrEmpty(dynamicModuleConfiguration.DisplayName))
            {
                var message = $"Property '{nameof(dynamicModuleConfiguration.DisplayName)}' is required";

                m_logger.LogWarning(message);
                return Error<bool>(message);
            }

            try
            {
                return Success(
                    m_externalLoginProviderUoW.UpdateNames(
                        externalLoginProvider.Id,
                        dynamicModuleConfiguration.Name,
                        dynamicModuleConfiguration.DisplayName
                    )
                );
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> UpdateDynamicModule(ExternalLoginProviderModel externalLoginProvider, DynamicModuleModel dynamicModule)
        {
            try
            {
                return Success(
                    m_externalLoginProviderUoW.UpdateDynamicModule(
                        externalLoginProvider.Id,
                        dynamicModule.Id
                    )
                );
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<int> CreateExternalLoginByDynamicModule(DynamicModuleModel dynamicModule, Type dynamicModuleType)
        {
            var mainLogo = dynamicModule.DynamicModuleBlobs.FirstOrDefault(
                x => x.Type == DynamicModuleBlobEnumModel.MainLogo
            );

            if (mainLogo == null)
            {
                var message =
                    $"Unable to create {nameof(ExternalLoginProviderEntity)} without {nameof(DynamicModuleBlobEnumModel.MainLogo)}";

                m_logger.LogWarning(message);
                return Error<int>(message);
            }

            var externalLoginProviderEntity = new ExternalLoginProviderEntity
            {
                Name = dynamicModule.Name,
                DisplayName = dynamicModule.Configuration(dynamicModuleType).DisplayName ?? dynamicModule.Name,
            };

            try
            {
                return Success(
                    m_externalLoginProviderUoW.CreateExternalLoginByDynamicModule(
                        dynamicModule.Id,
                        externalLoginProviderEntity,
                        mainLogo.File.Id
                    )
                );
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }
    }
}
