using Ridics.Authentication.Service.Configuration;
using Ridics.Authentication.Service.Configuration.Enum;

namespace Ridics.Authentication.Service.Managers
{
    public class FeatureFlagsManager
    {
        private readonly FeatureFlagsConfiguration m_featureFlagsConfiguration;

        public FeatureFlagsManager(
            FeatureFlagsConfiguration featureFlagsConfiguration
        )
        {
            m_featureFlagsConfiguration = featureFlagsConfiguration;
        }

        public bool IsFeatureEnabled(FeatureFlagsEnum featureFlag)
        {
            if (m_featureFlagsConfiguration.EnabledFeatures.TryGetValue(featureFlag, out var enabled))
            {
                return enabled;
            }

            return false;
        }
    }
}
