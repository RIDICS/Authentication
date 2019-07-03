using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(030)]
    public class M_030_AddExternalIdentity : ForwardOnlyMigration
    {
        public M_030_AddExternalIdentity()
        {
        }

        public override void Up()
        {
            Create.Table("ExternalIdentity")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_ExternalIdentity(Id)").Identity()
                .WithColumn("Name").AsString(255).NotNullable().Unique("UQ_ExternalIdentity(Name)");

            Create.Table("UserExternalIdentity")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_UserExternalIdentity(Id)").Identity()
                .WithColumn("ExternalIdentityId").AsInt32().NotNullable()
                .ForeignKey("FK_ExternalIdentity(ExternalIdentityId)", "ExternalIdentity", "Id")
                .WithColumn("UserId").AsInt32().NotNullable()
                .ForeignKey("FK_ExternalIdentity(UserId)", "User", "Id")
                .WithColumn("ExternalIdentity").AsString(256).NotNullable();
            
            Create.UniqueConstraint("UQ_UserExternalIdentity(ExternalIdentityId)(ExternalIdentity)(UserId)")
                .OnTable("UserExternalIdentity")
                .Columns("ExternalIdentityId", "ExternalIdentity", "UserId");
        }
    }
}