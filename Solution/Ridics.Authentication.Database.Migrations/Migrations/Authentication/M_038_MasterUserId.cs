using System;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(038)]
    public class M_038_MasterUserId : ForwardOnlyMigration
    {
        public M_038_MasterUserId()
        {
        }

        public override void Up()
        {
            Alter.Table("User").AddColumn("MasterUserId").AsGuid().Nullable();

            Execute.WithConnection((connection, transaction) =>
            {
                var userIdList = Query.Conn(connection, transaction).Select<int>("Id").From("User")
                    .Run();

                foreach (var userId in userIdList)
                {
                    var masterUserGuid = Guid.NewGuid();

                    Query.Conn(connection, transaction).Update("User")
                        .Set("MasterUserId", masterUserGuid)
                        .Where("Id", userId)
                        .Run();
                }
            });

            Alter.Column("MasterUserId").OnTable("User").AsGuid().NotNullable().Unique("UQ_User(MasterUserId)");
        }
    }
}