using PhoneNumbers;
using Ridics.Authentication.Core.Models.Enum;

namespace Ridics.Authentication.Core.Utils.Formatter
{
    public class PhoneNumberFormatter : IContactFormatter
    {
        public ContactTypeEnumModel Type => ContactTypeEnumModel.Phone;

        /// <summary>
        /// Formats provided phone to E164 phone number format 
        /// </summary>
        /// <param name="phone">Phone to be converted to E164 format</param>
        /// <returns>E164 format of <paramref name="phone"/></returns>
        public string FormatContact(string phone)
        {
            var phoneUtil = PhoneNumberUtil.GetInstance();

            var phoneNumber = phoneUtil.Parse(phone, null);

            return phoneUtil.Format(phoneNumber, PhoneNumberFormat.E164);
        }
    }
}