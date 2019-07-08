using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Configuration;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.DataResult;
using Ridics.Authentication.Core.Models.Enum;
using Ridics.Authentication.DataEntities;
using Ridics.Authentication.DataEntities.Types;
using Ridics.Authentication.Shared;

namespace Ridics.Authentication.Core.Managers
{
    public class ContactManager : ManagerBase
    {
        private readonly ContactFormatterManager m_contactFormatterManager;
        private readonly ContactValidatorManager m_contactValidatorManager;

        public ContactManager(ILogger logger, ITranslator translator, IMapper mapper, IPaginationConfiguration paginationConfiguration, ContactFormatterManager contactFormatterManager, ContactValidatorManager contactValidatorManager) : base(logger, translator, mapper, paginationConfiguration)
        {
            m_contactFormatterManager = contactFormatterManager;
            m_contactValidatorManager = contactValidatorManager;
        }

        public DataResult<IList<UserContactModel>> ValidateAndFormatContactList(IList<UserContactModel> contactList, UserAction userAction)
        {
            foreach (var contact in contactList)
            {
                var result = ValidateAndFormatContact(contact.Value, contact.Type, userAction, contact.User?.Id);

                if (result.Succeeded)
                {
                    contact.Value = result.Result;
                }
                else
                {
                    return Error<IList<UserContactModel>>(result.Error);
                }
            }

            return Success(contactList);
        }

        public DataResult<string> ValidateAndFormatContact(string contact, ContactTypeEnumModel contactType, UserAction userAction, int? userId = null)
        {
            var validationResult = ValidateContact(contact, contactType, userAction);
            if (validationResult.HasError) return validationResult;

            var formattedContactValue = m_contactFormatterManager.FormatContact(contact, contactType);

            var uniqueValidationResult = ValidateUniqueContact(formattedContactValue, contactType, userId, userAction);
            if (uniqueValidationResult.HasError) return uniqueValidationResult;

            return Success(formattedContactValue);
        }

        private DataResult<string> ValidateContact(string contact, ContactTypeEnumModel contactType, UserAction userAction)
        {
            var isValid = m_contactValidatorManager.ValidateContact(contactType, contact);
            if (!isValid)
                return Error<string>(m_translator.Translate("save-user-contact-failed"),
                    GetErrorCodeForValidation(contactType, userAction));

            return Success(contact);
        }

        private DataResult<string> ValidateUniqueContact(string contact, ContactTypeEnumModel contactType, int? userId, UserAction userAction)
        {
            var isUnique = m_contactValidatorManager.ValidateUniqueContact(contactType, contact, userId);
            if (!isUnique)
                return Error<string>(m_translator.Translate("save-user-contact-failed"),
                    GetErrorCodeForUniqueValidation(contactType, userAction));

            return Success(contact);
        }

        private string GetErrorCodeForValidation(ContactTypeEnumModel contactType, UserAction userAction)
        {
            switch (userAction)
            {
                case UserAction.Create:
                    return contactType == ContactTypeEnumModel.Email
                        ? DataResultErrorCode.CreateUserErrorContactEmailNotValid
                        : DataResultErrorCode.CreateUserErrorContactPhoneNotValid;
                case UserAction.Update:
                    return contactType == ContactTypeEnumModel.Email
                        ? DataResultErrorCode.SaveUserErrorContactEmailNotValid
                        : DataResultErrorCode.SaveUserErrorContactPhoneNotValid;
                default:
                    throw new ArgumentOutOfRangeException(nameof(userAction), userAction, null);
            }
        }

        private string GetErrorCodeForUniqueValidation(ContactTypeEnumModel contactType, UserAction userAction)
        {
            switch (userAction)
            {
                case UserAction.Create:
                    return contactType == ContactTypeEnumModel.Email
                        ? DataResultErrorCode.CreateUserErrorUserContactEmail
                        : DataResultErrorCode.CreateUserErrorUserContactPhone;
                case UserAction.Update:
                    return contactType == ContactTypeEnumModel.Email
                        ? DataResultErrorCode.SaveUserErrorContactEmailNotUnique
                        : DataResultErrorCode.SaveUserErrorContactPhoneNumberNotUnique;
                default:
                    throw new ArgumentOutOfRangeException(nameof(userAction), userAction, null);
            }
        }
    }
}