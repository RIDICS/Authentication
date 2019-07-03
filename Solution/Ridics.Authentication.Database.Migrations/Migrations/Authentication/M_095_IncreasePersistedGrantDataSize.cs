using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(095)]
    public class M_095_IncreasePersistedGrantDataSize : ForwardOnlyMigration
    {
        public M_095_IncreasePersistedGrantDataSize()
        {
        }

        public override void Up()
        {
            Alter.Table("PersistedGrant")
                .AlterColumn("Data")
                .AsString(int.MaxValue);
        }
    }
}