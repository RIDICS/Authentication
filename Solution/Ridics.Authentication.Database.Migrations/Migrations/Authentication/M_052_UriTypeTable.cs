using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(052)]
    public class M_052_UriTypeTable : ForwardOnlyMigration
    {
        public M_052_UriTypeTable()
        {
        }

        public override void Up()
        {
            Create.Table("UriType")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_UriType(Id)").Identity()
                .WithColumn("Value").AsString(255).NotNullable().Unique("UQ_UriType(Value)");

            Create.Table("Uri_UriType")
                .WithColumn("UriId").AsInt32().NotNullable().ForeignKey("FK_Uri_UriType(UriId)", "Uri", "Id")
                .WithColumn("UriTypeId").AsInt32().NotNullable().ForeignKey("FK_Uri_UriType(UriTypeId)", "UriType", "Id");

            Create.UniqueConstraint("UQ_Uri_UriType(UriId)(UriTypeId)")
                .OnTable("Uri_UriType")
                .Columns("UriId", "UriTypeId");
        }
    }
}