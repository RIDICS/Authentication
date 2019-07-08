using System.Collections.Generic;
using Ridics.Authentication.Service.Configuration.Enum;

namespace Ridics.Authentication.Service.Configuration
{
    public class FeatureFlagsConfiguration
    {
        public IDictionary<FeatureFlagsEnum, bool> EnabledFeatures { get; set; } = new Dictionary<FeatureFlagsEnum, bool>();
    }
}
