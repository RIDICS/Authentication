using System.Globalization;
using Scalesoft.Localization.AspNetCore;
using Ridics.Authentication.Modules.Shared;

namespace Ridics.Authentication.Service.Helpers.DynamicModule
{
    public class DynamicModuleLocalization : IModuleLocalization
    {
        private readonly ILocalizationService m_localization;

        public DynamicModuleLocalization(ILocalizationService localization)
        {
            m_localization = localization;
        }

        public string Translate(string key)
        {
            return m_localization.Translate(key).Value;
        }

        public string Translate(string key, string scope)
        {
            return m_localization.Translate(key, scope).Value;
        }

        public CultureInfo GetCurrentCultureInfo()
        {
            return m_localization.GetRequestCulture();
        }
    }
}
