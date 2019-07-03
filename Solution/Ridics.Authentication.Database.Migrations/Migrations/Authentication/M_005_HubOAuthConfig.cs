using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(005)]
    public class M_005_HubOAuthConfig : ForwardOnlyMigration
    {
        public M_005_HubOAuthConfig()
        {
        }

        public override void Up()
        {
            Execute.WithConnection((connection, transaction) =>
            {
                var clientId = Query.Conn(connection, transaction).Select<int>("Id").From("Client")
                    .Where("Name", "Vokabular")
                    .Run().Single();
            });
        }
    }
}