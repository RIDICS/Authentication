using System;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Structure, MigrationTypeTagTypes.All)]
    [Migration(085)]
    public class M_085_UserDataContactsVersioning : ForwardOnlyMigration
    {
        private readonly DateTime m_defaultActiveFrom = new DateTime(2019,01,01,0,0,0, DateTimeKind.Utc);

        public M_085_UserDataContactsVersioning()
        {
        }

        public override void Up()
        {
            Delete.Column("IsVerified").FromTable("UserData");
            Delete.Column("Confirmed").FromTable("UserContact");

            Alter.Table("UserData").AddColumn("ActiveFrom").AsDateTime().NotNullable().SetExistingRowsTo(m_defaultActiveFrom);
            Alter.Table("UserData").AddColumn("ActiveTo").AsDateTime().Nullable();

            Alter.Table("UserContact").AddColumn("ActiveFrom").AsDateTime().NotNullable().SetExistingRowsTo(m_defaultActiveFrom);
            Alter.Table("UserContact").AddColumn("ActiveTo").AsDateTime().Nullable();

            Execute.WithConnection((connection, transaction) =>
            {
                var userData = Query.Conn(connection, transaction).Select<UserData>()
                    .From("UserData")
                    .Run();

                var userContact = Query.Conn(connection, transaction).Select<UserContact>()
                    .From("UserContact")
                    .Run();

                foreach (var data in userData)
                {
                    Query.Conn(connection, transaction).Update("UserData")
                        .Set("ActiveFrom", data.VerifiedWhen ?? m_defaultActiveFrom)
                        .Where("Id", data.Id)
                        .Run();
                }

                foreach (var contact in userContact)
                {
                    Query.Conn(connection, transaction).Update("UserContact")
                        .Set("ActiveFrom", contact.CreateTime)
                        .Where("Id", contact.Id)
                        .Run();
                }
            });

            Delete.Column("VerifiedWhen").FromTable("UserData");
            Delete.Column("CreateTime").FromTable("UserContact");
        }

        private class UserData
        {
            public int Id { get; set; }
            public DateTime? VerifiedWhen { get; set; }
        }

        private class UserContact
        {
            public int Id { get; set; }
            public DateTime? CreateTime { get; set; }

        }
    }
}