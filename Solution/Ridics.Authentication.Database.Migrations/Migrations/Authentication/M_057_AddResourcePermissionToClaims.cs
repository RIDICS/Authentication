using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(057)]
    public class M_057_AddResourcePermissionToClaims : ForwardOnlyMigration
    {
        public M_057_AddResourcePermissionToClaims()
        {
        }

        public override void Up()
        {
            Execute.WithConnection((connection, transaction) =>
            {
                var stringClaimTypeId = Query.Conn(connection, transaction).Select<int>("Id").From("ClaimTypeEnum")
                    .Where("Name", "String")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("ClaimType")
                    .Row(new {Name = "resource-permission", Description = "Resource permission", ClaimTypeEnumId = stringClaimTypeId })
                    .Row(new {Name = "resource-permission-type", Description = "Resource type permission", ClaimTypeEnumId = stringClaimTypeId })
                    .Run();

                var resourceProfileId = Query.Conn(connection, transaction).Select<int>("Id").From("Resource")
                    .Where("Name", "profile")
                    .Run().Single();

                var resourcePermissionClaimTypeId = Query.Conn(connection, transaction).Select<int>("Id").From("ClaimType")
                    .Where("Name", "resource-permission")
                    .Run().Single();

                var resourcePermissionTypeClaimTypeId = Query.Conn(connection, transaction).Select<int>("Id").From("ClaimType")
                    .Where("Name", "resource-permission-type")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("Resource_ClaimType")
                    .Row(new {ResourceId = resourceProfileId, ClaimTypeId = resourcePermissionClaimTypeId})
                    .Row(new {ResourceId = resourceProfileId, ClaimTypeId = resourcePermissionTypeClaimTypeId})
                    .Run();
            });
        }
    }
}