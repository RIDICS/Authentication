using System;
using System.Data;
using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(087)]
    public class M_087_UserPropertiesToUserData : ForwardOnlyMigration
    {
        public M_087_UserPropertiesToUserData()
        {
        }

        //TODO decompose to structure and data migration
        public override void Up()
        {
            Execute.WithConnection((connection, transaction) =>
            {
                var users = Query.Conn(connection, transaction).Select<User>().From("User")
                    .Run().ToList();

                if (!users.Any())
                {
                    Console.WriteLine("No users to migrate");
                    return; //No users, no data to migrate
                }

                int firstNameDataType;
                int lastNameDataType;
                int masterUserIdDataType;
                int secondNameDataType;
                int userDataSourceId;
                int systemDataSourceId;
                int lowLevelOfAssurance;

                try
                {
                    //Single throws exception when list does not contain exactly one element, so null reference cannot happen
                    firstNameDataType = Query.Conn(connection, transaction).Select<int>("Id").From("UserDataType")
                        .Where("DataTypeValue", "FirstName")
                        .Run().SingleOrDefault();

                    lastNameDataType = Query.Conn(connection, transaction).Select<int>("Id").From("UserDataType")
                        .Where("DataTypeValue", "LastName")
                        .Run().Single();

                    masterUserIdDataType = Query.Conn(connection, transaction).Select<int>("Id").From("UserDataType")
                        .Where("DataTypeValue", "MasterUserId")
                        .Run().Single();

                    secondNameDataType = Query.Conn(connection, transaction).Select<int>("Id").From("UserDataType")
                        .Where("DataTypeValue", "SecondName")
                        .Run().Single();

                    userDataSourceId = Query.Conn(connection, transaction).Select<int>("Id").From("DataSource")
                        .Where("DataSource", "User")
                        .Run().Single();

                    systemDataSourceId = Query.Conn(connection, transaction).Select<int>("Id").From("DataSource")
                        .Where("DataSource", "System")
                        .Run().Single();

                    lowLevelOfAssurance = Query.Conn(connection, transaction).Select<int>("Id")
                        .From("LevelOfAssurance")
                        .Where("Name", "Low")
                        .Run().Single();
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("Could not find proper data type but also Users table is not empty. Check if all data types required for this migration are presented in DB and rerun this migration.");
                    throw;
                }
                

                var insertQuery = Query.Conn(connection, transaction).Insert("UserData");

                foreach (var user in users)
                {
                    // The most obvious way how to determine missing data for new userdata types (firstname, lastname, masteruserid)
                    // is to copy them from existing user data or use default
                    var otherUserData = GetUserDataOfUser(connection, transaction, user.Id, secondNameDataType) ?? new UserData
                    {
                        VerifiedBy = user.Id,
                        LevelOfAssuranceId = lowLevelOfAssurance,
                        DataSourceId = userDataSourceId,
                        ActiveFrom = new DateTime(2019, 1, 1)
                    };
                    insertQuery
                        .Row(new
                        {
                            UserId = user.Id,
                            Value = user.FirstName,
                            TypeId = firstNameDataType,
                            VerifiedBy = otherUserData.VerifiedBy,
                            LevelOfAssuranceId = otherUserData.LevelOfAssuranceId,
                            DataSourceId = otherUserData.DataSourceId,
                            ActiveFrom = otherUserData.ActiveFrom
                        })
                        .Row(new
                        {
                            UserId = user.Id,
                            Value = user.LastName,
                            TypeId = lastNameDataType,
                            VerifiedBy = otherUserData.VerifiedBy,
                            LevelOfAssuranceId = otherUserData.LevelOfAssuranceId,
                            DataSourceId = otherUserData.DataSourceId,
                            ActiveFrom = otherUserData.ActiveFrom
                        })
                        .Row(new
                        {
                            UserId = user.Id,
                            Value = user.MasterUserId.ToString(),
                            TypeId = masterUserIdDataType,
                            VerifiedBy = otherUserData.VerifiedBy,
                            LevelOfAssuranceId = otherUserData.LevelOfAssuranceId,
                            DataSourceId = systemDataSourceId, // MUID is generated by system
                            ActiveFrom = otherUserData.ActiveFrom
                        });
                }

                insertQuery.Run();
            });

            Delete.Index("UQ_User(MasterUserId)").OnTable("User");

            Delete.Column("FirstName").FromTable("User");
            Delete.Column("LastName").FromTable("User");
            Delete.Column("MasterUserId").FromTable("User");
        }

        private UserData GetUserDataOfUser(IDbConnection connection, IDbTransaction transaction, int userId, int dataTypeId)
        {
            var userData = Query.Conn(connection, transaction).Select<UserData>().From("UserData")
                .Where("UserId", userId)
                .Where("TypeId", dataTypeId)
                .Run().SingleOrDefault();

            return userData;
        }

        private class User
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public Guid MasterUserId { get; set; }
        }

        private class UserData
        {
            public int Id { get; set; }
            public int? VerifiedBy { get; set; }
            public int LevelOfAssuranceId { get; set; }
            public int DataSourceId { get; set; }
            public DateTime ActiveFrom { get; set; }
            public DateTime? ActiveTo { get; set; }
        }
    }

    
}