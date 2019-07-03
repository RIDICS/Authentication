using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(062)]
    public class M_062_DynamicModuleConfiguration : ForwardOnlyMigration
    {
        public M_062_DynamicModuleConfiguration()
        {
        }

        public override void Up()
        {
            Create.Table("DynamicModule")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_DynamicModule(Id)").Identity()
                .WithColumn("ModuleGuid").AsGuid().NotNullable()
                .WithColumn("Name").AsString().Nullable()
                .WithColumn("ConfigurationVersion").AsString(20).NotNullable()
                .WithColumn("Configuration").AsString().Nullable()
                .WithColumn("ConfigurationChecksum").AsString(64).NotNullable();

            Create.Table("DynamicModuleBlob")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_DynamicModuleBlob(Id)").Identity()
                .WithColumn("DynamicModuleId").AsInt32().NotNullable()
                .ForeignKey("FK_DynamicModuleBlob(DynamicModuleId)", "DynamicModule", "Id")
                .WithColumn("Type").AsString(60).NotNullable()
                .WithColumn("FileId").AsInt32().NotNullable()
                .ForeignKey("FK_DynamicModuleBlob(FileId)", "FileResource", "Id")
                .WithColumn("LastChange").AsDateTime().NotNullable();

            Alter.Table("ExternalLoginProvider")
                .AddColumn("DynamicModuleId").AsInt32().Nullable()
                .ForeignKey("FK_ExternalLoginProvider(DynamicModuleId)", "DynamicModule", "Id");
        }
    }
}
