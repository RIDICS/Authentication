using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(067)]
    public class M_067_ExternalProviderHideOnLoginScreen : ForwardOnlyMigration
    {
        public M_067_ExternalProviderHideOnLoginScreen()
        {
        }

        public override void Up()
        {
            Alter.Table("ExternalLoginProvider")
                .AddColumn("HideOnLoginScreen").AsBoolean().NotNullable().SetExistingRowsTo(false);
        }
    }
}
