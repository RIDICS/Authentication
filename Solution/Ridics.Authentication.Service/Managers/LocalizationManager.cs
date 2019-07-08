using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ridics.Authentication.Service.Models.Localization;
using Scalesoft.Localization.AspNetCore;

namespace Ridics.Authentication.Service.Managers
{
    public class LocalizationManager
    {
        private readonly ILocalizationService m_localization;

        public LocalizationManager(ILocalizationService localization)
        {
            m_localization = localization;
        }

        public List<SelectListItem> GetLanguageListForSelect()
        {
            var cultureItems = GetLanguageList().Select(c => new SelectListItem(c.Label, c.Name, c.IsSelected))
                .ToList();

            return cultureItems;
        }

        public List<SelectListItem> GetLanguageListForSelect(string selectedCultureName)
        {
            var cultureItems = GetLanguageList().Select(c => new SelectListItem(c.Label, c.Name, c.Name == selectedCultureName))
                .ToList();

            return cultureItems;
        }

        public List<CultureModel> GetLanguageList()
        {
            var currentCulture = m_localization.GetRequestCulture();
            var cultureItems = m_localization.GetSupportedCultures()
                .Select(c => new CultureModel
                {
                    Name = c.Name,
                    Label = c.NativeName,
                    IsSelected = c.Equals(currentCulture)
                }).ToList();

            return cultureItems;
        }
    }
}
