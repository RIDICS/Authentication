using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(025)]
    public class M_025_UserDataTypesData : ForwardOnlyMigration
    {
        public M_025_UserDataTypesData()
        {
        }

        public override void Up()
        {
            Insert.IntoTable("UserDataType")
                .Row(new {DataTypeValue = "Title"})
                .Row(new {DataTypeValue = "Prefix" })
                .Row(new {DataTypeValue = "SecondName" })
                .Row(new {DataTypeValue = "FullName" })
                .Row(new {DataTypeValue = "Suffix" });
        }
    }
}