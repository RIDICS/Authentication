using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Configuration;
using Ridics.Authentication.Core.Models.Enum;
using Ridics.Authentication.Core.Utils.Formatter;
using Ridics.Authentication.DataEntities.Entities.Enums;
using Ridics.Authentication.Shared;

namespace Ridics.Authentication.Core.Managers
{
    public class ContactFormatterManager : ManagerBase
    {
        private readonly Dictionary<ContactTypeEnumModel, IContactFormatter> m_contactValidatorsDict;

        public ContactFormatterManager(IEnumerable<IContactFormatter> contactFormatters, ILogger logger, ITranslator translator, IMapper mapper, IPaginationConfiguration paginationConfiguration) : base(logger, translator, mapper, paginationConfiguration)
        {
            m_contactValidatorsDict = contactFormatters.ToDictionary(x => x.Type); ;
        }

        public string FormatContact(string value, ContactTypeEnum contactType)
        {
            var contactTypeEnumModel = Mapper.Map<ContactTypeEnumModel>(contactType);

            return FormatContact(value, contactTypeEnumModel);
        }

        public string FormatContact(string value, ContactTypeEnumModel contactType)
        {
            var formattedValue = m_contactValidatorsDict[contactType].FormatContact(value);

            return formattedValue;
        }

        public string FormatEmail(string email)
        {
            return FormatContact(email, ContactTypeEnumModel.Email);
        }

        public string FormatPhoneNumber(string phoneNumber)
        {
            return FormatContact(phoneNumber, ContactTypeEnumModel.Phone);
        }
    }
}