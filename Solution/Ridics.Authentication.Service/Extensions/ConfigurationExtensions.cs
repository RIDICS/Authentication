using Microsoft.Extensions.Configuration;
using Ridics.Core.DataEntities.Shared.ConfigModels;

namespace Ridics.Authentication.Service.Extensions
{
    public static class ConfigurationExtensions
    {
        public static DatabaseServerType GetDatabaseServerType(this IConfiguration configuration)
        {
            return configuration.GetValue<DatabaseServerType>("DatabaseServer");
        }
    }
}
