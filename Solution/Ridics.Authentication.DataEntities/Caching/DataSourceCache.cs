using System.Collections.Concurrent;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;
using Ridics.Authentication.DataEntities.Repositories;

namespace Ridics.Authentication.DataEntities.Caching
{
    public class DataSourceCache
    {
        private readonly DataSourceRepository m_dataSourceRepository;
        private readonly ExternalLoginProviderRepository m_externalLoginProviderRepository;

        private ConcurrentDictionary<DataSourceEnum, DataSourceEntity> m_cache;
        private ConcurrentDictionary<string, DataSourceEntity> m_cacheExternalDource;

        public DataSourceCache(
            DataSourceRepository dataSourceRepository,
            ExternalLoginProviderRepository externalLoginProviderRepository
        )
        {
            m_dataSourceRepository = dataSourceRepository;
            m_externalLoginProviderRepository = externalLoginProviderRepository;

            m_cache = new ConcurrentDictionary<DataSourceEnum, DataSourceEntity>();
            m_cacheExternalDource = new ConcurrentDictionary<string, DataSourceEntity>();
        }

        public DataSourceEntity GetByExternalProvider(string externalProviderName)
        {
            if (string.IsNullOrEmpty(externalProviderName))
            {
                return GetByEnum(DataSourceEnum.User);
            }

            return m_cacheExternalDource.GetOrAdd(externalProviderName, key => m_dataSourceRepository.GetDataSourceByDataSource(
                DataSourceEnum.ExternalLoginProvider,
                m_externalLoginProviderRepository.GetExternalLoginProviderByName(
                    externalProviderName
                ).Id
            ));
        }

        public DataSourceEntity GetByEnum(DataSourceEnum levelOfAssuranceEnum)
        {
            return m_cache.GetOrAdd(levelOfAssuranceEnum, key => m_dataSourceRepository.GetDataSourceByDataSource(
                key
            ));
        }
    }
}
