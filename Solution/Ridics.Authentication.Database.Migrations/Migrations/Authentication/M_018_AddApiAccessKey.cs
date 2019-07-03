using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(018)]
    public class M_018_AddApiAccessKey : ForwardOnlyMigration
    {
        public M_018_AddApiAccessKey()
        {
        }

        public override void Up()
        {
            Create.Table("ApiAccessKey")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_ApiAccessKey(Id)").Identity()
                .WithColumn("ApiKeyHash").AsString(80).NotNullable().Unique("UQ_ApiAccessKey(ApiKeyHash)")
                .WithColumn("HashAlgorithm").AsString(16).NotNullable();
        }
    }
}