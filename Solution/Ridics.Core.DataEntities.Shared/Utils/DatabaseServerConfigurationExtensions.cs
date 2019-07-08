using System;
using NHibernate.Cfg.Loquacious;
using NHibernate.Dialect;
using NHibernate.Driver;
using Ridics.Core.DataEntities.Shared.ConfigModels;

namespace Ridics.Core.DataEntities.Shared.Utils
{
    public static class DatabaseServerConfigurationExtensions
    {
        public static void ConfigureDialectAndDriver(this IDbIntegrationConfigurationProperties db, DatabaseServerType databaseServer)
        {
            switch (databaseServer)
            {
                case DatabaseServerType.SqlServer:
                    db.Dialect<MsSql2012Dialect>();
                    db.Driver<SqlClientDriver>();
                    break;
                case DatabaseServerType.PostgreSql:
                    db.Dialect<PostgreSQL81Dialect>();
                    db.Driver<NpgsqlDriver>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(databaseServer), databaseServer, null);
            }
        }
    }
}
