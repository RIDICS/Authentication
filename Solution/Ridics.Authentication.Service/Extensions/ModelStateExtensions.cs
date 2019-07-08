using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ridics.Authentication.Service.Extensions
{
    public static class ModelStateExtensions
    {
        public static void AddModelError(this ModelStateDictionary modelStateDictionary, string errorMessage)
        {
            modelStateDictionary.AddModelError(string.Empty, errorMessage);
        }
    }
}
