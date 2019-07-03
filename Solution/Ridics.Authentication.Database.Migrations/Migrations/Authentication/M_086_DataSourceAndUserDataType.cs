using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(086)]
    public class M_086_DataSourceAndUserDataType : ForwardOnlyMigration
    {
        public M_086_DataSourceAndUserDataType()
        {
        }

        public override void Up()
        {
            Execute.WithConnection((connection, transaction) =>
            {
                Query.Conn(connection, transaction).Insert("DataSource")
                    .Row(new { DataSource = "System" })
                    .Run();

                Query.Conn(connection, transaction).Insert("UserDataType")
                    .Row(new { DataTypeValue = "FirstName" })
                    .Row(new { DataTypeValue = "LastName" })
                    .Row(new { DataTypeValue = "MasterUserId" })
                    .Run();
            });
        }
    }
}