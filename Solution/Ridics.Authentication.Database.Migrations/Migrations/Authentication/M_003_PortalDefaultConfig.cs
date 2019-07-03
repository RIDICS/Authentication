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
    [Migration(003)]
    public class M_003_PortalDefaultConfig : ForwardOnlyMigration
    {
        public M_003_PortalDefaultConfig()
        {
        }

        public override void Up()
        {
            Insert.IntoTable("Client")
                .Row(new
                {
                    Name = "Vokabular",
                    Description = "Vokabulář webový",
                    DisplayUrl = "https://vokabular.ujc.cas.cz/",
                    LogoUrl = "https://vokabular.ujc.cas.cz/",
                    RequireConsent = false
                });

            Execute.WithConnection((connection, transaction) =>
            {
                var clientId = Query.Conn(connection, transaction).Select<int>("Id").From("Client")
                    .Where("Name", "Vokabular")
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

                Query.Conn(connection, transaction).Insert("ClaimType")
                    .Row(new {Name = "openidclaim", Description = "openid claim", ClaimTypeEnumId = claimTypeEnumId})
                    .Row(new {Name = "role", Description = "User role", ClaimTypeEnumId = claimTypeEnumId})
                    .Row(new {Name = "name", Description = "Full name", ClaimTypeEnumId = claimTypeEnumId})
                    .Row(new {Name = "email", Description = "E-mail", ClaimTypeEnumId = claimTypeEnumId})
                    .Run();

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
                        Uri = "https://localhost:44368/",
                        IsRedirect = false,
                        IsPostLogoutRedirect = false,
                        IsCorsOrigin = true,
                        ClientId = clientId
                    })
                    .Row(new
                    {
                        Uri = "https://localhost:44368/signin-oidc",
                        IsRedirect = true,
                        IsPostLogoutRedirect = false,
                        IsCorsOrigin = false,
                        ClientId = clientId
                    })
                    .Row(new
                    {
                        Uri = "https://localhost:44368/signout-callback-oidc",
                        IsRedirect = false,
                        IsPostLogoutRedirect = true,
                        IsCorsOrigin = false,
                        ClientId = clientId
                    })
                    .Row(new
                    {
                        Uri = "https://localhost/",
                        IsRedirect = false,
                        IsPostLogoutRedirect = false,
                        IsCorsOrigin = true,
                        ClientId = clientId
                    })
                    .Row(new
                    {
                        Uri = "https://localhost/signin-oidc",
                        IsRedirect = true,
                        IsPostLogoutRedirect = false,
                        IsCorsOrigin = false,
                        ClientId = clientId
                    })
                    .Row(new
                    {
                        Uri = "https://localhost/signout-callback-oidc",
                        IsRedirect = false,
                        IsPostLogoutRedirect = true,
                        IsCorsOrigin = false,
                        ClientId = clientId
                    })
                    .Run();


                Query.Conn(connection, transaction).Insert("Resource")
                    .Row(new
                    {
                        Name = "openid",
                        Description = (string) null,
                        Required = false,
                        ShowInDiscoveryDocument = false,
                        Discriminator = "IdentityResource"
                    })
                    .Row(new
                    {
                        Name = "profile",
                        Description = (string) null,
                        Required = false,
                        ShowInDiscoveryDocument = false,
                        Discriminator = "IdentityResource"
                    })
                    .Row(new
                    {
                        Name = "authorization-provider",
                        Description = (string) null,
                        Required = false,
                        ShowInDiscoveryDocument = false,
                        Discriminator = "ApiResource"
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

                Query.Conn(connection, transaction).Insert("Resource_ClaimType")
                    .Row(new {ResourceId = resourceIdOpenId, ClaimTypeId = claimTypeIdOpenId})
                    .Row(new {ResourceId = resourceIdProfile, ClaimTypeId = claimTypeIdRole})
                    .Row(new {ResourceId = resourceIdProfile, ClaimTypeId = claimTypeIdName})
                    .Row(new {ResourceId = resourceIdProfile, ClaimTypeId = claimTypeIdEmail})
                    .Row(new {ResourceId = resourceIdAuthProvider, ClaimTypeId = claimTypeIdRole})
                    .Run();
            });
        }
    }
}