using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Configuration;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.Enum;
using Ridics.Authentication.Core.Utils.Validator;
using Ridics.Authentication.DataEntities.Entities.Enums;
using Ridics.Authentication.Shared;

namespace Ridics.Authentication.Core.Managers
{
    public class ContactValidatorManager : ManagerBase
    {
        private readonly Dictionary<ContactTypeEnumModel, IContactValidator> m_contactValidatorsDict;
        private readonly UniqueContactValidator m_uniqueContactValidator;

        public ContactValidatorManager(
            IEnumerable<IContactValidator> contactValidators, ILogger logger, ITranslator translator,
            IMapper mapper, IPaginationConfiguration paginationConfiguration, UniqueContactValidator uniqueContactValidator
        ) : base(logger, translator, mapper, paginationConfiguration)
        {
            m_uniqueContactValidator = uniqueContactValidator;
            m_contactValidatorsDict = contactValidators.ToDictionary(x => x.Type);
        }

        public bool ValidateContact(UserContactModel contact)
        {
            return m_contactValidatorsDict[contact.Type].Validate(contact.Value);
        }

        public bool ValidateContact(ContactTypeEnum type, string value)
        {
            var contactTypeEnumModel = Mapper.Map<ContactTypeEnumModel>(type);

            return ValidateContact(contactTypeEnumModel, value);
        }

        public bool ValidateContact(ContactTypeEnumModel type, string value)
        {
            return m_contactValidatorsDict[type].Validate(value);
        }

        public bool ValidateUniqueContact(ContactTypeEnumModel type, string value, int? userId = null)
        {
            var contactTypeEnum = Mapper.Map<ContactTypeEnum>(type);

            return ValidateUniqueContact(contactTypeEnum, value, userId);
        }

        public bool ValidateUniqueContact(ContactTypeEnum type, string value, int? userId = null)
        {
            return m_uniqueContactValidator.Validate(type, value, userId);
        }
    }
}
