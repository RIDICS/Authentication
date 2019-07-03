using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(054)]
    public class M_054_DeleteUriTypeColumns : ForwardOnlyMigration
    {
        public override void Up()
        {
            Delete.Column("IsRedirect").FromTable("Uri");
            Delete.Column("IsPostLogoutRedirect").FromTable("Uri");
            Delete.Column("IsCorsOrigin").FromTable("Uri");
        }
    }
}