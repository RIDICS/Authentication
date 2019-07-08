using PhoneNumbers;
using Ridics.Authentication.Core.Models.Enum;

namespace Ridics.Authentication.Core.Utils.Validator
{
    public class PhoneContactValidator : IContactValidator
    {
        public ContactTypeEnumModel Type => ContactTypeEnumModel.Phone;

        /// <summary>
        /// Checks if phone number is valid. Valid phone number must have country code in standard format, i.e.has to start with + sign.
        /// </summary>
        /// <param name="phone">Phone number to be validated</param>
        /// <returns>True if phone number is valid, false otherwise</returns>
        public bool Validate(string phone)
        {
            var phoneUtil = PhoneNumberUtil.GetInstance();
            try
            {
                var phoneNumber = phoneUtil.Parse(phone, null);

                return phoneUtil.IsValidNumber(phoneNumber);
            }
            catch (NumberParseException)
            {
                return false;
            }
        }
    }
}