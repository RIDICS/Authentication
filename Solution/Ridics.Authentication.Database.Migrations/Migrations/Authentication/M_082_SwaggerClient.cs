using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    //[EnvironmentTags(MigrationEnvironmentTagTypes.Development)]
    [Migration(082)]
    public class M_082_SwaggerClient : ForwardOnlyMigration
    {
        public M_082_SwaggerClient()
        {
        }

        public override void Up()
        {
            Insert.IntoTable("Client")
                .Row(new
                {
                    Name = "Swagger",
                    Description = (string)null,
                    DisplayUrl = "Swagger",
                    LogoUrl = "Swagger",
                    RequireConsent = false,
                    AllowAccessTokensViaBrowser = true,
                });

            Execute.WithConnection((connection, transaction) =>
            {
                var clientId = Query.Conn(connection, transaction).Select<int>("Id").From("Client")
                    .Where("Name", "Swagger")
                    .Run().Single();

                InsertGrantTypes(connection, transaction, clientId);
                InsertScopes(connection, transaction, clientId);
                InsertUris(connection, transaction, clientId);
                InsertSecret(connection, transaction, clientId);
            });
        }

        private void InsertSecret(IDbConnection connection, IDbTransaction transaction, int clientId)
        {
            Query.Conn(connection, transaction).Insert("Secret")
                .Row(new
                {
                    Value = "secret",
                    Description = "secret",
                    Expiration = (DateTime?)null,
                    ResourceId = (int?)null,
                    ClientId = clientId,
                    Discriminator = "ClientSecret"
                }).Run();
        }

        private void InsertUris(IDbConnection connection, IDbTransaction transaction, int clientId)
        {
            //Add URI only for development
            var uris = new List<string>
            {
                "https://localhost:44395/swagger/oauth2-redirect.html",
                "https://localhost/Auth/swagger/oauth2-redirect.html",
                "https://localhost:44331/swagger/oauth2-redirect.html",
            };

            var uriInsertQuery = Query.Conn(connection, transaction).Insert("Uri");

            foreach (var uri in uris)
            {
                uriInsertQuery.Row(new
                {
                    Uri = uri,
                    ClientId = clientId
                });
            }

            uriInsertQuery.Run();

            var redirectUriTypeId = Query.Conn(connection, transaction).Select<int>("Id")
                .From("UriType")
                .Where("Value", "Redirect")
                .Run().Single();

            var uriTypeInsertQuery = Query.Conn(connection, transaction).Insert("Uri_UriType");

            foreach (var uri in uris)
            {
                var uriId = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("Uri")
                    .Where("Uri", uri)
                    .Run().Single();
                uriTypeInsertQuery.Row(new { UriId = uriId, UriTypeId = redirectUriTypeId });
            }
            uriTypeInsertQuery.Run();
        }

        private void InsertScopes(IDbConnection connection, IDbTransaction transaction, int clientId)
        {
            var internalScopeId = Query.Conn(connection, transaction).Select<int>("Id")
                .From("Scope")
                .Where("Name", "auth_api.Internal")
                .Run().Single();

            var nonceScopeId = Query.Conn(connection, transaction).Select<int>("Id")
                .From("Scope")
                .Where("Name", "auth_api.Nonce")
                .Run().Single();

            Query.Conn(connection, transaction).Insert("Client_Scope")
                .Row(new { ClientId = clientId, ScopeId = internalScopeId })
                .Row(new { ClientId = clientId, ScopeId = nonceScopeId })
                .Run();
        }

        private void InsertGrantTypes(IDbConnection connection, IDbTransaction transaction, int clientId)
        {
            var grantTypeImplicitId = Query.Conn(connection, transaction).Select<int>("Id").From("GrantType")
                .Where("Value", "implicit")
                .Run().Single();

            var grantTypeClientCredentialsId = Query.Conn(connection, transaction).Select<int>("Id").From("GrantType")
                .Where("Value", "client_credentials")
                .Run().Single();

            Query.Conn(connection, transaction).Insert("Client_GrantType")
                .Row(new { ClientId = clientId, GrantTypeId = grantTypeImplicitId })
                .Row(new { ClientId = clientId, GrantTypeId = grantTypeClientCredentialsId })
                .Run();
        }
    }
}