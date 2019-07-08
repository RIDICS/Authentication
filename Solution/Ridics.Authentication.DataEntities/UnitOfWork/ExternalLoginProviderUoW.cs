using System.Collections.Generic;
using DryIoc.Facilities.NHibernate;
using DryIoc.Transactions;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Exceptions;
using Ridics.Authentication.DataEntities.Repositories;
using Ridics.Core.DataEntities.Shared.UnitOfWorks;

namespace Ridics.Authentication.DataEntities.UnitOfWork
{
    public class ExternalLoginProviderUoW : UnitOfWorkBase
    {
        private readonly ExternalLoginProviderRepository m_externalLoginProviderRepository;
        private readonly FileResourceRepository m_fileResourceRepository;
        private readonly DynamicModuleRepository m_dynamicModuleRepository;

        public ExternalLoginProviderUoW(
            ISessionManager sessionManager,
            ExternalLoginProviderRepository externalLoginProviderRepository,
            FileResourceRepository fileResourceRepository,
            DynamicModuleRepository dynamicModuleRepository
        ) : base(sessionManager)
        {
            m_externalLoginProviderRepository = externalLoginProviderRepository;
            m_fileResourceRepository = fileResourceRepository;
            m_dynamicModuleRepository = dynamicModuleRepository;
        }

        [Transaction]
        public virtual IList<ExternalLoginProviderEntity> FindAllExternalLoginProviders()
        {
            var externalLoginProviders = m_externalLoginProviderRepository.FindAllExternalLoginProviders();

            return externalLoginProviders;
        }

        [Transaction]
        public virtual IList<ExternalLoginProviderEntity> FindExternalLoginProviders(int start, int count)
        {
            var externalLoginProviders = m_externalLoginProviderRepository.FindExternalLoginProviders(start, count);

            return externalLoginProviders;
        }

        [Transaction]
        public virtual int GetExternalLoginProvidersCount()
        {
            var externalLoginProvidersCount = m_externalLoginProviderRepository.GetExternalLoginProvidersCount();

            return externalLoginProvidersCount;
        }

        [Transaction]
        public virtual IList<ExternalLoginProviderEntity> FindManageableExternalLoginProviders()
        {
            var externalLoginProviders = m_externalLoginProviderRepository.FindManageableExternalLoginProviders();

            return externalLoginProviders;
        }

        [Transaction]
        public virtual ExternalLoginProviderEntity GetExternalLoginProviderByName(string name)
        {
            var externalLoginProvider = m_externalLoginProviderRepository.GetExternalLoginProviderByName(name);

            if (externalLoginProvider == null)
            {
                throw new NoResultException<ExternalLoginProviderEntity>();
            }

            return externalLoginProvider;
        }

        [Transaction]
        public virtual ExternalLoginProviderEntity GetExternalLoginProvidersByDynamicModule(int dynamicModuleId)
        {
            var externalLoginProvider = m_externalLoginProviderRepository.GetExternalLoginProviderByDynamicModuleId(dynamicModuleId);

            if (externalLoginProvider == null)
            {
                throw new NoResultException<ExternalLoginProviderEntity>();
            }

            return externalLoginProvider;
        }

        [Transaction]
        public virtual bool UpdateLogo(int id, int fileId)
        {
            var externalLoginProvider = m_externalLoginProviderRepository.GetExternalLoginProviderById(id);

            externalLoginProvider.Logo = m_fileResourceRepository.Load<FileResourceEntity>(fileId);

            m_externalLoginProviderRepository.Update(externalLoginProvider);

            return true;
        }

        [Transaction]
        public virtual bool UpdateNames(int id, string name, string displayName)
        {
            var externalLoginProvider = m_externalLoginProviderRepository.GetExternalLoginProviderById(id);

            externalLoginProvider.Name = name;
            externalLoginProvider.DisplayName = displayName;

            m_externalLoginProviderRepository.Update(externalLoginProvider);

            return true;
        }

        [Transaction]
        public virtual bool UpdateDynamicModule(int id, int dynamicModuleId)
        {
            var externalLoginProvider = m_externalLoginProviderRepository.GetExternalLoginProviderById(id);

            externalLoginProvider.DynamicModule = m_dynamicModuleRepository.Load<DynamicModuleEntity>(dynamicModuleId);

            m_externalLoginProviderRepository.Update(externalLoginProvider);

            return true;
        }

        [Transaction]
        public virtual int CreateExternalLoginByDynamicModule(
            int dynamicModuleId,
            ExternalLoginProviderEntity externalLoginProviderEntity,
            int fileId
        )
        {
            externalLoginProviderEntity.DynamicModule = m_dynamicModuleRepository.Load<DynamicModuleEntity>(dynamicModuleId);
            externalLoginProviderEntity.Logo = m_fileResourceRepository.Load<FileResourceEntity>(fileId);

            return (int) m_externalLoginProviderRepository.Create(externalLoginProviderEntity);
        }
    }
}
