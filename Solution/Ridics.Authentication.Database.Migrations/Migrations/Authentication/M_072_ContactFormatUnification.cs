using System;
using System.Collections.Generic;
using System.Data;
using FluentMigrator;
using PhoneNumbers;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(072)]
    public class M_072_ContactFormatUnification : ForwardOnlyMigration
    {
        public M_072_ContactFormatUnification()
        {
        }

        public override void Up()
        {
            Execute.WithConnection((connection, transaction) =>
            {
                var contacts = GetContacts(connection, transaction);
                var confirmedValues = new HashSet<string>(); // both e-mail and phone values can be stored in single Set because of different (non-conflict) value formats

                foreach (var contact in contacts)
                {
                    var formattedContactValue = GetFormattedValue(contact.Value, contact.Type);

                    if (contact.Confirmed && !confirmedValues.Add(formattedContactValue))
                    {
                        var exceptionMessage = $"Duplicate confirmed contact ({contact.Type}) value for {formattedContactValue}. You need to manually resolve the problem.";
                        Console.WriteLine(exceptionMessage);
                        throw new MigrationException(exceptionMessage);
                    }

                    UpdateContactValue(contact.Id, formattedContactValue, connection, transaction);
                }
            });
        }

        private IEnumerable<ContactResult> GetContacts(IDbConnection connection, IDbTransaction transaction)
        {
            return Query.Conn(connection, transaction).Select<ContactResult>().From("UserContact")
                .Run();
        }

        private string GetFormattedEmailValue(string emailValue)
        {
            return emailValue.ToLowerInvariant();
        }

        private string GetFormattedPhoneValue(string phoneValue)
        {
            var phoneUtil = PhoneNumberUtil.GetInstance();

            var phoneNumber = phoneUtil.Parse(phoneValue, null);

            return phoneUtil.Format(phoneNumber, PhoneNumberFormat.E164);
        }

        private string GetFormattedValue(string contactValue, string contactType)
        {
            switch (contactType)
            {
                case "Email":
                    return GetFormattedEmailValue(contactValue);
                case "Phone":
                    return GetFormattedPhoneValue(contactValue);
                default:
                    return contactValue;
            }
        }

        private void UpdateContactValue(int contactId, string value, IDbConnection connection, IDbTransaction transaction)
        {
            Query.Conn(connection, transaction).Update("UserContact").Set("Value", value)
                .Where("Id", contactId)
                .Run();
        }

        private class ContactResult
        {
            public int Id { get; set; }
            public string Value { get; set; }
            public bool Confirmed { get; set; }
            public string Type { get; set; }
        }
    }
}