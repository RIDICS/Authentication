using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(070)]
    public class M_070_AddMasterUserIdToClaims : ForwardOnlyMigration
    {
        public M_070_AddMasterUserIdToClaims()
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
                    .Row(new {Name = "master-user-id", Description = "Master user ID (GUID)", ClaimTypeEnumId = stringClaimTypeId })
                    .Run();

                var resourceProfileId = Query.Conn(connection, transaction).Select<int>("Id").From("Resource")
                    .Where("Name", "profile")
                    .Run().Single();

                var masterUserIdClaimTypeId = Query.Conn(connection, transaction).Select<int>("Id").From("ClaimType")
                    .Where("Name", "master-user-id")
                    .Run().Single();
                
                Query.Conn(connection, transaction).Insert("Resource_ClaimType")
                    .Row(new {ResourceId = resourceProfileId, ClaimTypeId = masterUserIdClaimTypeId})
                    .Run();
            });
        }
    }
}