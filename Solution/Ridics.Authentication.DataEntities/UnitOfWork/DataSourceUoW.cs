using System.Collections.Generic;
using DryIoc.Facilities.NHibernate;
using DryIoc.Transactions;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;
using Ridics.Authentication.DataEntities.Exceptions;
using Ridics.Authentication.DataEntities.Repositories;
using Ridics.Core.DataEntities.Shared.UnitOfWorks;

namespace Ridics.Authentication.DataEntities.UnitOfWork
{
    public class DataSourceUoW : UnitOfWorkBase
    {
        private readonly DataSourceRepository m_dataSourceRepository;
        private readonly ExternalLoginProviderRepository m_externalLoginProviderRepository;

        public DataSourceUoW(
            ISessionManager sessionManager,
            DataSourceRepository dataSourceRepository,
            ExternalLoginProviderRepository externalLoginProviderRepository
        ) : base(sessionManager)
        {
            m_dataSourceRepository = dataSourceRepository;
            m_externalLoginProviderRepository = externalLoginProviderRepository;
        }

        [Transaction]
        public virtual DataSourceEntity GetDataSourceById(int userId)
        {
            var dataSource = m_dataSourceRepository.GetDataSourceById(userId);

            return dataSource;
        }

        [Transaction]
        public virtual DataSourceEntity GetDataSourceByDataSource(DataSourceEnum dataSourceEnum, int externalLoginProviderId)
        {
            var dataSource = m_dataSourceRepository.GetDataSourceByDataSource(dataSourceEnum, externalLoginProviderId);

            return dataSource;
        }

        [Transaction]
        public virtual IList<DataSourceEntity> FindDataSourceByDataSource(DataSourceEnum dataSourceEnum)
        {
            var dataSource = m_dataSourceRepository.FindDataSourceByDataSource(dataSourceEnum);

            return dataSource;
        }

        [Transaction]
        public virtual int AddDataSource(DataSourceEntity dataSourceEntity, int externalIdentityProviderId)
        {
            var externalLoginProviderEntity = m_externalLoginProviderRepository.Load<ExternalLoginProviderEntity>(externalIdentityProviderId);

            dataSourceEntity.ExternalLoginProvider = externalLoginProviderEntity;

            var result = (int) m_dataSourceRepository.Create(dataSourceEntity);

            return result;
        }

        [Transaction]
        public virtual void RemoveDataSource(int id)
        {
            var dataSource = m_dataSourceRepository.GetDataSourceById(id); //TODO select claim directly, not collection

            if (dataSource == null)
            {
                throw new NoResultException<DataSourceEntity>();
            }

            m_dataSourceRepository.Delete(dataSource);
        }
    }
}
