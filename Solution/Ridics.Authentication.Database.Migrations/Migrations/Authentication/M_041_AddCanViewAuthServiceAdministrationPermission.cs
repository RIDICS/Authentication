using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(041)]
    public class M_041_AddCanViewAuthServiceAdministrationPermission : ForwardOnlyMigration
    {
        public M_041_AddCanViewAuthServiceAdministrationPermission()
        {
        }

        public override void Up()
        {
            Insert.IntoTable("Permission")
                .Row(new {Name = "auth-can-view-auth-service-administration", Description = "Permission to display/work with auth service administration section"});

            Execute.WithConnection((connection, transaction) =>
            {
                var adminId = Query.Conn(connection, transaction).Select<int>("Id").From("Role")
                    .Where("Name", "Admin")
                    .Run().Single();

                var permissionCanViewAuthServiceId = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("Permission")
                    .Where("Name", "auth-can-view-auth-service-administration")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("Role_Permission")
                    .Row(new { RoleId = adminId, PermissionId = permissionCanViewAuthServiceId })
                    .Run();
            });
        }
    }
}