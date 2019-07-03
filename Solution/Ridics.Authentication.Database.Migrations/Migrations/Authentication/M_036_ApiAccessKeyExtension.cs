using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(036)]
    public class M_036_ApiAccessKeyExtension : ForwardOnlyMigration
    {
        public M_036_ApiAccessKeyExtension()
        {
        }

        //TODO decompose to structure and data migration
        public override void Up()
        {
            // Add Name column to distinguish keys
            Alter.Table("ApiAccessKey")
                .AddColumn("Name").AsString(100).Nullable();

            Update.Table("ApiAccessKey").Set(new { Name = "Vokabular" }).AllRows();

            Alter.Column("Name").OnTable("ApiAccessKey").AsString(100).NotNullable();

            // Table for permissions
            Create.Table("ApiAccessKeyPermission")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_ApiAccessKeyPermission(Id)").Identity()
                .WithColumn("ApiAccessKeyId").AsInt32().NotNullable().ForeignKey("FK_ApiAccessKeyPermission(ApiAccessKeyId)", "ApiAccessKey", "Id")
                .WithColumn("Permission").AsInt32().NotNullable();

            Execute.WithConnection((connection, transaction) =>
            {
                var vokabularKeyId = Query.Conn(connection, transaction).Select<int>("Id").From("ApiAccessKey")
                    .Where("Name", "Vokabular")
                    .Run().Single();

                var internalAccessPermission = 1;
                var userActivationPermission = 2;

                Query.Conn(connection, transaction).Insert("ApiAccessKeyPermission")
                    .Row(new {ApiAccessKeyId = vokabularKeyId, Permission = internalAccessPermission})
                    .Row(new {ApiAccessKeyId = vokabularKeyId, Permission = userActivationPermission})
                    .Run();
            });
        }
    }
}
