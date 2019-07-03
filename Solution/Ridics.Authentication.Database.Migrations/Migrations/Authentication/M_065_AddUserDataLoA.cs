using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(065)]
    public class M_065_AddUserDataLoA : ForwardOnlyMigration
    {
        public M_065_AddUserDataLoA()
        {
        }

        public override void Up()
        {
            Insert.IntoTable("LevelOfAssurance")
                .Row(new {Level = 5, Name = "Low"})
                .Row(new {Level = 10, Name = "Substantial"})
                .Row(new {Level = 15, Name = "High"});

            Insert.IntoTable("DataSource")
                .Row(new {DataSource = "User"});

            Execute.WithConnection((connection, transaction) =>
            {
                var externalLoginProviderIds = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("ExternalLoginProvider")
                    .Run();

                foreach (var externalLoginProviderId in externalLoginProviderIds)
                {
                    Query.Conn(connection, transaction).Insert("DataSource")
                        .Row(new {DataSource = "ExternalLoginProvider", ExternalLoginProviderId = externalLoginProviderId})
                        .Run();
                }

                var lowLevelOfAssurance = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("LevelOfAssurance")
                    .Where("Name", "Low")
                    .Run().Single();

                var highLevelOfAssurance = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("LevelOfAssurance")
                    .Where("Name", "High")
                    .Run().Single();

                var userDataSource = Query.Conn(connection, transaction).Select<int>("Id")
                    .From("DataSource")
                    .Where("DataSource", "User")
                    .Run().Single();

                var userData = Query.Conn(connection, transaction).Select<UserData>()
                    .From("UserData")
                    .Run();

                foreach (var data in userData)
                {
                    Query.Conn(connection, transaction).Update("UserData")
                        .Set("LevelOfAssuranceId", data.IsVerified ? highLevelOfAssurance : lowLevelOfAssurance)
                        .Set("DataSourceId", userDataSource)
                        .Where("Id", data.Id)
                        .Run();
                }

                var userContact = Query.Conn(connection, transaction).Select<UserContact>()
                    .From("UserContact")
                    .Run();

                foreach (var data in userContact)
                {
                    Query.Conn(connection, transaction).Update("UserContact")
                        .Set("LevelOfAssuranceId", data.Confirmed ? highLevelOfAssurance : lowLevelOfAssurance)
                        .Set("DataSourceId", userDataSource)
                        .Where("Id", data.Id)
                        .Run();
                }
            });
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class UserData
        {
            public int Id { get; }

            public bool IsVerified { get; }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class UserContact
        {
            public int Id { get; }

            public bool Confirmed { get; }
        }
    }
}
