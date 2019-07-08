using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;

namespace Ridics.Authentication.Service.Helpers
{
    public class CustomUriAttributeAdapterProvider : IValidationAttributeAdapterProvider
    {
        private readonly IValidationAttributeAdapterProvider m_baseProvider = new ValidationAttributeAdapterProvider();

        public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
        {
            if (attribute is CustomUriAttribute uriAttribute)
                return new CustomUriAttributeAdapter(uriAttribute, stringLocalizer);
            else
                return m_baseProvider.GetAttributeAdapter(attribute, stringLocalizer);
        }
    }
}