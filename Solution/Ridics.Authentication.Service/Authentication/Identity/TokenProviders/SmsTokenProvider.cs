using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Ridics.Authentication.Service.Authentication.Identity.Models;

namespace Ridics.Authentication.Service.Authentication.Identity.TokenProviders
{
    public class SmsTokenProvider : PhoneNumberTokenProvider<ApplicationUser>
    {
        private readonly TwoFactorTokenValidator m_twoFactorTokenValidator;

        public SmsTokenProvider(TwoFactorTokenValidator twoFactorTokenValidator)
        {
            m_twoFactorTokenValidator = twoFactorTokenValidator;
        }

        public override Task<bool> ValidateAsync(string purpose, string token, UserManager<ApplicationUser> manager, ApplicationUser user)
        {
            return m_twoFactorTokenValidator.ValidateAsync(ProviderName, token, user);
        }

        public override Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PhoneNumber) && user.PhoneNumberConfirmed);
        }

        public static string ProviderName => nameof(SmsTokenProvider);
    }
}