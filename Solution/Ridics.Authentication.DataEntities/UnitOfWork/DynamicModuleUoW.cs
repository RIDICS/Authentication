using System;
using System.Collections.Generic;
using DryIoc.Facilities.NHibernate;
using DryIoc.Transactions;
using NHibernate;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;
using Ridics.Authentication.DataEntities.Exceptions;
using Ridics.Authentication.DataEntities.Repositories;
using Ridics.Core.DataEntities.Shared.UnitOfWorks;

namespace Ridics.Authentication.DataEntities.UnitOfWork
{
    public class DynamicModuleUoW : UnitOfWorkBase
    {
        private readonly DynamicModuleRepository m_dynamicModuleRepository;
        private readonly DynamicModuleBlobRepository m_dynamicModuleBlobRepository;
        private readonly ExternalLoginProviderRepository m_externalLoginProviderRepository;
        private readonly ExternalLoginRepository m_externalLoginRepository;
        private readonly FileResourceRepository m_fileResourceRepository;

        public DynamicModuleUoW(
            ISessionManager sessionManager,
            DynamicModuleRepository dynamicModuleRepository,
            DynamicModuleBlobRepository dynamicModuleBlobRepository,
            ExternalLoginProviderRepository externalLoginProviderRepository,
            ExternalLoginRepository externalLoginRepository,
            FileResourceRepository fileResourceRepository
        ) : base(sessionManager)
        {
            m_dynamicModuleRepository = dynamicModuleRepository;
            m_dynamicModuleBlobRepository = dynamicModuleBlobRepository;
            m_externalLoginProviderRepository = externalLoginProviderRepository;
            m_externalLoginRepository = externalLoginRepository;
            m_fileResourceRepository = fileResourceRepository;
        }

        [Transaction]
        public virtual IList<DynamicModuleEntity> FindAllDynamicModule()
        {
            var dynamicModules = m_dynamicModuleRepository.FindAllDynamicModule();

            return dynamicModules;
        }

        [Transaction]
        public virtual IList<DynamicModuleEntity> FindAllDynamicModule(int start, int count)
        {
            var dynamicModules = m_dynamicModuleRepository.FindAllDynamicModule(start, count);

            return dynamicModules;
        }

        [Transaction]
        public virtual DynamicModuleEntity GetByName(string name)
        {
            var dynamicModule = m_dynamicModuleRepository.GetByName(name);

            return dynamicModule;
        }

        [Transaction]
        public virtual DynamicModuleEntity GetById(int id)
        {
            var dynamicModule = m_dynamicModuleRepository.GetById(id);

            return dynamicModule;
        }

        [Transaction]
        public virtual int GetDynamicModuleCount()
        {
            var usersCount = m_dynamicModuleRepository.GetDynamicModuleCount();

            return usersCount;
        }

        [Transaction]
        public virtual int CreateDynamicModule(DynamicModuleEntity dynamicModuleEntity)
        {
            var result = (int) m_dynamicModuleRepository.Create(dynamicModuleEntity);

            return result;
        }

        [Transaction]
        public virtual void UpdateConfiguration(int id, string moduleConfiguration, Version configurationVersion)
        {
            var dynamicModuleEntity = m_dynamicModuleRepository.GetById(id);

            if (dynamicModuleEntity == null)
            {
                throw new NoResultException<DynamicModuleEntity>();
            }

            dynamicModuleEntity.Configuration = moduleConfiguration;
            dynamicModuleEntity.ConfigurationVersion = configurationVersion;

            m_dynamicModuleRepository.Update(dynamicModuleEntity);
        }

        [Transaction]
        public virtual void DeleteDynamicModuleWithId(int id)
        {
            try
            {
                var dynamicModule = m_dynamicModuleRepository.Load<DynamicModuleEntity>(id);

                try
                {
                    var externalLoginProvider = m_externalLoginProviderRepository.GetExternalLoginProviderByDynamicModuleId(
                        dynamicModule.Id
                    );

                    if (externalLoginProvider != null)
                    {
                        var isUsedByUsers = m_externalLoginRepository.GetExternalLoginCountByProvider(externalLoginProvider.Id);

                        if (isUsedByUsers > 0)
                        {
                            throw new NotSupportedException(
                                $"Unable to delete dynamic module '{id}', it is used by {isUsedByUsers} user(s)");
                        }

                        m_dynamicModuleRepository.Delete(externalLoginProvider);
                    }
                }
                catch (NoResultException<ExternalLoginProviderEntity>)
                {
                    // it can be standalone dynamic module == without linked external login provider
                }

                m_dynamicModuleRepository.Delete(dynamicModule);
            }
            catch (ObjectNotFoundException)
            {
                throw new NoResultException<DynamicModuleEntity>();
            }
        }

        [Transaction]
        public virtual DynamicModuleBlobEntity GetDynamicModuleBlob(int dynamicModuleId, DynamicModuleBlobEnum dynamicModuleBlobEnum)
        {
            var dynamicModuleBlob = m_dynamicModuleBlobRepository.GetByModuleIdAndType(dynamicModuleId, dynamicModuleBlobEnum);

            return dynamicModuleBlob;
        }

        [Transaction]
        public virtual int CreateDynamicModuleBlob(
            int dynamicModuleId,
            DynamicModuleBlobEnum dynamicModuleBlobEnum,
            string fileExtension,
            Action<FileResourceEntity> persistStream
        )
        {
            var dynamicModule = GetById(dynamicModuleId);

            var fileResourceId = m_fileResourceRepository.CreateExternal(fileExtension);

            var fileResource = m_fileResourceRepository.GetFileResource(fileResourceId);

            var dynamicModuleBlobEntity = new DynamicModuleBlobEntity
            {
                DynamicModule = dynamicModule,
                Type = dynamicModuleBlobEnum,
                File = fileResource,
                LastChange = DateTime.UtcNow,
            };

            var result = (int) m_dynamicModuleRepository.Create(dynamicModuleBlobEntity);

            persistStream(fileResource);

            return result;
        }

        [Transaction]
        public virtual bool UpdateDynamicModuleBlob(
            int dynamicModuleBlobId,
            string fileExtension,
            Action<FileResourceEntity> persistStream
        )
        {
            var result = m_dynamicModuleBlobRepository.GetById(dynamicModuleBlobId);

            result.LastChange = DateTime.UtcNow;
            m_dynamicModuleBlobRepository.Update(result);

            result.File.FileExtension = fileExtension;
            m_fileResourceRepository.Update(result.File);

            var fileResource = m_fileResourceRepository.GetFileResource(result.File.Id);
            persistStream(fileResource);

            return true;
        }
    }
}
