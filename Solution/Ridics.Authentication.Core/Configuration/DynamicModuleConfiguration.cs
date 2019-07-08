using System;

namespace Ridics.Authentication.Core.Configuration
{
    public class DynamicModuleConfiguration
    {
        public string Checksum { get; set; }
        
        public DateTime LastConfigurationReload { get; set; }
    }
}