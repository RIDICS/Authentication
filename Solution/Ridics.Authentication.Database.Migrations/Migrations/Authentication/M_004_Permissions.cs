using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(004)]
    public class M_004_Permissions : AutoReversingMigration
    {
        public M_004_Permissions()
        {
        }

        public override void Up()
        {
            Alter.Table("Role")
                .AddColumn("Description").AsString(500).Nullable();

            Create.Table("Permission")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_Permission(Id)").Identity()
                .WithColumn("Name").AsString(50).NotNullable().Unique("UQ_Permission(Name)")
                .WithColumn("Description").AsString(500).Nullable();

            Create.Table("Role_Permission")
                .WithColumn("RoleId").AsInt32().NotNullable().ForeignKey("FK_Role_Permission(RoleId)", "Role", "Id")
                .WithColumn("PermissionId").AsInt32().NotNullable().ForeignKey("FK_Role_Permission(PermissionId)", "Permission", "Id");

            Create.UniqueConstraint("UQ_Role_Permission(RoleId)(PermissionId)")
                .OnTable("Role_Permission")
                .Columns("RoleId", "PermissionId");

        }
    }
}