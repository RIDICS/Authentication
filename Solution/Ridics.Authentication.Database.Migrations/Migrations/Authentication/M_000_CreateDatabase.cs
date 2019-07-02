using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Structure, CoreMigrationTypeTagTypes.Data, CoreMigrationTypeTagTypes.All)]
    [Migration(000)]
    public class M_000_CreateDatabase : ForwardOnlyMigration
    {
        public override void Up()
        {
        }
    }
}
