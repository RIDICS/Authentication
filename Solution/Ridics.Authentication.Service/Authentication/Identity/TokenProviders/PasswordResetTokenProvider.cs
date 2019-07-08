using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Ridics.Authentication.Service.Authentication.Identity.Models;

namespace Ridics.Authentication.Service.Authentication.Identity.TokenProviders
{
    public class PasswordResetTokenProvider : TotpSecurityStampBasedTokenProvider<ApplicationUser>
    {
        private readonly PasswordResetTokenValidator m_passwordResetTokenValidator;

        public PasswordResetTokenProvider(PasswordResetTokenValidator passwordResetTokenValidator)
        {
            m_passwordResetTokenValidator = passwordResetTokenValidator;
        }

        public override Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
        {
            return Task.FromResult(false);
        }

        public override Task<bool> ValidateAsync(string purpose, string token, UserManager<ApplicationUser> manager, ApplicationUser user)
        {
            return m_passwordResetTokenValidator.ValidateAsync(token, user);
        }

        public static string ProviderName => nameof(PasswordResetTokenProvider);
    }
}