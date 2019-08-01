using System.Globalization;

namespace Ridics.Authentication.Shared
{
    public interface ITranslator
    {
        string Translate(string translationKey);

        string Translate(string translationKey, string scope);

        CultureInfo GetRequestCulture();
    }
}