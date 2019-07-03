using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(071)]
    public class M_071_ConvertUserTitleToEnum : ForwardOnlyMigration
    {
        public M_071_ConvertUserTitleToEnum()
        {
        }

        public override void Up()
        {
            Execute.WithConnection((connection, transaction) =>
            {
                var userTitleTypeId = Query.Conn(connection, transaction).Select<int>("Id").From("UserDataType")
                    .Where("DataTypeValue", "Title")
                    .Run().Single();

                var userTitles = Query.Conn(connection, transaction).Select<UserDataResult>().From("UserData")
                    .Where("TypeId", userTitleTypeId)
                    .Run().ToList();

                userTitles.ForEach(x =>
                {
                    Query.Conn(connection, transaction).Update("UserData").Set("Value", ConvertTitleStringToEnum(x.Value))
                        .Where("Id", x.Id).Run();
                });
            });
        }

        private string ConvertTitleStringToEnum(string title)
        {
            UserAddressingWays convertedEnumValue;
            switch (title)
            {
                case "":
                    convertedEnumValue = UserAddressingWays.None;
                    break;
                case "mr":
                    convertedEnumValue = UserAddressingWays.Mr;
                    break;
                case "ms":
                    convertedEnumValue = UserAddressingWays.Mrs;
                    break;
                case "miss":
                    convertedEnumValue = UserAddressingWays.Miss;
                    break;
                default:
                    convertedEnumValue = UserAddressingWays.None;
                    break;
            }

            return convertedEnumValue.ToString("G");
        }

        private enum UserAddressingWays
        {
            None = 0,
            Mr = 1,
            Mrs = 2,
            Miss = 3,
        }

        private class UserDataResult
        {
            public int Id { get; set; }
            public string Value { get; set; }
        }
    }
}