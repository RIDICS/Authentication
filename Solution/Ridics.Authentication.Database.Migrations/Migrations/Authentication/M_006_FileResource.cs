using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(006)]
    public class M_006_FileResource : AutoReversingMigration
    {
        readonly IScriptPathResolver m_scriptPathResolver;

        public M_006_FileResource(IScriptPathResolver scriptPathResolver)
        {
            m_scriptPathResolver = scriptPathResolver;
        }

        public override void Up()
        {
            Create.Table("FileResource")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_FileResource(Id)").Identity()
                .WithColumn("Guid").AsGuid().NotNullable().Unique("UQ_FileResource(Guid)")
                .WithColumn("Type").AsString(20).NotNullable().WithDefaultValue("External")
                .WithColumn("FileExtension").AsString(20).NotNullable();
        }
    }
}