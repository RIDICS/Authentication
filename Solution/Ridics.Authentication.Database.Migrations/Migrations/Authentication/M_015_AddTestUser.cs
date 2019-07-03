using System;
using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(015)]
    public class M_015_AddTestUser : ForwardOnlyMigration
    {
        public M_015_AddTestUser()
        {
        }

        public override void Up()
        {
            Insert.IntoTable("User")
                .Row(new
                {
                    FirstName = "Portal",
                    LastName = "Admin",
                    SecurityStamp = "PZHWFLQC753DIDIVPORGFCTOXSLVJVGI",
                    TwoFactorEnabled = false,
                    LockoutEndDateUtc = (DateTime?)null,
                    LockoutEnabled = false,
                    AccessFailedCount = 0,
                    Username = "portaladmin",
                    PasswordHash = "AQAAAAEAACcQAAAAEFB4dM+nR3ID36aQoAfP9MkY9X3ypFubC/ftzivV4fOc82o6Z/gaL9CCEr/+Mb1vSA==", // password is 'administrator'
                    LastChange = "2018-09-04 00:00:00.000"
                });

            Execute.WithConnection((connection, transaction) =>
            {
                var userId = Query.Conn(connection, transaction).Select<int>("Id").From("User")
                    .Where("Username", "portaladmin")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("UserContact")
                    .Row(new
                    {
                        UserId = userId,
                        Type = "Email",
                        Value = "portaladmin@example.com",
                        Confirmed = false,
                        CreateTime = new DateTime(2018, 9, 4),
                        ConfirmCodeChangeTime = (DateTime?) null
                    })
                    .Row(new
                    {
                        UserId = userId,
                        Type = "Phone",
                        Value = "+420000000002",
                        Confirmed = false,
                        CreateTime = new DateTime(2018, 9, 4),
                        ConfirmCodeChangeTime = (DateTime?)null
                    })
                    .Run();

                var portalAdminRoleId = Query.Conn(connection, transaction).Select<int>("Id").From("Role")
                    .Where("Name", "PortalAdmin")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("User_Role")
                    .Row(new {UserId = userId, RoleId = portalAdminRoleId})
                    .Run();
            });
        }
    }
}