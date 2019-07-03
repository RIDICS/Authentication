using FluentMigrator;
using Ridics.Authentication.Database.Migrations.Extensions;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(016)]
    public class M_016_UserRegistrationCode : ForwardOnlyMigration
    {
        public M_016_UserRegistrationCode()
        {
        }

        public override void Up()
        {
            Alter.Table("User")
                .AddColumn("VerificationCode").AsString(10).Nullable()
                .AddColumn("VerificationCodeCreateTime").AsDateTime().Nullable();

            this.CreateUniqueConstraint("UQ_User(VerificationCode)", "User", "VerificationCode");
        }
    }
}