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
    [Migration(079)]
    public class M_079_AuthApiResource : ForwardOnlyMigration
    {
        public M_079_AuthApiResource()
        {
        }

        public override void Up()
        {
            Insert.IntoTable("Resource")
                .Row(new
                {
                    Name = "auth_api",
                    Description = "API for communication with auth service",
                    Required = false,
                    ShowInDiscoveryDocument = false,
                    Discriminator = "ApiResource"
                });

            Execute.WithConnection((connection, transaction) =>
            {
                var apiResourceId = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("Resource")
                    .Where("Name", "auth_api")
                    .Run().Single();

                var permissionClaimTypeId = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("ClaimType")
                    .Where("Name", "permission")
                    .Run().Single();

                var clientIds = Query.Conn(connection, transaction).Select<int>("Id").From("Client")
                    .Run();

                CreateScopesForApiResource(connection, transaction, apiResourceId);

                var internalScopeId = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("Scope")
                    .Where("Name", "auth_api.Internal")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("Resource_ClaimType")
                    .Row(new {ResourceId = apiResourceId, ClaimTypeId = permissionClaimTypeId})
                    .Run();

                CreateScopesForClient(connection, transaction, clientIds, internalScopeId);
            });
        }

        private void CreateScopesForApiResource(IDbConnection connection, IDbTransaction transaction, int apiResourceId)
        {
            Query.Conn(connection, transaction).Insert("Scope")
                .Row(new
                {
                    Name = "auth_api",
                    Description = "Default scope for auth api",
                    Required = false,
                    ShowInDiscoveryDocument = false,
                    ResourceId = apiResourceId
                })
                .Row(new
                {
                    Name = "auth_api.Internal",
                    Description = "Scope for internal services, i.e. webhub",
                    Required = false,
                    ShowInDiscoveryDocument = false,
                    ResourceId = apiResourceId
                })
                .Row(new
                {
                    Name = "auth_api.Nonce",
                    Description = "Scope for Oauth service for accessing nonce endpoint",
                    Required = false,
                    ShowInDiscoveryDocument = false,
                    ResourceId = apiResourceId
                })
                .Run();
        }

        private void CreateScopesForClient(IDbConnection connection, IDbTransaction transaction, IEnumerable<int> clientIds, int internalScopeId)
        {
            var insertApiScopeToClientQuery = Query.Conn(connection, transaction).Insert("Client_Scope");
            foreach (var clientId in clientIds)
            {
                insertApiScopeToClientQuery.Row(new { ClientId = clientId, ScopeId = internalScopeId });
            }

            insertApiScopeToClientQuery.Run();
        }
    }
}