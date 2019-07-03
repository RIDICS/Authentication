using System.Collections;
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
    [Migration(053)]
    public class M_053_MigrateDataToUriType : ForwardOnlyMigration
    {
        public override void Up()
        {
            Execute.WithConnection((connection, transaction) =>
            {
                Query.Conn(connection, transaction).Insert("UriType")
                    .Row(new { Value = "Redirect" })
                    .Row(new { Value = "PostLogoutRedirect" })
                    .Row(new { Value = "CorsOrigin" })
                    .Row(new { Value = "FrontChannelLogout" })
                    .Run();

                var redirectUris = Query.Conn(connection, transaction).Select<int>("Id").From("Uri")
                    .Where("IsRedirect", true)
                    .Run();

                var postLogoutUris = Query.Conn(connection, transaction).Select<int>("Id").From("Uri")
                    .Where("IsPostLogoutRedirect", true)
                    .Run();

                var corsOrigin = Query.Conn(connection, transaction).Select<int>("Id").From("Uri")
                    .Where("IsCorsOrigin", true)
                    .Run();


                var redirectUriType = Query.Conn(connection, transaction).Select<int>("Id").From("UriType")
                    .Where("Value", "Redirect")
                    .Run().Single();

                var postLogoutUriType = Query.Conn(connection, transaction).Select<int>("Id").From("UriType")
                    .Where("Value", "PostLogoutRedirect")
                    .Run().Single();

                var corsOriginType = Query.Conn(connection, transaction).Select<int>("Id").From("UriType")
                    .Where("Value", "CorsOrigin")
                    .Run().Single();

                var frontChannelLogoutType = Query.Conn(connection, transaction).Select<int>("Id").From("UriType")
                    .Where("Value", "FrontChannelLogout")
                    .Run().Single();

                InsertRowsIntoUriUriTypeTable(connection, transaction, redirectUris, redirectUriType);
                InsertRowsIntoUriUriTypeTable(connection, transaction, postLogoutUris, postLogoutUriType);
                InsertRowsIntoUriUriTypeTable(connection, transaction, corsOrigin, corsOriginType);

                var frontChannelLogoutUriIds = CreateNewFrontendLogoutUris(connection, transaction);

                InsertRowsIntoUriUriTypeTable(connection, transaction, frontChannelLogoutUriIds, frontChannelLogoutType);
            });
        }

        private void InsertRowsIntoUriUriTypeTable(IDbConnection connection, IDbTransaction transaction, IEnumerable uriIds, int uriTypeId)
        {
            var insertQueryBuilder = Query.Conn(connection, transaction).Insert("Uri_UriType");
            foreach (var uriId in uriIds)
            {
                insertQueryBuilder = insertQueryBuilder.Row(new { UriId = uriId, UriTypeId = uriTypeId });
            }
            insertQueryBuilder.Run();
        }

        private List<int> CreateNewFrontendLogoutUris(IDbConnection connection, IDbTransaction transaction)
        {
            var clientIds = Query.Conn(connection, transaction).Select<int>("Id").From("Client")
                .Run();

            var uriIdList = new List<int>();
            foreach (var clientId in clientIds)
            {
                var corsOrigin = Query.Conn(connection, transaction).Select<string>("Uri").From("Uri")
                    .Where("ClientId", clientId)
                    .Where("IsCorsOrigin", true)
                    .Run();

                foreach (var uri in corsOrigin)
                {
                    var frontChannelUri = uri.Last() == '/'
                        ? $"{uri}account/clientsignout"
                        : $"{uri}/account/clientsignout";

                    Query.Conn(connection, transaction).Insert("Uri")
                        .Row(new
                        {
                            Uri = frontChannelUri,
                            ClientId = clientId,
                            IsRedirect = false,
                            IsPostLogoutRedirect = false,
                            IsCorsOrigin = false,
                        })
                        .Run();

                    var uriId = Query.Conn(connection, transaction).Select<int>("Id").From("Uri")
                        .Where("Uri", frontChannelUri)
                        .Where("ClientId", clientId)
                        .Run().Single();

                    uriIdList.Add(uriId);
                }
            }

            return uriIdList;
        }
    }
}