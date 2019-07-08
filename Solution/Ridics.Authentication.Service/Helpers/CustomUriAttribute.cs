using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Ridics.Authentication.Service.Helpers
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomUriAttribute : ValidationAttribute
    {
        private const string DefaultErrorLocalizationKey = "not-valid-uri";

        private readonly Regex m_uriPattern = new Regex(@"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)");

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            var valueAsString = Convert.ToString(value, CultureInfo.CurrentCulture);

            if (string.IsNullOrEmpty(valueAsString))
            {
                return ValidationResult.Success;
            }

            var m = m_uriPattern.Match(valueAsString);

            var isValid = (m.Success && m.Index == 0 && m.Length == valueAsString.Length);

            var errorLocalizationKey = string.IsNullOrEmpty(ErrorMessage) ? DefaultErrorLocalizationKey : ErrorMessage;

            var localizedErrorMsg = FormatErrorMessage(errorLocalizationKey);

            return isValid ? ValidationResult.Success : new ValidationResult(localizedErrorMsg);
        }
    }
}