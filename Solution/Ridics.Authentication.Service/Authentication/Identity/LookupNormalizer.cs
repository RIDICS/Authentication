using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace Ridics.Authentication.Service.Authentication.Identity
{
    public class LookupNormalizer : ILookupNormalizer
    {
        public string Normalize(string key)
        {
            return key.ToLower(CultureInfo.InvariantCulture);
        }
    }
}
