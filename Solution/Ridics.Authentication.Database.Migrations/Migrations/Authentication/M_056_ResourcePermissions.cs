using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(056)]
    public class M_056_ResourcePermissions : AutoReversingMigration
    {
        public M_056_ResourcePermissions()
        {
        }

        public override void Up()
        {
            Create.Table("ResourcePermissionType")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_ResourcePermissionType(Id)").Identity()
                .WithColumn("Name").AsString(50).NotNullable().Unique("UQ_ResourcePermissionType(Name)")
                .WithColumn("Description").AsString(500).Nullable();

            Create.Table("ResourcePermission")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_ResourcePermission(Id)").Identity()
                .WithColumn("ResourceId").AsString(50).NotNullable()
                .WithColumn("ResourcePermissionTypeId").AsInt32().NotNullable()
                .ForeignKey("FK_ResourcePermission(ResourcePermissionType)", "ResourcePermissionType", "Id");

            Create.UniqueConstraint("UQ_ResourcePermission(ResourceId)(ResourcePermissionTypeId)")
                .OnTable("ResourcePermission")
                .Columns("ResourceId", "ResourcePermissionTypeId");

            Create.Table("User_ResourcePermission")
                .WithColumn("UserId").AsInt32().NotNullable()
                .ForeignKey("FK_User_ResourcePermission(UserId)", "User", "Id")
                .WithColumn("ResourcePermissionId").AsInt32().NotNullable()
                .ForeignKey("FK_User_ResourcePermission(ResourcePermissionId)", "ResourcePermission", "Id");

            Create.UniqueConstraint("UQ_User_ResourcePermission(UserId)(ResourcePermissionId)")
                .OnTable("User_ResourcePermission")
                .Columns("UserId", "ResourcePermissionId");

            Create.Table("User_ResourcePermissionType")
                .WithColumn("UserId").AsInt32().NotNullable()
                .ForeignKey("FK_User_ResourcePermissionType(UserId)", "User", "Id")
                .WithColumn("ResourcePermissionTypeId").AsInt32().NotNullable()
                .ForeignKey("FK_User_ResourcePermissionType(ResourcePermissionTypeId)", "ResourcePermissionType", "Id");

            Create.UniqueConstraint("UQ_User_ResourcePermissionType(UserId)(ResourcePermissionTypeId)")
                .OnTable("User_ResourcePermissionType")
                .Columns("UserId", "ResourcePermissionTypeId");

            Create.Table("Role_ResourcePermission")
                .WithColumn("RoleId").AsInt32().NotNullable()
                .ForeignKey("FK_Role_ResourcePermission(RoleId)", "Role", "Id")
                .WithColumn("ResourcePermissionId").AsInt32().NotNullable()
                .ForeignKey("FK_Role_ResourcePermission(ResourcePermissionId)", "ResourcePermission", "Id");

            Create.UniqueConstraint("UQ_Role_ResourcePermission(RoleId)(ResourcePermissionId)")
                .OnTable("Role_ResourcePermission")
                .Columns("RoleId", "ResourcePermissionId");

            Create.Table("Role_ResourcePermissionType")
                .WithColumn("RoleId").AsInt32().NotNullable()
                .ForeignKey("FK_Role_ResourcePermissionType(RoleId)", "Role", "Id")
                .WithColumn("ResourcePermissionTypeId").AsInt32().NotNullable()
                .ForeignKey("FK_Role_ResourcePermissionType(ResourcePermissionTypeId)", "ResourcePermissionType", "Id");

            Create.UniqueConstraint("UQ_Role_ResourcePermissionType(RoleId)(ResourcePermissionTypeId)")
                .OnTable("Role_ResourcePermissionType")
                .Columns("RoleId", "ResourcePermissionTypeId");
        }
    }
}