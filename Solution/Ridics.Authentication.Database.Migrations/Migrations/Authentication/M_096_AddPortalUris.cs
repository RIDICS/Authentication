using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(096)]
    public class M_096_AddPortalUris : ForwardOnlyMigration
    {
        public M_096_AddPortalUris()
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


                var clientId = Query.Conn(connection, transaction).Select<int>("Id").From("Client")
                    .Where("Name", "Vokabular")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("Uri")
                    .Row(new {Uri = "https://localhost/Research/", ClientId = clientId})
                    .Row(new {Uri = "https://localhost/Research/account/clientsignout", ClientId = clientId})
                    .Row(new {Uri = "https://localhost/Research/signin-oidc", ClientId = clientId})
                    .Row(new {Uri = "https://localhost/Research/signout-callback-oidc", ClientId = clientId})
                    .Row(new {Uri = "https://localhost/Community/", ClientId = clientId})
                    .Row(new {Uri = "https://localhost/Community/account/clientsignout", ClientId = clientId})
                    .Row(new {Uri = "https://localhost/Community/signin-oidc", ClientId = clientId})
                    .Row(new {Uri = "https://localhost/Community/signout-callback-oidc", ClientId = clientId})
                    .Run();

                var uriId1 = Query.Conn(connection, transaction).Select<int>("Id").From("Uri")
                    .Where("Uri", "https://localhost/Research/")
                    .Run().Single();

                var uriId2 = Query.Conn(connection, transaction).Select<int>("Id").From("Uri")
                    .Where("Uri", "https://localhost/Research/account/clientsignout")
                    .Run().Single();

                var uriId3 = Query.Conn(connection, transaction).Select<int>("Id").From("Uri")
                    .Where("Uri", "https://localhost/Research/signin-oidc")
                    .Run().Single();

                var uriId4 = Query.Conn(connection, transaction).Select<int>("Id").From("Uri")
                    .Where("Uri", "https://localhost/Research/signout-callback-oidc")
                    .Run().Single();

                var uriId5 = Query.Conn(connection, transaction).Select<int>("Id").From("Uri")
                    .Where("Uri", "https://localhost/Community/")
                    .Run().Single();

                var uriId6 = Query.Conn(connection, transaction).Select<int>("Id").From("Uri")
                    .Where("Uri", "https://localhost/Community/account/clientsignout")
                    .Run().Single();

                var uriId7 = Query.Conn(connection, transaction).Select<int>("Id").From("Uri")
                    .Where("Uri", "https://localhost/Community/signin-oidc")
                    .Run().Single();

                var uriId8 = Query.Conn(connection, transaction).Select<int>("Id").From("Uri")
                    .Where("Uri", "https://localhost/Community/signout-callback-oidc")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("Uri_UriType")
                    .Row(new {UriId = uriId1, UriTypeId = uriTypeIdCors})
                    .Row(new {UriId = uriId2, UriTypeId = uriTypeIdFront})
                    .Row(new {UriId = uriId3, UriTypeId = uriTypeIdRedirect})
                    .Row(new {UriId = uriId4, UriTypeId = uriTypeIdPost})
                    .Row(new {UriId = uriId5, UriTypeId = uriTypeIdCors})
                    .Row(new {UriId = uriId6, UriTypeId = uriTypeIdFront})
                    .Row(new {UriId = uriId7, UriTypeId = uriTypeIdRedirect})
                    .Row(new {UriId = uriId8, UriTypeId = uriTypeIdPost})
                    .Run();
            });
        }
    }
}