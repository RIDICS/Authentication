using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(009)]
    public class M_009_ExternalLoginProviderData : AutoReversingMigration
    {
        readonly IScriptPathResolver m_scriptPathResolver;

        public M_009_ExternalLoginProviderData(IScriptPathResolver scriptPathResolver)
        {
            m_scriptPathResolver = scriptPathResolver;
        }

        public override void Up()
        {
        }
    }
}