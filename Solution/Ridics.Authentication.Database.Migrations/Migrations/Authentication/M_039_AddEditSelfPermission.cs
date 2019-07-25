using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(039)]
    public class M_039_AddEditSelfPermission : ForwardOnlyMigration
    {
        public M_039_AddEditSelfPermission()
        {
        }

        public override void Up()
        {
            Insert.IntoTable("Permission")
                .Row(new { Name = "auth-edit-self-personal-data", Description = "Permission to edit own personal data" })
                .Row(new { Name = "auth-edit-self-contacts", Description = "Permission to edit own contacts" });

            Execute.WithConnection((connection, transaction) =>
            {
                var adminUserId = Query.Conn(connection, transaction).Select<int>("Id").From("Role")
                    .Where("Name", "Admin")
                    .Run().Single();

                var portalAdminUserId = Query.Conn(connection, transaction).Select<int>("Id").From("Role")
                    .Where("Name", "PortalAdmin")
                    .Run().Single();

                var registeredUserId = Query.Conn(connection, transaction).Select<int>("Id").From("Role")
                    .Where("Name", "RegisteredUser")
                    .Run().Single();

                var verifiedUserId = Query.Conn(connection, transaction).Select<int>("Id").From("Role")
                    .Where("Name", "VerifiedUser")
                    .Run().Single();

                var permissionEditSelfPersonalData = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("Permission")
                    .Where("Name", "auth-edit-self-personal-data")
                    .Run().Single();

                var permissionEditSelfContacts = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("Permission")
                    .Where("Name", "auth-edit-self-contacts")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("Role_Permission")
                    .Row(new { RoleId = adminUserId, PermissionId = permissionEditSelfPersonalData })
                    .Row(new { RoleId = adminUserId, PermissionId = permissionEditSelfContacts })
                    .Row(new { RoleId = portalAdminUserId, PermissionId = permissionEditSelfPersonalData })
                    .Row(new { RoleId = portalAdminUserId, PermissionId = permissionEditSelfContacts })
                    .Row(new { RoleId = registeredUserId, PermissionId = permissionEditSelfPersonalData })
                    .Row(new { RoleId = registeredUserId, PermissionId = permissionEditSelfContacts })
                    .Row(new { RoleId = verifiedUserId, PermissionId = permissionEditSelfContacts })
                    .Run();
            });
        }
    }
}