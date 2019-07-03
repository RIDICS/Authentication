using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(074)]
    public class M_074_DeleteObsoleteApiResource : ForwardOnlyMigration
    {
        public M_074_DeleteObsoleteApiResource()
        {
        }

        public override void Up()
        {
            Execute.WithConnection((connection, transaction) =>
            {
                var apiId = Query.Conn(connection, transaction).Select<int>("Id").From("Resource")
                    .Where("Name", "authorization-provider")
                    .Run().Single();

                Query.Conn(connection, transaction).DeleteFrom("Resource_ClaimType")
                    .Where("ResourceId", apiId)
                    .Run();
                
                Query.Conn(connection, transaction).DeleteFrom("Client_ApiResource")
                    .Where("ApiResourceId", apiId)
                    .Run();

                Query.Conn(connection, transaction).DeleteFrom("Resource")
                    .Where("Id", apiId)
                    .Run();
            });
        }
    }
}