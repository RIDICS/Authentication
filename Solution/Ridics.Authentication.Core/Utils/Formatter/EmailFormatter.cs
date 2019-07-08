using Ridics.Authentication.Core.Models.Enum;

namespace Ridics.Authentication.Core.Utils.Formatter
{
    public class EmailFormatter : IContactFormatter
    {
        public ContactTypeEnumModel Type => ContactTypeEnumModel.Email;

        /// <summary>
        /// Formats provided email to lower case.
        /// </summary>
        /// <param name="email">Email to be converter to lower case invariant</param>
        /// <returns>Lower case of <paramref name="email"/></returns>
        public string FormatContact(string email)
        {
            return email.ToLowerInvariant();
        }
    }
}