using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(059)]
    public class M_059_ResourcePermissionActions : ForwardOnlyMigration
    {
        public M_059_ResourcePermissionActions()
        {
        }

        public override void Up()
        {
            Create.Table("ResourcePermissionTypeAction")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_ResourcePermissionTypeAction(Id)").Identity()
                .WithColumn("Name").AsString(50).NotNullable()
                .WithColumn("Description").AsString(500).Nullable()
                .WithColumn("ResourcePermissionTypeId").AsInt32().NotNullable()
                .ForeignKey("FK_ResourcePermissionTypeAction(ResourcePermissionType)", "ResourcePermissionType", "Id");

            Create.UniqueConstraint("UQ_ResourcePermissionTypeAction(Name)(ResourcePermissionTypeId)")
                .OnTable("ResourcePermissionTypeAction")
                .Columns("Name", "ResourcePermissionTypeId");
            
            Delete.UniqueConstraint("UQ_ResourcePermission(ResourceId)(ResourcePermissionTypeId)").FromTable("ResourcePermission");
            Delete.ForeignKey("FK_ResourcePermission(ResourcePermissionType)").OnTable("ResourcePermission");
            Delete.Column("ResourcePermissionTypeId").FromTable("ResourcePermission");

            Alter.Table("ResourcePermission")
                .AddColumn("ResourcePermissionTypeActionId").AsInt32().NotNullable()
                .ForeignKey("FK_ResourcePermission(ResourcePermissionTypeAction)", "ResourcePermissionTypeAction", "Id");

            Create.UniqueConstraint("UQ_ResourcePermission(ResourceId)(ResourcePermissionTypeActionId)")
                .OnTable("ResourcePermission")
                .Columns("ResourceId", "ResourcePermissionTypeActionId");

            Delete.UniqueConstraint("UQ_User_ResourcePermissionType(UserId)(ResourcePermissionTypeId)").FromTable("User_ResourcePermissionType");

            Delete.Table("User_ResourcePermissionType");

            Create.Table("User_ResourcePermissionTypeAction")
                .WithColumn("UserId").AsInt32().NotNullable()
                .ForeignKey("FK_User_ResourcePermissionTypeAction(UserId)", "User", "Id")
                .WithColumn("ResourcePermissionTypeActionId").AsInt32().NotNullable()
                .ForeignKey("FK_User_ResourcePermissionTypeAction(ResourcePermissionTypeActionId)", "ResourcePermissionTypeAction", "Id");

            Create.UniqueConstraint("UQ_User_ResourcePermissionTypeAction(UserId)(ResourcePermissionTypeActionId)")
                .OnTable("User_ResourcePermissionTypeAction")
                .Columns("UserId", "ResourcePermissionTypeActionId");

            Delete.UniqueConstraint("UQ_Role_ResourcePermissionType(RoleId)(ResourcePermissionTypeId)").FromTable("Role_ResourcePermissionType");

            Delete.Table("Role_ResourcePermissionType");

            Create.Table("Role_ResourcePermissionTypeAction")
                .WithColumn("RoleId").AsInt32().NotNullable()
                .ForeignKey("FK_Role_ResourcePermissionTypeAction(RoleId)", "Role", "Id")
                .WithColumn("ResourcePermissionTypeActionId").AsInt32().NotNullable()
                .ForeignKey("FK_Role_ResourcePermissionTypeAction(ResourcePermissionTypeActionId)", "ResourcePermissionTypeAction", "Id");

            Create.UniqueConstraint("UQ_Role_ResourcePermissionTypeAction(RoleId)(ResourcePermissionTypeActionId)")
                .OnTable("Role_ResourcePermissionTypeAction")
                .Columns("RoleId", "ResourcePermissionTypeActionId");
        }
    }
}