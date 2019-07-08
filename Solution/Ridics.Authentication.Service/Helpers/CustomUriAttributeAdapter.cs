using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace Ridics.Authentication.Service.Helpers
{
    public class CustomUriAttributeAdapter : AttributeAdapterBase<CustomUriAttribute>
    {
        public CustomUriAttributeAdapter(CustomUriAttribute attribute, IStringLocalizer stringLocalizer) : base(attribute, stringLocalizer)
        {
            
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-customuri", GetErrorMessage(context));
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return GetErrorMessage(validationContext.ModelMetadata, validationContext.ModelMetadata.GetDisplayName());
        }
    }
}