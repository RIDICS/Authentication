using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(048)]
    public class M_048_AddEditTwoFactorPermissions : ForwardOnlyMigration
    {
        public M_048_AddEditTwoFactorPermissions()
        {
        }

        public override void Up()
        {
            Insert.IntoTable("Permission")
                .Row(new { Name = "auth-select-two-factor-provider", Description = "Permission to select two factor provider" })
                .Row(new { Name = "auth-set-two-factor", Description = "Permission to enable/disable two factor authentication" });

            Execute.WithConnection((connection, transaction) =>
            {
                var registeredUserId = Query.Conn(connection, transaction).Select<int>("Id").From("Role")
                    .Where("Name", "RegisteredUser")
                    .Run().Single();

                var verifiedUserId = Query.Conn(connection, transaction).Select<int>("Id").From("Role")
                    .Where("Name", "VerifiedUser")
                    .Run().Single();

                var adminId = Query.Conn(connection, transaction).Select<int>("Id").From("Role")
                    .Where("Name", "Admin")
                    .Run().Single();

                var portalAdminId = Query.Conn(connection, transaction).Select<int>("Id").From("Role")
                    .Where("Name", "PortalAdmin")
                    .Run().Single();

                var permissionSelectTwoFactorProvider = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("Permission")
                    .Where("Name", "auth-select-two-factor-provider")
                    .Run().Single();

                var permissionToggleTwoFactor = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("Permission")
                    .Where("Name", "auth-set-two-factor")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("Role_Permission")
                    .Row(new { RoleId = registeredUserId, PermissionId = permissionSelectTwoFactorProvider })
                    .Row(new { RoleId = registeredUserId, PermissionId = permissionToggleTwoFactor })
                    .Row(new { RoleId = verifiedUserId, PermissionId = permissionSelectTwoFactorProvider })
                    .Row(new { RoleId = verifiedUserId, PermissionId = permissionToggleTwoFactor })
                    .Row(new { RoleId = adminId, PermissionId = permissionSelectTwoFactorProvider })
                    .Row(new { RoleId = adminId, PermissionId = permissionToggleTwoFactor })
                    .Row(new { RoleId = portalAdminId, PermissionId = permissionSelectTwoFactorProvider })
                    .Row(new { RoleId = portalAdminId, PermissionId = permissionToggleTwoFactor })
                    .Run();
            });
        }
    }
}