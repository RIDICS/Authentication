using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(033)]
    public class M_033_AddDataToClaims : ForwardOnlyMigration
    {
        public M_033_AddDataToClaims()
        {
        }

        public override void Up()
        {
            Execute.WithConnection((connection, transaction) =>
            {
                var stringClaimTypeId = Query.Conn(connection, transaction).Select<int>("Id").From("ClaimTypeEnum")
                    .Where("Name", "String")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("ClaimType")
                    .Row(new {Name = "phone_number", Description = "Phone number", ClaimTypeEnumId = stringClaimTypeId})
                    .Row(new {Name = "given_name", Description = "First name", ClaimTypeEnumId = stringClaimTypeId})
                    .Row(new {Name = "family_name", Description = "Last name", ClaimTypeEnumId = stringClaimTypeId})
                    .Run();

                var resourceProfileId = Query.Conn(connection, transaction).Select<int>("Id").From("Resource")
                    .Where("Name", "profile")
                    .Run().Single();
                
                var phoneNumberClaimTypeId = Query.Conn(connection, transaction).Select<int>("Id").From("ClaimType")
                    .Where("Name", "phone_number")
                    .Run().Single();

                var givenNameClaimTypeId = Query.Conn(connection, transaction).Select<int>("Id").From("ClaimType")
                    .Where("Name", "given_name")
                    .Run().Single();

                var familyNameClaimTypeId = Query.Conn(connection, transaction).Select<int>("Id").From("ClaimType")
                    .Where("Name", "family_name")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("Resource_ClaimType")
                    .Row(new {ResourceId = resourceProfileId, ClaimTypeId = phoneNumberClaimTypeId})
                    .Row(new {ResourceId = resourceProfileId, ClaimTypeId = givenNameClaimTypeId})
                    .Row(new {ResourceId = resourceProfileId, ClaimTypeId = familyNameClaimTypeId})
                    .Run();
            });
        }
    }
}