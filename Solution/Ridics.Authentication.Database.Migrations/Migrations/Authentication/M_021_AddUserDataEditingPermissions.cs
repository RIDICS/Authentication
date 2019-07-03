using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(021)]
    public class M_021_AddUserDataEditingPermissions : ForwardOnlyMigration
    {
        public M_021_AddUserDataEditingPermissions()
        {
        }

        public override void Up()
        {
            Insert.IntoTable("Permission")
                .Row(new
                {
                    Name = "auth-manage-non-verified-users-data",
                    Description = "Permission to manage non-verified users"
                })
                .Row(new {Name = "auth-edit-any-users-data", Description = "Permission to edit data of any user"})
                .Row(new { Name = "auth-get-non-verified-user-by-verification-code", Description = "Permission to get user by verification code" })
                .Row(new { Name = "auth-register-other-users", Description = "Permission to register other users" })
                .Row(new { Name = "auth-show-registered-user-data", Description = "Permission to show registered user's data" })
                .Row(new { Name = "auth-validate-user", Description = "Permission to validate users" })
                .Row(new { Name = "auth-find-user-by-registration-code", Description = "Permission to find user by registration code" })
                .Row(new {Name = "auth-manage-user-roles", Description = "Permission to manage user roles"})
                .Row(new
                {
                    Name = "auth-manage-user-permissions", Description = "Permission to manage user permissions"
                })
                .Row(new
                {
                    Name = "auth-assign-permissions-to-roles",
                    Description = "Permission to assign user permissions to user roles"
                })
                .Row(new {Name = "auth-list-users", Description = "Permission to list all users"});

            Execute.WithConnection((connection, transaction) =>
            {
                var rolePortalAdminId = Query.Conn(connection, transaction).Select<int>("Id").From("Role")
                    .Where("Name", "PortalAdmin")
                    .Run().Single();

                var roleAdminUserId = Query.Conn(connection, transaction).Select<int>("Id").From("Role")
                    .Where("Name", "Admin")
                    .Run().Single();

                var permissionEditNonVerifiedUsersDataId = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("Permission")
                    .Where("Name", "auth-manage-non-verified-users-data")
                    .Run().Single();

                var permissionShowRegisteredUserDataId = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("Permission")
                    .Where("Name", "auth-show-registered-user-data")
                    .Run().Single();

                var permissionGetNonVerifiedUserByVerificationCodeId = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("Permission")
                    .Where("Name", "auth-get-non-verified-user-by-verification-code")
                    .Run().Single();

                var permissionRegisterOtherUsersId = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("Permission")
                    .Where("Name", "auth-register-other-users")
                    .Run().Single();

                var permissionValidateUsersId = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("Permission")
                    .Where("Name", "auth-validate-user")
                    .Run().Single();

                var permissionFindUserByRegistrationCodeId = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("Permission")
                    .Where("Name", "auth-find-user-by-registration-code")
                    .Run().Single();

                var permissionEditAllUsersDataId = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("Permission")
                    .Where("Name", "auth-edit-any-users-data")
                    .Run().Single();

                var permissionCreateRolesId = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("Permission")
                    .Where("Name", "auth-manage-user-roles")
                    .Run().Single();

                var permissionCreatePermissionsId = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("Permission")
                    .Where("Name", "auth-manage-user-permissions")
                    .Run().Single();

                var permissionAssignPermissionToRoleId = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("Permission")
                    .Where("Name", "auth-assign-permissions-to-roles")
                    .Run().Single();

                var permissionListUsersDataId = Query.Conn(connection, transaction).Select<int>("Id").From("Permission")
                    .Where("Name", "auth-list-users")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("Role_Permission")
                    .Row(new {RoleId = roleAdminUserId, PermissionId = permissionEditAllUsersDataId})
                    .Row(new {RoleId = roleAdminUserId, PermissionId = permissionListUsersDataId})
                    .Row(new {RoleId = roleAdminUserId, PermissionId = permissionCreateRolesId })
                    .Row(new {RoleId = roleAdminUserId, PermissionId = permissionCreatePermissionsId })
                    .Row(new {RoleId = roleAdminUserId, PermissionId = permissionAssignPermissionToRoleId })
                    .Row(new {RoleId = roleAdminUserId, PermissionId = permissionGetNonVerifiedUserByVerificationCodeId })
                    .Row(new {RoleId = roleAdminUserId, PermissionId = permissionRegisterOtherUsersId })
                    .Row(new {RoleId = roleAdminUserId, PermissionId = permissionShowRegisteredUserDataId })
                    .Row(new {RoleId = roleAdminUserId, PermissionId = permissionValidateUsersId })
                    .Row(new {RoleId = roleAdminUserId, PermissionId = permissionFindUserByRegistrationCodeId })
                    .Row(new {RoleId = roleAdminUserId, PermissionId = permissionEditNonVerifiedUsersDataId })
                    .Row(new {RoleId = rolePortalAdminId, PermissionId = permissionEditNonVerifiedUsersDataId})
                    .Row(new {RoleId = rolePortalAdminId, PermissionId = permissionEditAllUsersDataId })
                    .Row(new {RoleId = rolePortalAdminId, PermissionId = permissionAssignPermissionToRoleId })
                    .Row(new {RoleId = rolePortalAdminId, PermissionId = permissionGetNonVerifiedUserByVerificationCodeId })
                    .Row(new {RoleId = rolePortalAdminId, PermissionId = permissionRegisterOtherUsersId })
                    .Row(new {RoleId = rolePortalAdminId, PermissionId = permissionShowRegisteredUserDataId })
                    .Row(new {RoleId = rolePortalAdminId, PermissionId = permissionValidateUsersId })
                    .Row(new {RoleId = rolePortalAdminId, PermissionId = permissionFindUserByRegistrationCodeId })
                    .Run();
            });
        }
    }
}