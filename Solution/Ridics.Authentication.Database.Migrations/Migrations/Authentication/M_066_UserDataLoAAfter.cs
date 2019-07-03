using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(066)]
    public class M_066_UserDataLoAAfter : ForwardOnlyMigration
    {
        public M_066_UserDataLoAAfter()
        {
        }

        public override void Up()
        {
            Alter.Table("UserData")
                .AlterColumn("LevelOfAssuranceId").AsInt32().NotNullable()
                .AlterColumn("DataSourceId").AsInt32().NotNullable();

            Alter.Table("UserContact")
                .AlterColumn("LevelOfAssuranceId").AsInt32().NotNullable()
                .AlterColumn("DataSourceId").AsInt32().NotNullable();
        }
    }
}
