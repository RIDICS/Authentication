using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(020)]
    public class M_020_DistinguishAuthenticationServiceAdmin : ForwardOnlyMigration
    {
        public M_020_DistinguishAuthenticationServiceAdmin()
        {
        }

        public override void Up()
        {
            Alter.Table("Role")
                .AddColumn("AuthenticationServiceOnly")
                .AsBoolean()
                .SetExistingRowsTo(false)
                .NotNullable();

            Execute.WithConnection((connection, transaction) =>
            {
                var adminUserId = Query.Conn(connection, transaction).Select<int>("Id").From("Role")
                    .Where("Name", "Admin")
                    .Run().Single();

                Query.Conn(connection, transaction).Update("Role")
                    .Set("AuthenticationServiceOnly", true)
                    .Where("Id", adminUserId)
                    .Run();
            });
        }
    }
}