using Microsoft.AspNetCore.Mvc;
using Scalesoft.Localization.AspNetCore;

namespace Ridics.Authentication.Service.Controllers
{
    public class LocalizationController : AuthControllerBase<LocalizationController>
    {
        private readonly ILocalizationService m_localization;
        private readonly IDictionaryService m_dictionary;

        public LocalizationController(ILocalizationService localization, IDictionaryService dictionary)
        {
            m_localization = localization;
            m_dictionary = dictionary;
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            if (
                string.IsNullOrEmpty(culture)
                || string.IsNullOrEmpty(returnUrl)
            )
            {
                return BadRequest();
            }

            m_localization.SetCulture(culture);
            
            return LocalRedirect(returnUrl);
        }

        public IActionResult Dictionary(string scope)
        {
            return Json(m_dictionary.GetDictionary(scope));
        }
    }
}
