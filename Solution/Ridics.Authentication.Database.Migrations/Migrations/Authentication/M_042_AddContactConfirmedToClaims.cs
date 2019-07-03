using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(042)]
    public class M_042_AddContactConfirmedToClaims : ForwardOnlyMigration
    {
        public M_042_AddContactConfirmedToClaims()
        {
        }

        public override void Up()
        {
            Execute.WithConnection((connection, transaction) =>
            {
                var boolClaimTypeId = Query.Conn(connection, transaction).Select<int>("Id").From("ClaimTypeEnum")
                    .Where("Name", "Boolean")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("ClaimType")
                    .Row(new {Name = "phone_number_confirmed", Description = "Phone number confirmed flag", ClaimTypeEnumId = boolClaimTypeId})
                    .Row(new {Name = "email_confirmed", Description = "Email confirmed flag", ClaimTypeEnumId = boolClaimTypeId})
                    .Run();

                var resourceProfileId = Query.Conn(connection, transaction).Select<int>("Id").From("Resource")
                    .Where("Name", "profile")
                    .Run().Single();

                var phoneNumberConfirmedClaimTypeId = Query.Conn(connection, transaction).Select<int>("Id").From("ClaimType")
                    .Where("Name", "phone_number_confirmed")
                    .Run().Single();

                var emailConfirmedClaimTypeId = Query.Conn(connection, transaction).Select<int>("Id").From("ClaimType")
                    .Where("Name", "email_confirmed")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("Resource_ClaimType")
                    .Row(new {ResourceId = resourceProfileId, ClaimTypeId = phoneNumberConfirmedClaimTypeId})
                    .Row(new {ResourceId = resourceProfileId, ClaimTypeId = emailConfirmedClaimTypeId})
                    .Run();
            });
        }
    }
}