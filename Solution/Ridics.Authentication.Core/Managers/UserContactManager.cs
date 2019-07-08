using System;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Configuration;
using Ridics.Authentication.Core.Models.DataResult;
using Ridics.Authentication.Core.Models.Enum;
using Ridics.Authentication.DataEntities;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;
using Ridics.Authentication.DataEntities.Exceptions;
using Ridics.Authentication.DataEntities.UnitOfWork;
using Ridics.Authentication.Shared;
using Ridics.Core.Shared;
using Ridics.Core.Shared.Exceptions;

namespace Ridics.Authentication.Core.Managers
{
    public class UserContactManager : ManagerBase
    {
        private readonly UserContactUoW m_userContactUoW;
        private readonly CodeGeneratorManager m_codeGeneratorManager;
        private readonly ContactValidatorManager m_contactValidatorManager;
        private readonly ContactFormatterManager m_contactFormatterManager;

        public UserContactManager(UserContactUoW userContactUoW, ILogger logger, ITranslator translator, IMapper mapper,
            IPaginationConfiguration paginationConfiguration, CodeGeneratorManager codeGeneratorManager, ContactValidatorManager contactValidatorManager, ContactFormatterManager contactFormatterManager) : base(logger, translator, mapper,
            paginationConfiguration)
        {
            m_userContactUoW = userContactUoW;
            m_codeGeneratorManager = codeGeneratorManager;
            m_contactValidatorManager = contactValidatorManager;
            m_contactFormatterManager = contactFormatterManager;
        }

        public DataResult<string> GeneratePhoneConfirmCode()
        {
            return GenerateConfirmCode(ContactTypeEnumModel.Phone);
        }

        public DataResult<string> GenerateEmailConfirmCode()
        {
            return GenerateConfirmCode(ContactTypeEnumModel.Email);
        }

        private DataResult<string> GenerateConfirmCode(ContactTypeEnumModel contactTypeModel)
        {
            var code = m_codeGeneratorManager.GenerateContactConfirmCode(contactTypeModel);

            return Success(code);
        }

        public DataResult<string> SavePhoneConfirmCode(int userId, string code)
        {
            return SaveConfirmCode(userId, code, ContactTypeEnum.Phone);
        }

        public DataResult<string> SaveEmailConfirmCode(int userId, string code)
        {
            return SaveConfirmCode(userId, code, ContactTypeEnum.Email);
        }

        private DataResult<string> SaveConfirmCode(int userId, string code, ContactTypeEnum contactType)
        {
            try
            {
                m_userContactUoW.SaveConfirmCode(userId, code, contactType);
                return Success(code);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<string>(e.Message);
            }
        }

        public DataResult<bool> ConfirmPhone(int userId, string code)
        {
            return ConfirmContact(userId, code, ContactTypeEnum.Phone);
        }

        public DataResult<bool> ConfirmEmail(int userId, string code)
        {
            return ConfirmContact(userId, code, ContactTypeEnum.Email);
        }

        private DataResult<bool> ConfirmContact(int userId, string code, ContactTypeEnum contactType)
        {
            try
            {
                var contact = m_userContactUoW.GetUserContact(userId, contactType);

                if (!m_contactValidatorManager.ValidateUniqueContact(contactType, contact.Value, userId))
                {
                    return Error<bool>(m_translator.Translate("confirm-contact-failed"),
                        contactType == ContactTypeEnum.Email
                            ? DataResultErrorCode.EmailNotUnique
                            : DataResultErrorCode.PhoneNumberNotUnique);
                }

                var confirmedSuccessfully = m_userContactUoW.ConfirmCode(userId, code, contactType);

                return Success(confirmedSuccessfully);
            }
            catch (NoResultException<UserContactEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        private DataResult<string> GetUserConfirmCode(int userId, ContactTypeEnum contactType)
        {
            try
            {
                var code = m_userContactUoW.GetUserConfirmCode(userId, contactType);

                return Success(code);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<string>(m_translator.Translate("invalid-user-id"), DataResultErrorCode.UserNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<string>(e.Message);
            }
        }

        public DataResult<string> GetUserConfirmCodePhoneNumber(int userId)
        {
            return GetUserConfirmCode(userId, ContactTypeEnum.Phone);
        }

        public DataResult<string> GetUserConfirmCodeEmail(int userId)
        {
            return GetUserConfirmCode(userId, ContactTypeEnum.Email);
        }

        public DataResult<bool> ChangePhone(int userId, string newValue, string code)
        {
            return ChangeContact(userId, newValue, code, ContactTypeEnum.Phone);
        }

        public DataResult<bool> ChangeEmail(int userId, string newValue, string code)
        {
            return ChangeContact(userId, newValue, code, ContactTypeEnum.Email);
        }

        private DataResult<bool> ChangeContact(int userId, string newValue, string code, ContactTypeEnum contactType)
        {
            var isValid = m_contactValidatorManager.ValidateContact(contactType, newValue);
            if (!isValid)
            {
                return Error<bool>(m_translator.Translate("save-contact-failed"),
                    contactType == ContactTypeEnum.Email
                        ? DataResultErrorCode.EmailNotValid
                        : DataResultErrorCode.PhoneNumberNotValid);
            }

            var formattedValue = FormatContact(contactType, newValue);

            var isUnique = m_contactValidatorManager.ValidateUniqueContact(contactType, formattedValue, userId);
            if (!isUnique)
            {
                return Error<bool>(m_translator.Translate("save-contact-failed"),
                    contactType == ContactTypeEnum.Email
                        ? DataResultErrorCode.EmailNotUnique
                        : DataResultErrorCode.PhoneNumberNotUnique);
            }

            try
            {
                var newContact = new UserContactEntity
                {
                    Value = formattedValue,
                    Type = contactType,
                    ConfirmCode = code,
                    ConfirmCodeChangeTime = DateTime.UtcNow,
                };

                m_userContactUoW.ChangeContact(userId, newContact);
                return Success(true);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        private string FormatContact(ContactTypeEnum contactType, string newValue)
        {
            var formattedValue = m_contactFormatterManager.FormatContact(newValue, contactType);

            return formattedValue;
        }
    }
}
