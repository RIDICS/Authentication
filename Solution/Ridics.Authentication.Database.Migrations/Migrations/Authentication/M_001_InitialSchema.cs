using FluentMigrator;
using Ridics.Authentication.Database.Migrations.Extensions;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(001)]
    public class M_001_InitialSchema : AutoReversingMigration
    {
        public M_001_InitialSchema()
        {
        }

        public override void Up()
        {
            Create.Table("User")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_User(Id)").Identity()
                .WithColumn("Username").AsString(50).NotNullable().Unique("UQ_User(Username)")
                .WithColumn("FirstName").AsString(50)
                .WithColumn("LastName").AsString(50)
                .WithColumn("PasswordHash").AsString().NotNullable()
                .WithColumn("SecurityStamp").AsString().Nullable()
                .WithColumn("TwoFactorEnabled").AsBoolean().NotNullable()
                .WithColumn("LockoutEndDateUtc").AsDateTime().Nullable()
                .WithColumn("LockoutEnabled").AsBoolean().NotNullable()
                .WithColumn("AccessFailedCount").AsInt32().NotNullable()
                .WithColumn("LastChange").AsDateTime().NotNullable()
                .WithColumn("TwoFactorProvider").AsString().Nullable();

            Create.Table("UserContact")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_UserContact(Id)").Identity()
                .WithColumn("UserId").AsInt32().NotNullable().ForeignKey("FK_UserContact(UserId)", "User", "Id")
                .WithColumn("Type").AsString(255).NotNullable()
                .WithColumn("CreateTime").AsDateTime().NotNullable()
                .WithColumn("Confirmed").AsBoolean()
                .WithColumn("ConfirmCode").AsString(255).Nullable()
                .WithColumn("ConfirmCodeChangeTime").AsDateTime().Nullable()
                .WithColumn("Value").AsString(255).NotNullable().Unique("UQ_UserContact(Value)");

            Create.Table("TwoFactorLogin")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_TwoFactorLogin(Id)").Identity()
                .WithColumn("UserId").AsInt32().NotNullable().ForeignKey("FK_TwoFactorLogin(UserId)", "User", "Id")
                .WithColumn("TokenProvider").AsString(255).NotNullable()
                .WithColumn("Token").AsString(50).NotNullable()
                .WithColumn("CreateTime").AsDateTime().NotNullable();

            Create.UniqueConstraint("UQ_TwoFactorLogin(UserId)(TokenProvider)")
                .OnTable("TwoFactorLogin")
                .Columns("UserId", "TokenProvider");

            Create.Table("Client")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_Client(Id)").Identity()
                .WithColumn("Name").AsString(50).NotNullable().Unique("UQ_Client(Name)")
                .WithColumn("Description").AsString(500).Nullable()
                .WithColumn("DisplayUrl").AsString(100)
                .WithColumn("LogoUrl").AsString(100)
                .WithColumn("RequireConsent").AsBoolean();

            Create.Table("Role")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_Role(Id)").Identity()
                .WithColumn("Name").AsString(50).NotNullable().Unique("UQ_Role(Name)");

            Create.Table("User_Role")
                .WithColumn("UserId").AsInt32().NotNullable().ForeignKey("FK_User_Role(UserId)", "User", "Id")
                .WithColumn("RoleId").AsInt32().NotNullable().ForeignKey("FK_User_Role(RoleId)", "Role", "Id");

            Create.UniqueConstraint("UQ_User_Role(UserId)(RoleId)")
                .OnTable("User_Role")
                .Columns("UserId", "RoleId");

            Create.Table("ClaimTypeEnum")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_ClaimTypeEnum(Id)").Identity()
                .WithColumn("Name").AsString(50).NotNullable().Unique("UQ_ClaimTypeEnum(Name)");

            Create.Table("ClaimType")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_ClaimType(Id)").Identity()
                .WithColumn("Name").AsString(50).NotNullable().Unique("UQ_ClaimType(Name)")
                .WithColumn("Description").AsString(500).Nullable()
                .WithColumn("ClaimTypeEnumId").AsInt32().ForeignKey("FK_ClaimType(ClaimTypeEnumId)", "ClaimTypeEnum", "Id");

            Create.Table("Claim")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_Claim(Id)").Identity()
                .WithColumn("Value").AsString(500).NotNullable()
                .WithColumn("UserId").AsInt32().ForeignKey("FK_Claim(UserId)", "User", "Id")
                .WithColumn("ClaimTypeId").AsInt32().ForeignKey("FK_Claim(ClaimTypeId)", "ClaimType", "Id");

            Create.UniqueConstraint("UQ_Claim(UserId)(ClaimTypeId")
                .OnTable("Claim")
                .Columns("UserId", "ClaimTypeId");

            Create.Table("Resource")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_Resource(Id)").Identity()
                .WithColumn("Name").AsString(50).NotNullable().Unique("UQ_Resource(Name)")
                .WithColumn("Description").AsString(500).Nullable()
                .WithColumn("Required").AsBoolean()
                .WithColumn("ShowInDiscoveryDocument").AsBoolean()
                .WithColumn("Discriminator").AsString(50).NotNullable();

            Create.Table("Resource_ClaimType")
                .WithColumn("ResourceId").AsInt32().NotNullable().ForeignKey("FK_Resource_ClaimType(ResourceId)", "Resource", "Id")
                .WithColumn("ClaimTypeId").AsInt32().NotNullable().ForeignKey("FK_Resource_ClaimType(ClaimTypeId)", "ClaimType", "Id");

            Create.UniqueConstraint("UQ_Resource_ClaimType(ResourceId)(ClaimTypeId)")
                .OnTable("Resource_ClaimType")
                .Columns("ResourceId", "ClaimTypeId");

            Create.Table("Secret")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_Secret(Id)").Identity()
                .WithColumn("Value").AsString(100).NotNullable()
                .WithColumn("Description").AsString(500).Nullable()
                .WithColumn("Expiration").AsDateTime().Nullable()
                .WithColumn("ResourceId").AsInt32().Nullable().ForeignKey("FK_Secret(ResourceId)", "Resource", "Id")
                .WithColumn("ClientId").AsInt32().Nullable().ForeignKey("FK_Secret(ClientId)", "Client", "Id")
                .WithColumn("Discriminator").AsString(50).NotNullable(); //TODO investigate COALESCE check


            Create.Table("Scope")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_Scope(Id)").Identity()
                .WithColumn("Name").AsString(50).NotNullable().Unique("UQ_Scope(Name)")
                .WithColumn("Description").AsString(500).Nullable()
                .WithColumn("Required").AsBoolean()
                .WithColumn("ShowInDiscoveryDocument").AsBoolean()
                .WithColumn("ResourceId").AsInt32().NotNullable().ForeignKey("FK_Scope(ResourceId)", "Resource", "Id");

            Create.Table("Scope_ClaimType")
                .WithColumn("ScopeId").AsInt32().NotNullable().ForeignKey("FK_Scope_ClaimType(ScopeId)", "Scope", "Id")
                .WithColumn("ClaimTypeId").AsInt32().NotNullable().ForeignKey("FK_Scope_ClaimType(ClaimTypeId)", "ClaimType", "Id");

            Create.UniqueConstraint("UQ_Scope_ClaimType(ScopeId)(ClaimTypeId)")
                .OnTable("Scope_ClaimType")
                .Columns("ScopeId", "ClaimTypeId");

            Create.Table("Client_ApiResource")
                .WithColumn("ClientId").AsInt32().NotNullable().ForeignKey("FK_Client_ApiResource(ClientId)", "Client", "Id")
                .WithColumn("ApiResourceId").AsInt32().NotNullable().ForeignKey("FK_Client_ApiResource(ApiResourceId)", "Resource", "Id");

            Create.UniqueConstraint("UQ_Client_ApiResource(ClientId)(ApiResourceId)")
                .OnTable("Client_ApiResource")
                .Columns("ClientId", "ApiResourceId");

            Create.Table("Client_IdentityResource")
                .WithColumn("ClientId").AsInt32().NotNullable().ForeignKey("FK_Client_IdentityResource(ClientId)", "Client", "Id")
                .WithColumn("IdentityResourceId").AsInt32().NotNullable()
                .ForeignKey("FK_Client_IdentityResource(IdentityResourceId)", "Resource", "Id");

            Create.UniqueConstraint("UQ_Client_IdentityResource(ClientId)(IdentityResourceId)")
                .OnTable("Client_IdentityResource")
                .Columns("ClientId", "IdentityResourceId");

            Create.Table("Uri")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_Uri(Id)").Identity()
                .WithColumn("Uri").AsString(100).NotNullable()
                .WithColumn("IsRedirect").AsBoolean()
                .WithColumn("IsPostLogoutRedirect").AsBoolean()
                .WithColumn("IsCorsOrigin").AsBoolean()
                .WithColumn("ClientId").AsInt32().NotNullable().ForeignKey("FK_Uri(ClientId)", "Client", "Id");

            Create.UniqueConstraint("UQ_Uri(Uri)(ClientId)")
                .OnTable("Uri")
                .Columns("Uri", "ClientId");

            Create.Table("GrantType")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_GrantType(Id)").Identity()
                .WithColumn("DisplayName").AsString(50)
                .WithColumn("Value").AsString(50).NotNullable().Unique("UQ_GrantType(Value)");

            Create.Table("Client_GrantType")
                .WithColumn("ClientId").AsInt32().NotNullable().ForeignKey("FK_Client_GrantType(ClientId)", "Client", "Id")
                .WithColumn("GrantTypeId").AsInt32().NotNullable().ForeignKey("FK_Client_GrantType(GrantTypeId)", "GrantType", "Id");

            Create.UniqueConstraint("UQ_Client_GrantType(ClientId)(GrantTypeId)")
                .OnTable("Client_GrantType")
                .Columns("ClientId", "GrantTypeId");

            Create.Table("PersistedGrant")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_PersistedGrant(Id)").Identity()
                .WithColumn("Key").AsString(100).NotNullable().Unique("UQ_PersistedGrant(Key)")
                .WithColumn("Type").AsString(50)
                .WithColumn("UserId").AsInt32().NotNullable().ForeignKey("FK_PersistedGrant(UserId)", "User", "Id")
                .WithColumn("ClientId").AsInt32().NotNullable().ForeignKey("FK_PersistedGrant(ClientId)", "Client", "Id")
                .WithColumn("CreationTime").AsDateTime()
                .WithColumn("ExpirationTime").AsDateTime()
                .WithColumn("Data").AsMaxString();
        }
    }
}
