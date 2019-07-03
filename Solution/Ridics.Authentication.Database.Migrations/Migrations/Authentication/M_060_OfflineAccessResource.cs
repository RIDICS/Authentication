using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(060)]
    public class M_060_OfflineAccessResource : ForwardOnlyMigration
    {
        public M_060_OfflineAccessResource()
        {
        }

        public override void Up()
        {
            Insert.IntoTable("Resource")
                .Row(new
                {
                    Name = "offline_access",
                    Description = "Enable client to use refresh token",
                    Required = false,
                    ShowInDiscoveryDocument = false,
                    Discriminator = "IdentityResource"
                });

            Execute.WithConnection((connection, transaction) =>
            {
                var clientIds = Query.Conn(connection, transaction).Select<int>("Id").From("Client")
                    .Run();

                var resourceIdOfflineAccess = Query.Conn(connection, transaction).Select<int>("Id").From("Resource")
                    .Where("Name", "offline_access")
                    .Run().Single();

                var claimTypeEnumId = Query.Conn(connection, transaction).Select<int>("Id").From("ClaimTypeEnum")
                    .Where("Name", "String")
                    .Run().Single();

                var insertResourceToClientQuery = Query.Conn(connection, transaction).Insert("Client_IdentityResource");
                foreach (var clientId in clientIds)
                {
                    insertResourceToClientQuery.Row(new {ClientId = clientId, IdentityResourceId = resourceIdOfflineAccess});
                }
                insertResourceToClientQuery.Run();

                Query.Conn(connection, transaction).Insert("ClaimType")
                    .Row(new {Name = "offline_access", Description = "offline_access claim", ClaimTypeEnumId = claimTypeEnumId})
                    .Run();

                var claimTypeIdOfflineAccess = Query.Conn(connection, transaction).Select<int>("Id").From("ClaimType")
                    .Where("Name", "offline_access")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("Resource_ClaimType")
                    .Row(new {ResourceId = resourceIdOfflineAccess, ClaimTypeId = claimTypeIdOfflineAccess})
                    .Run();
            });
        }
    }
}
