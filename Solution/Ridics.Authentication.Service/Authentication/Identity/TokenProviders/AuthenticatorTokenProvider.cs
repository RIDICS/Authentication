using Microsoft.AspNetCore.Identity;
using Ridics.Authentication.Service.Authentication.Identity.Models;

namespace Ridics.Authentication.Service.Authentication.Identity.TokenProviders
{
    public class AuthenticatorTokenProvider : AuthenticatorTokenProvider<ApplicationUser>
    {
        public static string ProviderName => nameof(AuthenticatorTokenProvider);
    }
}