using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(081)]
    public class M_081_AddClientProperty : ForwardOnlyMigration
    {
        public M_081_AddClientProperty()
        {
        }

        public override void Up()
        {
            Alter.Table("Client")
                .AddColumn("AllowAccessTokensViaBrowser")
                .AsBoolean()
                .SetExistingRowsTo(false);
        }
    }
}