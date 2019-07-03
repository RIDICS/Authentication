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
    [Migration(017)]
    public class M_017_TestClient : ForwardOnlyMigration
    {
        public M_017_TestClient()
        {
        }

        public override void Up()
        {
            Insert.IntoTable("Client")
                .Row(new
                {
                    Name = "test-client",
                    Description = (string) null,
                    DisplayUrl = "https://localhost:44325/",
                    LogoUrl = "",
                    RequireConsent = false
                });

            Execute.WithConnection((connection, transaction) =>
            {
                var clientId = Query.Conn(connection, transaction).Select<int>("Id").From("Client")
                    .Where("Name", "test-client")
                    .Run().Single();

                var grantTypeIdHybridId = Query.Conn(connection, transaction).Select<int>("Id").From("GrantType")
                    .Where("Value", "hybrid")
                    .Run().Single();

                var grantTypeIdClientCredentialsId = Query.Conn(connection, transaction).Select<int>("Id").From("GrantType")
                    .Where("Value", "client_credentials")
                    .Run().Single();

                var grantTypeIdPasswordId = Query.Conn(connection, transaction).Select<int>("Id").From("GrantType")
                    .Where("Value", "password")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("Client_GrantType")
                    .Row(new {ClientId = clientId, GrantTypeId = grantTypeIdHybridId})
                    .Row(new {ClientId = clientId, GrantTypeId = grantTypeIdClientCredentialsId})
                    .Row(new {ClientId = clientId, GrantTypeId = grantTypeIdPasswordId})
                    .Run();

                var claimTypeEnumId = Query.Conn(connection, transaction).Select<int>("Id").From("ClaimTypeEnum")
                    .Where("Name", "String")
                    .Run().Single();

                var claimTypeIdOpenId = Query.Conn(connection, transaction).Select<int>("Id").From("ClaimType")
                    .Where("Name", "openidclaim")
                    .Run().Single();

                var claimTypeIdRole = Query.Conn(connection, transaction).Select<int>("Id").From("ClaimType")
                    .Where("Name", "role")
                    .Run().Single();

                var claimTypeIdName = Query.Conn(connection, transaction).Select<int>("Id").From("ClaimType")
                    .Where("Name", "name")
                    .Run().Single();

                var claimTypeIdEmail = Query.Conn(connection, transaction).Select<int>("Id").From("ClaimType")
                    .Where("Name", "email")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("Uri")
                    .Row(new
                    {
                        Uri = "http://localhost:50345/",
                        IsRedirect = false,
                        IsPostLogoutRedirect = false,
                        IsCorsOrigin = true,
                        ClientId = clientId
                    })
                    .Row(new
                    {
                        Uri = "http://localhost:50345/signin-oidc",
                        IsRedirect = true,
                        IsPostLogoutRedirect = false,
                        IsCorsOrigin = false,
                        ClientId = clientId
                    })
                    .Row(new
                    {
                        Uri = "http://localhost:50345/signout-callback-oidc",
                        IsRedirect = false,
                        IsPostLogoutRedirect = true,
                        IsCorsOrigin = false,
                        ClientId = clientId
                    })
                    .Row(new
                    {
                        Uri = "https://localhost:44325/",
                        IsRedirect = false,
                        IsPostLogoutRedirect = false,
                        IsCorsOrigin = true,
                        ClientId = clientId
                    })
                    .Row(new
                    {
                        Uri = "https://localhost:44325/signin-oidc",
                        IsRedirect = true,
                        IsPostLogoutRedirect = false,
                        IsCorsOrigin = false,
                        ClientId = clientId
                    })
                    .Row(new
                    {
                        Uri = "https://localhost:44325/signout-callback-oidc",
                        IsRedirect = false,
                        IsPostLogoutRedirect = true,
                        IsCorsOrigin = false,
                        ClientId = clientId
                    })
                    .Run();

                var resourceIdOpenId = Query.Conn(connection, transaction).Select<int>("Id").From("Resource")
                    .Where("Name", "openid")
                    .Run().Single();

                var resourceIdProfile = Query.Conn(connection, transaction).Select<int>("Id").From("Resource")
                    .Where("Name", "profile")
                    .Run().Single();

                var resourceIdAuthProvider = Query.Conn(connection, transaction).Select<int>("Id").From("Resource")
                    .Where("Name", "authorization-provider")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("Secret")
                    .Row(new
                    {
                        Value = "secret",
                        Description = "secret",
                        Expiration = (DateTime?) null,
                        ResourceId = (int?) null,
                        ClientId = clientId,
                        Discriminator = "ClientSecret"
                    })
                    .Run();

                Query.Conn(connection, transaction).Insert("Client_ApiResource")
                    .Row(new {ClientId = clientId, ApiResourceId = resourceIdAuthProvider})
                    .Run();

                Query.Conn(connection, transaction).Insert("Client_IdentityResource")
                    .Row(new {ClientId = clientId, IdentityResourceId = resourceIdOpenId})
                    .Row(new {ClientId = clientId, IdentityResourceId = resourceIdProfile})
                    .Run();
            });
        }
    }
}