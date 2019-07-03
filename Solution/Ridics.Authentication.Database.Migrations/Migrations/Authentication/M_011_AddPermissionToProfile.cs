using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(011)]
    public class M_011_AddPermissionToProfile : ForwardOnlyMigration
    {
        public M_011_AddPermissionToProfile()
        {
        }

        public override void Up()
        {
            Execute.WithConnection((connection, transaction) =>
            {
                var resourceProfileId = Query.Conn(connection, transaction).Select<int>("Id").From("Resource")
                    .Where("Name", "profile")
                    .Run().Single();

                var permissionClaimTypeId = Query.Conn(connection, transaction).Select<int>("Id").From("ClaimType")
                    .Where("Name", "permission")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("Resource_ClaimType")
                    .Row(new { ResourceId = resourceProfileId, ClaimTypeId = permissionClaimTypeId })
                    .Run();
            });
        }
    }
}