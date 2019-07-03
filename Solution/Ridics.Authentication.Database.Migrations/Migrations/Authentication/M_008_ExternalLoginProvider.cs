using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(008)]
    public class M_008_ExternalLoginProvider : AutoReversingMigration
    {
        readonly IScriptPathResolver m_scriptPathResolver;

        public M_008_ExternalLoginProvider(IScriptPathResolver scriptPathResolver)
        {
            m_scriptPathResolver = scriptPathResolver;
        }

        public override void Up()
        {
            Create.Table("ExternalLoginProvider")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_ExternalLoginProvider(Id)").Identity()
                .WithColumn("Name").AsString(50).NotNullable().Unique("UQ_ExternalLoginProvider(Name)")
                .WithColumn("DisplayName").AsString(500).NotNullable()
                .WithColumn("LogoId").AsInt32().Nullable()
                .ForeignKey("FK_ExternalLoginProvider(LogoId)", "FileResource", "Id")
                .WithColumn("MainColor").AsString(6).Nullable()
                .WithColumn("CreateNotExistUser").AsBoolean().WithDefaultValue(false)
                .WithColumn("DisableManagingByUser").AsBoolean().NotNullable().WithDefaultValue(false);

            Create.Table("ExternalLogin")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_ExternalLogin(Id)").Identity()
                .WithColumn("ProviderId").AsInt32().NotNullable()
                .ForeignKey("FK_ExternalLogin(ProviderId)", "ExternalLoginProvider", "Id")
                .WithColumn("UserId").AsInt32().NotNullable()
                .ForeignKey("FK_ExternalLogin(UserId)", "User", "Id")
                .WithColumn("ProviderKey").AsString(256).NotNullable();

            Create.UniqueConstraint("UQ_ExternalLoginProvider(ProviderId)(UserId)")
                .OnTable("ExternalLogin")
                .Columns("ProviderId", "UserId");
            Create.UniqueConstraint("UQ_ExternalLoginProvider(ProviderId)(ProviderKey)")
                .OnTable("ExternalLogin")
                .Columns("ProviderId", "ProviderKey");
        }
    }
}