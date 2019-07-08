using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    /// <summary>
    /// Fixed GUIDs are required for testing
    /// </summary>
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(077)]
    public class M_077_FixedTestUsersMasterUserId : ForwardOnlyMigration
    {
        public M_077_FixedTestUsersMasterUserId()
        {
        }

        public override void Up()
        {
            Execute.WithConnection((connection, transaction) =>
            {
                var idMuidDict = GetAndParseUsers(connection, transaction);

                foreach (var testUser in idMuidDict)
                {
                    Query.Conn(connection, transaction).Update("User")
                        .Set("MasterUserId", testUser.Value)
                        .Where("Id", testUser.Key)
                        .Run();
                }
            });
        }

        private Dictionary<string, string> GetUsernameMuidDict()
        {
            var usernameMuidStringDict = new Dictionary<string, string>
            {
                {"admin", "7BD3630E-E41E-43EE-82CB-55E4642C68D3"},
                {"portaladmin", "99E0D31A-A9B6-4B79-B56C-0B7C231AC723"},
            };

            return usernameMuidStringDict;
        }

        private Dictionary<int, Guid> GetAndParseUsers(IDbConnection connection, IDbTransaction transaction)
        {
            var usernameMuidStringDict = GetUsernameMuidDict();

            var idMuidDict = new Dictionary<int, Guid>();

            foreach (var user in usernameMuidStringDict)
            {
                var userId = Query.Conn(connection, transaction).Select<int>("Id").From("User")
                    .Where("Username", user.Key)
                    .Run().Single();

                var userMuid = Guid.Parse(user.Value);

                idMuidDict.Add(userId, userMuid);
            }

            return idMuidDict;
        }
    }
}