using System;
using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.QueryBuilder.Shared;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(023)]
    public class M_023_AddPortalAdmin : ForwardOnlyMigration
    {
        public M_023_AddPortalAdmin()
        {
        }

        public override void Up()
        {
            Execute.WithConnection((connection, transaction) =>
            {
                var permissionNames = new[]
                {
                    "auth-manage-user-roles",
                    //"auth-manage-user-permissions", //Disabled because of very powerful permission
                    //"auth-assign-permissions-to-roles", //Disabled because of very powerful permission
                    "auth-list-users",
                };

                var permissionIds = Query.Conn(connection, transaction).Select<int>("Id").From("Permission")
                    .Where("Name", permissionNames, WhereStatementOperator.In)
                    .Run().ToList();

                if (!permissionIds.Any()) throw new ArgumentException("Check correctness of input permissions for SELECT statement");

                var portalAdminUserId = Query.Conn(connection, transaction).Select<int>("Id").From("Role")
                    .Where("Name", "PortalAdmin")
                    .Run().Single();

                var query = Query.Conn(connection, transaction).Insert("Role_Permission");

                foreach (var permissionId in permissionIds)
                {
                    query.Row(new {RoleId = portalAdminUserId, PermissionId = permissionId});
                }

                query.Run();
            });
        }
    }
}