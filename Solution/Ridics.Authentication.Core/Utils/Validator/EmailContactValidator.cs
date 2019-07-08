using System;
using Ridics.Authentication.Core.Models.Enum;

namespace Ridics.Authentication.Core.Utils.Validator
{
    public class EmailContactValidator : IContactValidator
    {
        public ContactTypeEnumModel Type => ContactTypeEnumModel.Email;

        /// <summary>
        /// Checks if email is valid. Copied from: https://stackoverflow.com/questions/1365407/c-sharp-code-to-validate-email-address
        /// </summary>
        /// <param name="email">Email address to be validated</param>
        /// <returns>True if email is valid, false otherwise</returns>
        public bool Validate(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch (ArgumentNullException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}