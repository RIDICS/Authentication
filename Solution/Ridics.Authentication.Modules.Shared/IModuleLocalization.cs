using System.Globalization;

namespace Ridics.Authentication.Modules.Shared
{
    public interface IModuleLocalization
    {
        string Translate(string key);
        string Translate(string key, string scope);
        CultureInfo GetCurrentCultureInfo();
    }
}
