using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(024)]
    public class M_024_UserDataSchema : ForwardOnlyMigration
    {
        public M_024_UserDataSchema()
        {
        }

        public override void Up()
        {
            Create.Table("UserDataType")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_UserDataType(Id)").Identity()
                .WithColumn("DataTypeValue").AsString(255).NotNullable().Unique("UQ_UserDataType(DataTypeValue)");

            Create.Table("Country")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_Country(Id)").Identity()
                .WithColumn("Iso2Code").AsString(2).NotNullable().Unique("UQ_Country(Iso2Code)");

            Create.Table("UserData")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_UserData(Id)").Identity()
                .WithColumn("UserId").AsInt32().NotNullable().ForeignKey("FK_UserData(UserId)", "User", "Id")
                .WithColumn("Value").AsString(255).Nullable()
                .WithColumn("TypeId").AsInt32().NotNullable().ForeignKey("FK_UserData(TypeId)", "UserDataType", "Id")
                .WithColumn("IsVerified").AsBoolean().NotNullable()
                .WithColumn("VerifiedWhen").AsDateTime().Nullable()
                .WithColumn("VerifiedBy").AsInt32().Nullable().ForeignKey("FK_UserData(VerifiedBy)", "User", "Id")
                .WithColumn("DataParentId").AsInt32().Nullable().ForeignKey("FK_UserData(DataParentId)", "UserData", "Id");
        }
    }
}