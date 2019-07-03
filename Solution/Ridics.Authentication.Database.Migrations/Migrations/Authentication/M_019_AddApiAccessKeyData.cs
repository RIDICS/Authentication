using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(019)]
    public class M_019_AddApiAccessKeyData : ForwardOnlyMigration
    {
        public M_019_AddApiAccessKeyData()
        {
        }

        public override void Up()
        {
            Insert.IntoTable("ApiAccessKey")
                .Row(new
                {
                    ApiKeyHash = "2BB80D537B1DA3E38BD30361AA855686BDE0EACD7162FEF6A25FE97BF527A25B", // API key is 'secret'
                    HashAlgorithm = "sha256"
                });
        }
    }
}