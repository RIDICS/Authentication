using Scalesoft.Localization.AspNetCore;
using Ridics.Authentication.Shared;

namespace Ridics.Authentication.Service.SharedInterfaceImpl
{
    public class TranslatorImpl : ITranslator
    {
        private readonly ILocalizationService m_localization;

        public TranslatorImpl(ILocalizationService localization)
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
    }
}
