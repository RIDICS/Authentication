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
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(078)]
    public class M_078_ClientApiResourceScopeRelationship : ForwardOnlyMigration
    {
        public M_078_ClientApiResourceScopeRelationship()
        {
        }

        public override void Up()
        {
            CreateClientScopeTable();

            Execute.WithConnection((connection, transaction) =>
            {
                var apiResources = Query.Conn(connection, transaction).Select<ApiResourceResult>()
                    .From("Resource")
                    .Where("Discriminator", "ApiResource")
                    .Run().ToList();

                if (apiResources.Count > 0)
                {
                    InsertDefaultScopesForApiResources(connection, transaction, apiResources);

                    CreateScopeClientRelationship(connection, transaction, apiResources);
                }
            });

            DeleteClientApiResourceTable();
        }

        private void CreateScopeClientRelationship(IDbConnection connection, IDbTransaction transaction, IEnumerable<ApiResourceResult> apiResources)
        {
            var insertClientScopeQuery = Query.Conn(connection, transaction).Insert("Client_Scope");
            foreach (var apiResource in apiResources)
            {
                var scopeId = Query.Conn(connection, transaction).Select<int>("Id").From("Scope")
                    .Where("Name", apiResource.Name)
                    .Run().Single();

                var clientIds = Query.Conn(connection, transaction).Select<int>("ClientId").From("Client_ApiResource")
                    .Where("ApiResourceId", apiResource.Id)
                    .Run();

                foreach (var clientId in clientIds)
                {
                    insertClientScopeQuery.Row(new {ClientId = clientId, ScopeId = scopeId});
                }
            }
            insertClientScopeQuery.RunAlsoIfEmpty();
        }

        private void InsertDefaultScopesForApiResources(IDbConnection connection, IDbTransaction transaction, IEnumerable<ApiResourceResult> apiResources)
        {
            var insertApiScopeQuery = Query.Conn(connection, transaction).Insert("Scope");
            foreach (var apiResource in apiResources)
            {
                insertApiScopeQuery.Row(new
                {
                    Name = apiResource.Name,
                    Required = apiResource.Required,
                    ShowInDiscoveryDocument = apiResource.ShowInDiscoveryDocument,
                    ResourceId = apiResource.Id,
                });
            }
            insertApiScopeQuery.Run();
        }

        private void DeleteClientApiResourceTable()
        {
            Delete.Table("Client_ApiResource");
        }

        private void CreateClientScopeTable()
        {
            Create.Table("Client_Scope")
                .WithColumn("ClientId").AsInt32().NotNullable().ForeignKey("FK_Client_Scope(ClientId)", "Client", "Id")
                .WithColumn("ScopeId").AsInt32().NotNullable().ForeignKey("FK_Client_Scope(ScopeId)", "Scope", "Id");

            Create.UniqueConstraint("UQ_Client_Scope(ClientId)(ScopeId)")
                .OnTable("Client_Scope")
                .Columns("ClientId", "ScopeId");
        }

        private class ApiResourceResult
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool Required { get; set; }
            public bool ShowInDiscoveryDocument { get; set; }
        }
    }
}