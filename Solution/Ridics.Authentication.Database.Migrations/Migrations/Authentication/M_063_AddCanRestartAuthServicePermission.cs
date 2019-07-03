using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(063)]
    public class M_063_AddCanRestartAuthServicePermission : ForwardOnlyMigration
    {
        public M_063_AddCanRestartAuthServicePermission()
        {
        }

        public override void Up()
        {
            Insert.IntoTable("Permission")
                .Row(new {Name = "auth-can-restart-auth-service", Description = "Permission to allow restart authorization service"});

            Execute.WithConnection((connection, transaction) =>
            {
                var adminId = Query.Conn(connection, transaction).Select<int>("Id").From("Role")
                    .Where("Name", "Admin")
                    .Run().Single();

                var permissionCanRestartAuthServiceId = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("Permission")
                    .Where("Name", "auth-can-restart-auth-service")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("Role_Permission")
                    .Row(new { RoleId = adminId, PermissionId = permissionCanRestartAuthServiceId })
                    .Run();
            });
        }
    }
}
