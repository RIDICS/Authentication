using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(064)]
    public class M_064_UserDataLoA : ForwardOnlyMigration
    {
        public M_064_UserDataLoA()
        {
        }

        public override void Up()
        {
            Create.Table("LevelOfAssurance")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_LevelOfAssurance(Id)").Identity()
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("Level").AsInt32().NotNullable();

            Create.Table("DataSource")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_DataSource(Id)").Identity()
                .WithColumn("DataSource").AsString().NotNullable()
                .WithColumn("ExternalLoginProviderId").AsInt32().Nullable()
                .ForeignKey("FK_DataSource(ExternalLoginProviderId)", "ExternalLoginProvider", "Id");

            Create.UniqueConstraint("UQ_DataSource(DataSource)(ExternalLoginProviderId)")
                .OnTable("DataSource")
                .Columns("DataSource", "ExternalLoginProviderId");

            Alter.Table("UserData")
                .AddColumn("LevelOfAssuranceId").AsInt32().Nullable()
                .ForeignKey("FK_UserData(LevelOfAssuranceId)", "LevelOfAssurance", "Id")
                .AddColumn("DataSourceId").AsInt32().Nullable()
                .ForeignKey("FK_UserData(DataSourceId)", "DataSource", "Id");

            Alter.Table("UserContact")
                .AddColumn("LevelOfAssuranceId").AsInt32().Nullable()
                .ForeignKey("FK_UserContact(LevelOfAssuranceId)", "LevelOfAssurance", "Id")
                .AddColumn("DataSourceId").AsInt32().Nullable()
                .ForeignKey("FK_UserContact(DataSourceId)", "DataSource", "Id");
        }
    }
}
