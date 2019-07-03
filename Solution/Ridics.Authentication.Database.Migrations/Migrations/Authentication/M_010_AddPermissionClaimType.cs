using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(010)]
    public class M_010_AddPermissionClaimType : ForwardOnlyMigration
    {
        public M_010_AddPermissionClaimType()
        {
        }

        public override void Up()
        {
            Execute.WithConnection((connection, transaction) =>
            {
                var claimTypeStringEnumId = Query.Conn(connection, transaction).Select<int>("Id").From("ClaimTypeEnum")
                    .Where("Name", "String")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("ClaimType")
                    .Row(new { Name = "permission", Description = "permission claim", ClaimTypeEnumId = claimTypeStringEnumId })
                    .Run();
            });
        }
    }
}