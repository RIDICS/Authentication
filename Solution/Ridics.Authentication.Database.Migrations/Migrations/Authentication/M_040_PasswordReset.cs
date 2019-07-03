using FluentMigrator;
using Ridics.Authentication.Database.Migrations.Extensions;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(040)]
    public class M_040_PasswordReset : ForwardOnlyMigration
    {
        public M_040_PasswordReset()
        {
        }

        public override void Up()
        {
            Alter.Table("User")
                .AddColumn("PasswordResetToken").AsString(255).Nullable()
                .AddColumn("PasswordResetTokenCreateTime").AsDateTime().Nullable();

            this.CreateUniqueConstraint("UQ_User(PasswordResetToken)", "User", "PasswordResetToken");
        }
    }
}