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
    [Migration(083)]
    public class M_083_AddForumAndBatchImportClient : ForwardOnlyMigration
    {
        public M_083_AddForumAndBatchImportClient()
        {
        }

        public override void Up()
        {
            Execute.WithConnection((connection, transaction) =>
            {
                var uriTypeIdRedirect = Query.Conn(connection, transaction).Select<int>("Id").From("UriType")
                    .Where("Value", "Redirect")
                    .Run().Single();

                var uriTypeIdPost = Query.Conn(connection, transaction).Select<int>("Id").From("UriType")
                    .Where("Value", "PostLogoutRedirect")
                    .Run().Single();

                var uriTypeIdCors = Query.Conn(connection, transaction).Select<int>("Id").From("UriType")
                    .Where("Value", "CorsOrigin")
                    .Run().Single();

                var uriTypeIdFront = Query.Conn(connection, transaction).Select<int>("Id").From("UriType")
                    .Where("Value", "FrontChannelLogout")
                    .Run().Single();


                var scopeId = Query.Conn(connection, transaction).Select<int>("Id").From("Scope")
                    .Where("Name", "auth_api.Internal")
                    .Run().Single();

                var grantTypeId1 = Query.Conn(connection, transaction).Select<int>("Id").From("GrantType")
                    .Where("Value", "client_credentials")
                    .Run().Single();

                var grantTypeId2 = Query.Conn(connection, transaction).Select<int>("Id").From("GrantType")
                    .Where("Value", "hybrid")
                    .Run().Single();

                var grantTypeId3 = Query.Conn(connection, transaction).Select<int>("Id").From("GrantType")
                    .Where("Value", "password")
                    .Run().Single();

                var identityResourceId1 = Query.Conn(connection, transaction).Select<int>("Id").From("Resource")
                    .Where("Name", "offline_access")
                    .Run().Single();

                var identityResourceId2 = Query.Conn(connection, transaction).Select<int>("Id").From("Resource")
                    .Where("Name", "openid")
                    .Run().Single();

                var identityResourceId3 = Query.Conn(connection, transaction).Select<int>("Id").From("Resource")
                    .Where("Name", "profile")
                    .Run().Single();


                // Script - add Forum client
                Query.Conn(connection, transaction).Insert("Client")
                    .Row(new
                    {
                        Name = "VokabularForum",
                        Description = "Vokabulář webový - Forum",
                        DisplayUrl = "https://vokabular.ujc.cas.cz/",
                        LogoUrl = "https://vokabular.ujc.cas.cz/",
                        RequireConsent = false,
                    })
                    .Run();

                var clientIdForum = Query.Conn(connection, transaction).Select<int>("Id").From("Client")
                    .Where("Name", "VokabularForum")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("Uri")
                    .Row(new { Uri = "http://localhost:50165/auth.aspx?auth=vokabular", ClientId = clientIdForum })
                    .Row(new { Uri = "http://localhost:50165/client_logout.aspx", ClientId = clientIdForum })
                    .Row(new { Uri = "https://localhost/Forum/auth.aspx?auth=vokabular", ClientId = clientIdForum })
                    .Row(new { Uri = "https://localhost/Forum/client_logout.aspx", ClientId = clientIdForum })
                    .Run();

                var forumUriId1 = Query.Conn(connection, transaction).Select<int>("Id").From("Uri")
                    .Where("Uri", "http://localhost:50165/auth.aspx?auth=vokabular")
                    .Run().Single();

                var forumUriId2 = Query.Conn(connection, transaction).Select<int>("Id").From("Uri")
                    .Where("Uri", "http://localhost:50165/client_logout.aspx")
                    .Run().Single();

                var forumUriId3 = Query.Conn(connection, transaction).Select<int>("Id").From("Uri")
                    .Where("Uri", "https://localhost/Forum/auth.aspx?auth=vokabular")
                    .Run().Single();

                var forumUriId4 = Query.Conn(connection, transaction).Select<int>("Id").From("Uri")
                    .Where("Uri", "https://localhost/Forum/client_logout.aspx")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("Uri_UriType")
                    .Row(new { UriId = forumUriId1, UriTypeId = uriTypeIdRedirect })
                    .Row(new { UriId = forumUriId3, UriTypeId = uriTypeIdRedirect })
                    .Row(new { UriId = forumUriId2, UriTypeId = uriTypeIdFront })
                    .Row(new { UriId = forumUriId4, UriTypeId = uriTypeIdFront })
                    .Run();

                Query.Conn(connection, transaction).Insert("Secret")
                    .Row(new
                    {
                        Value = "secret",
                        Description = "secret",
                        Expiration = (DateTime?)null,
                        ResourceId = (int?)null,
                        ClientId = clientIdForum,
                        Discriminator = "ClientSecret"
                    })
                    .Run();

                var grantTypeId4 = Query.Conn(connection, transaction).Select<int>("Id").From("GrantType")
                    .Where("Value", "authorization_code")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("Client_Scope")
                    .Row(new { ClientId = clientIdForum, ScopeId = scopeId })
                    .Run();

                Query.Conn(connection, transaction).Insert("Client_GrantType")
                    .Row(new { ClientId = clientIdForum, GrantTypeId = grantTypeId4 })
                    .Run();

                Query.Conn(connection, transaction).Insert("Client_IdentityResource")
                    .Row(new { ClientId = clientIdForum, IdentityResourceId = identityResourceId1 })
                    .Row(new { ClientId = clientIdForum, IdentityResourceId = identityResourceId2 })
                    .Row(new { ClientId = clientIdForum, IdentityResourceId = identityResourceId3 })
                    .Run();


                // Script - add Batch import client
                Query.Conn(connection, transaction).Insert("Client")
                    .Row(new
                    {
                        Name = "VokabularBatchImport",
                        Description = "Vokabulář webový - batch import books",
                        DisplayUrl = "https://vokabular.ujc.cas.cz/",
                        LogoUrl = "https://vokabular.ujc.cas.cz/",
                        RequireConsent = false,
                    })
                    .Run();

                var clientIdBatchImport = Query.Conn(connection, transaction).Select<int>("Id").From("Client")
                    .Where("Name", "VokabularBatchImport")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("Uri")
                    .Row(new { Uri = "http://127.0.0.1:7890/", ClientId = clientIdBatchImport })
                    .Row(new { Uri = "http://127.0.0.1:7890/logout", ClientId = clientIdBatchImport })
                    .Run();

                var batchImportUriId1 = Query.Conn(connection, transaction).Select<int>("Id").From("Uri")
                    .Where("Uri", "http://127.0.0.1:7890/")
                    .Run().Single();

                var batchImportUriId2 = Query.Conn(connection, transaction).Select<int>("Id").From("Uri")
                    .Where("Uri", "http://127.0.0.1:7890/logout")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("Uri_UriType")
                    .Row(new { UriId = batchImportUriId1, UriTypeId = uriTypeIdRedirect })
                    .Row(new { UriId = batchImportUriId2, UriTypeId = uriTypeIdFront })
                    .Run();

                Query.Conn(connection, transaction).Insert("Secret")
                    .Row(new
                    {
                        Value = "secret",
                        Description = "secret",
                        Expiration = (DateTime?)null,
                        ResourceId = (int?)null,
                        ClientId = clientIdBatchImport,
                        Discriminator = "ClientSecret"
                    })
                    .Run();


                Query.Conn(connection, transaction).Insert("Client_Scope")
                    .Row(new { ClientId = clientIdBatchImport, ScopeId = scopeId })
                    .Run();

                Query.Conn(connection, transaction).Insert("Client_GrantType")
                    .Row(new { ClientId = clientIdBatchImport, GrantTypeId = grantTypeId2 })
                    .Run();

                Query.Conn(connection, transaction).Insert("Client_IdentityResource")
                    .Row(new { ClientId = clientIdBatchImport, IdentityResourceId = identityResourceId2 })
                    .Row(new { ClientId = clientIdBatchImport, IdentityResourceId = identityResourceId3 })
                    .Run();
            });
        }
    }
}