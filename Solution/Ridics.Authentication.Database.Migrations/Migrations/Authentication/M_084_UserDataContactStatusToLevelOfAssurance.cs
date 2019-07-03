using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(084)]
    public class M_084_UserDataContactStatusToLevelOfAssurance : ForwardOnlyMigration
    {
        public M_084_UserDataContactStatusToLevelOfAssurance()
        {
        }

        public override void Up()
        {
            Execute.WithConnection((connection, transaction) =>
            {
               var lowLevelOfAssurance = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("LevelOfAssurance")
                    .Where("Name", "Low")
                    .Run().Single();

                var highLevelOfAssurance = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("LevelOfAssurance")
                    .Where("Name", "High")
                    .Run().Single();

                Query.Conn(connection, transaction).Update("UserData")
                        .Set("LevelOfAssuranceId", highLevelOfAssurance)
                        .Where("IsVerified", true)
                        .Run();

                Query.Conn(connection, transaction).Update("UserData")
                        .Set("LevelOfAssuranceId", lowLevelOfAssurance)
                        .Where("IsVerified", false)
                        .Run();

                Query.Conn(connection, transaction).Update("UserContact")
                    .Set("LevelOfAssuranceId", highLevelOfAssurance)
                    .Where("Confirmed", true)
                    .Run();

                Query.Conn(connection, transaction).Update("UserContact")
                    .Set("LevelOfAssuranceId", lowLevelOfAssurance)
                    .Where("Confirmed", false)
                    .Run();
            });
        }
    }
}