using System;
using System.Collections.Generic;
using System.IO;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Configuration;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.DataResult;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;
using Ridics.Authentication.DataEntities.Exceptions;
using Ridics.Authentication.DataEntities.UnitOfWork;
using Ridics.Authentication.Modules.Shared;
using Ridics.Authentication.Shared;
using Ridics.Core.Shared;
using Ridics.Core.Shared.Exceptions;

namespace Ridics.Authentication.Core.Managers
{
    public class DynamicModuleManager : ManagerBase
    {
        private readonly DynamicModuleUoW m_dynamicModuleUoW;
        private readonly IFileResourceManager m_fileResourceManager;

        public DynamicModuleManager(
            DynamicModuleUoW dynamicModuleUoW,
            IFileResourceManager fileResourceManager,
            ILogger logger, ITranslator translator, IMapper mapper,
            IPaginationConfiguration paginationConfiguration
        ) : base(logger, translator, mapper, paginationConfiguration)
        {
            m_dynamicModuleUoW = dynamicModuleUoW;
            m_fileResourceManager = fileResourceManager;
        }

        public DataResult<List<DynamicModuleModel>> FindAllDynamicModule()
        {
            var dynamicModules = m_dynamicModuleUoW.FindAllDynamicModule();

            var viewModelList = m_mapper.Map<List<DynamicModuleModel>>(dynamicModules);

            return Success(viewModelList);
        }

        public DataResult<List<DynamicModuleModel>> FindAllDynamicModule(int start, int count)
        {
            var dynamicModules = m_dynamicModuleUoW.FindAllDynamicModule(start, count);

            var viewModelList = m_mapper.Map<List<DynamicModuleModel>>(dynamicModules);

            return Success(viewModelList);
        }

        public DataResult<DynamicModuleModel> GetByName(string name)
        {
            var dynamicModule = m_dynamicModuleUoW.GetByName(name);

            var viewModel = m_mapper.Map<DynamicModuleModel>(dynamicModule);

            return Success(viewModel);
        }

        public DataResult<DynamicModuleModel> GetById(int id)
        {
            var dynamicModule = m_dynamicModuleUoW.GetById(id);

            var model = m_mapper.Map<DynamicModuleModel>(dynamicModule);

            return Success(model);
        }

        public int GetDynamicModuleCount()
        {
            var usersCount = m_dynamicModuleUoW.GetDynamicModuleCount();

            return usersCount;
        }

        public DataResult<int> CreateDynamicModule(
            DynamicModuleModel dynamicModuleModel,
            ILibModuleInfo dynamicModuleInfo
        )
        {
            var dynamicModuleEntity = new DynamicModuleEntity
            {
                ModuleGuid = dynamicModuleModel.ModuleGuid,
                Name = dynamicModuleModel.Name,
                ConfigurationVersion = dynamicModuleInfo.Version
            };

            try
            {
                var result = m_dynamicModuleUoW.CreateDynamicModule(dynamicModuleEntity);
                return Success(result);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<bool> UpdateConfiguration(int id, IModuleConfiguration moduleConfiguration, Version configurationVersion)
        {
            try
            {
                m_dynamicModuleUoW.UpdateConfiguration(id, moduleConfiguration.Serialize(), configurationVersion);
                return Success(true);
            }
            catch (NoResultException<DynamicModuleEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-dynamic-module-id"), DataResultErrorCode.DynamicModuleNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> DeleteDynamicModuleWithId(int id)
        {
            try
            {
                m_dynamicModuleUoW.DeleteDynamicModuleWithId(id);

                return Success(true);
            }
            catch (NoResultException<DynamicModuleEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-dynamic-module-id"), DataResultErrorCode.DynamicModuleNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
            catch (NotSupportedException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<DynamicModuleBlobModel> GetDynamicModuleBlob(int dynamicModuleId, DynamicModuleBlobEnum dynamicModuleBlobEnum)
        {
            try
            {
                var dynamicModuleBlobEntity = m_dynamicModuleUoW.GetDynamicModuleBlob(dynamicModuleId, dynamicModuleBlobEnum);

                var model = m_mapper.Map<DynamicModuleBlobModel>(dynamicModuleBlobEntity);

                return Success(model);
            }
            catch (NoResultException<DynamicModuleBlobModel> e)
            {
                m_logger.LogWarning(e);
                return Error<DynamicModuleBlobModel>(
                    m_translator.Translate("invalid-dynamic-module-id"),
                    DataResultErrorCode.DynamicModuleNotExistId
                );
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<DynamicModuleBlobModel>(e.Message);
            }
        }

        public DataResult<int> CreateDynamicModuleBlob(
            int dynamicModuleId,
            DynamicModuleBlobEnum dynamicModuleBlobEnum,
            Stream fileStream,
            string fileExtension
        )
        {
            try
            {
                var result = m_dynamicModuleUoW.CreateDynamicModuleBlob(
                    dynamicModuleId,
                    dynamicModuleBlobEnum,
                    fileExtension,
                    fileResourceEntity =>
                    {
                        var model = m_mapper.Map<FileResourceModel>(fileResourceEntity);

                        using (var writeStream = m_fileResourceManager.GetWriteStream(model))
                        {
                            fileStream.CopyTo(writeStream);
                        }
                    }
                );

                return Success(result);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);

                return Error<int>(e.Message);
            }
        }

        public DataResult<bool> UpdateDynamicModuleBlob(
            int dynamicModuleBlobId,
            Stream fileStream,
            string fileExtension
        )
        {
            try
            {
                var result = m_dynamicModuleUoW.UpdateDynamicModuleBlob(
                    dynamicModuleBlobId,
                    fileExtension,
                    fileResourceEntity =>
                    {
                        var model = m_mapper.Map<FileResourceModel>(fileResourceEntity);

                        using (var writeStream = m_fileResourceManager.GetWriteStream(model))
                        {
                            fileStream.CopyTo(writeStream);
                        }
                    }
                );

                return Success(result);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);

                return Error<bool>(e.Message);
            }
        }
    }
}
