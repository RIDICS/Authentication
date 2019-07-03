using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(049)]
    public class M_049_UserContactDropUniqueConstraint : ForwardOnlyMigration
    {
        public M_049_UserContactDropUniqueConstraint()
        {
        }

        public override void Up()
        {
            Delete.Index("UQ_UserContact(Value)")
                .OnTable("UserContact");
        }
    }
}