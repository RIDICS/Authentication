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
    [Migration(002)]
    public class M_002_DefaultUserAndValues : ForwardOnlyMigration
    {
        public M_002_DefaultUserAndValues()
        {
        }

        public override void Up()
        {
            Insert.IntoTable("User")
                .Row(new
                {
                    FirstName = "Admin",
                    LastName = "Administrator",
                    SecurityStamp = "PZHWFLQC753DIDIVPORGFCTOXSLVJVGI",
                    TwoFactorEnabled = false,
                    LockoutEndDateUtc = (DateTime?) null,
                    LockoutEnabled = false,
                    AccessFailedCount = 0,
                    Username = "admin",
                    PasswordHash = "AQAAAAEAACcQAAAAEFB4dM+nR3ID36aQoAfP9MkY9X3ypFubC/ftzivV4fOc82o6Z/gaL9CCEr/+Mb1vSA==", // password is 'administrator'
                    LastChange = "2018-07-10 00:00:00.000"
                });

            Insert.IntoTable("Role")
                .Row(new {Name = "Admin"});

            Insert.IntoTable("GrantType")
                .Row(new {DisplayName = "Implicit", Value = "implicit"})
                .Row(new {DisplayName = "Hybrid", Value = "hybrid"})
                .Row(new {DisplayName = "AuthorizationCode", Value = "authorization_code"})
                .Row(new {DisplayName = "ClientCredentials", Value = "client_credentials"})
                .Row(new {DisplayName = "ResourceOwnerPassword", Value = "password"});

            Insert.IntoTable("ClaimTypeEnum")
                .Row(new {Name = "String"})
                .Row(new {Name = "Int"})
                .Row(new {Name = "DateTime"})
                .Row(new {Name = "Boolean"});

            Execute.WithConnection((connection, transaction) =>
            {
                var userId = Query.Conn(connection, transaction).Select<int>("Id").From("User")
                    .Where("Username", "admin")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("UserContact")
                    .Row(new
                    {
                        UserId = userId,
                        Type = "Email",
                        Value = "admin@example.com",
                        Confirmed = true,
                        CreateTime = new DateTime(2018, 7, 10),
                        ConfirmCodeChangeTime = (DateTime?) null
                    })
                    .Row(new
                    {
                        UserId = userId,
                        Type = "Phone",
                        Value = "+420000000001",
                        Confirmed = true,
                        CreateTime = new DateTime(2018, 7, 10),
                        ConfirmCodeChangeTime = (DateTime?) null
                    })
                    .Run();

                var roleId = Query.Conn(connection, transaction).Select<int>("Id").From("Role")
                    .Where("Name", "Admin")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("User_Role")
                    .Row(new {UserId = userId, RoleId = roleId})
                    .Run();
            });
        }
    }
}