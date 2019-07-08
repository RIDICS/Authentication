using System.Collections.Concurrent;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;
using Ridics.Authentication.DataEntities.Repositories;

namespace Ridics.Authentication.DataEntities.Caching
{
    public class LevelOfAssuranceCache
    {
        private readonly LevelOfAssuranceRepository m_levelOfAssuranceRepository;

        private ConcurrentDictionary<LevelOfAssuranceEnum, LevelOfAssuranceEntity> m_cache;

        public LevelOfAssuranceCache(LevelOfAssuranceRepository levelOfAssuranceRepository)
        {
            m_levelOfAssuranceRepository = levelOfAssuranceRepository;
            m_cache = new ConcurrentDictionary<LevelOfAssuranceEnum, LevelOfAssuranceEntity>();
        }

        public LevelOfAssuranceEntity GetByEnum(LevelOfAssuranceEnum levelOfAssuranceEnum)
        {
            return m_cache.GetOrAdd(levelOfAssuranceEnum, key => m_levelOfAssuranceRepository.GetLevelOfAssuranceByName(
                key
            ));
        }
    }
}
