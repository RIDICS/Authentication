using System.Collections.Generic;
using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(044)]
    public class M_044_SetConfirmContactTestUsers : ForwardOnlyMigration
    {
        public M_044_SetConfirmContactTestUsers()
        {
        }

        public override void Up()
        {
            Execute.WithConnection((connection, transaction) =>
            {
                var bobUserId = Query.Conn(connection, transaction).Select<int>("Id").From("User")
                    .Where("Username", "admin").Run().FirstOrDefault();

                var johnUserId = Query.Conn(connection, transaction).Select<int>("Id").From("User")
                    .Where("Username", "portaladmin").Run().FirstOrDefault();

                var userIdList = new List<int>
                {
                    bobUserId,
                    johnUserId,
                };

                foreach (var userId in userIdList)
                {
                    Query.Conn(connection, transaction).Update("UserContact")
                        .Set("Confirmed", true)
                        .Where("UserId", userId)
                        .Run();
                }
            });
        }
    }
}